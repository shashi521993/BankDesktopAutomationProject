import requests
import json
import os
import logging
import time
from pathlib import Path
from datetime import datetime

ROOT = Path(__file__).resolve().parent.parent
DATASET = ROOT / 'visual' / 'dataset' / 'initial'
MODELS_FILE = ROOT / 'visual_models' / 'models.json'
OUT = ROOT / 'visual' / 'benchmark' / 'results'

OUT.mkdir(parents=True, exist_ok=True)

# Config via environment (optional)
MAX_RETRIES = int(os.environ.get('VM_MAX_RETRIES', '5'))
REQUEST_TIMEOUT = int(os.environ.get('VM_REQUEST_TIMEOUT', '20'))
RETRY_BACKOFF = float(os.environ.get('VM_RETRY_BACKOFF', '1.5'))

logging.basicConfig(level=logging.INFO, format='%(asctime)s %(levelname)s: %(message)s')
logger = logging.getLogger('benchmark')


def post_with_retries(url, files, data, timeout, max_retries=5, backoff=1.5):
	attempt = 0
	last_exc = None
	while attempt < max_retries:
		attempt += 1
		try:
			start = time.time()
			resp = requests.post(url, files=files, data=data, timeout=timeout)
			elapsed = time.time() - start
			return {'ok': True, 'status_code': resp.status_code, 'elapsed': elapsed, 'text': resp.text, 'attempts': attempt}
		except Exception as e:
			last_exc = e
			sleep = backoff * (2 ** (attempt - 1))
			logger.warning('Request to %s failed on attempt %d/%d: %s. Retrying in %.1fs', url, attempt, max_retries, e, sleep)
			time.sleep(sleep)
	return {'ok': False, 'error': str(last_exc), 'attempts': attempt}


def load_models():
	if not MODELS_FILE.exists():
		logger.error('models.json not found at %s', MODELS_FILE)
		return []
	try:
		return json.load(open(MODELS_FILE, encoding='utf-8'))
	except Exception as e:
		logger.exception('Failed to load models.json: %s', e)
		return []


def load_images():
	imgs = list(DATASET.glob('*.png'))
	if not imgs:
		logger.warning('No images found in %s', DATASET)
	return imgs


def read_spec(img_path):
	tree_file = img_path.with_name(img_path.stem + '_tree.json')
	if tree_file.exists():
		try:
			return json.load(open(tree_file, encoding='utf-8'))
		except Exception:
			logger.exception('Failed to parse tree json: %s', tree_file)
			return {}
	return {}


def save_result(out_dir: Path, img_name: str, payload: dict):
	out_path = out_dir / (img_name + '.json')
	with open(out_path, 'w', encoding='utf-8') as outf:
		json.dump(payload, outf, indent=2)


def main():
	models = load_models()
	images = load_images()

	for m in models:
		model_name = m.get('name') or m.get('model') or 'model'
		url = m.get('url')
		model_out_dir = OUT / model_name
		model_out_dir.mkdir(parents=True, exist_ok=True)
		logger.info('Running model %s at %s', model_name, url)
		for img in images:
			logger.info('Processing image %s', img.name)
			spec = read_spec(img)
			data = {'model': m.get('model', model_name), 'spec': json.dumps(spec)}
			with open(img, 'rb') as fh:
				files = {'image': (img.name, fh, 'image/png')}
				result = post_with_retries(url, files, data, timeout=REQUEST_TIMEOUT, max_retries=MAX_RETRIES, backoff=RETRY_BACKOFF)

			record = {
				'timestamp': datetime.utcnow().isoformat() + 'Z',
				'model': model_name,
				'image': img.name,
				'attempts': result.get('attempts', 0),
			}

			if result.get('ok'):
				record.update({'status_code': result.get('status_code'), 'elapsed': result.get('elapsed')})
				# try to parse json response
				try:
					record['response'] = json.loads(result.get('text') or '{}')
				except Exception:
					record['response_text'] = result.get('text')
			else:
				record.update({'error': result.get('error')})

			save_result(model_out_dir, img.stem, record)
			logger.info('Wrote result for %s/%s: attempts=%s ok=%s', model_name, img.name, record.get('attempts'), record.get('status_code') if record.get('status_code') else 'ERROR')

	logger.info('Benchmark run complete. Results in %s', OUT)


if __name__ == '__main__':
	main()
