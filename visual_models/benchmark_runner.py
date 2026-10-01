import requests
import json
import os
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
DATASET = ROOT / 'visual' / 'dataset' / 'initial'
MODELS_FILE = ROOT / 'visual_models' / 'models.json'
OUT = ROOT / 'visual' / 'benchmark' / 'results'

OUT.mkdir(parents=True, exist_ok=True)

with open(MODELS_FILE) as f:
	models = json.load(f)

images = list(DATASET.glob('*.png'))
if not images:
	print('No images found in', DATASET)

for m in models:
	model_name = m['name']
	url = m['url']
	model_out_dir = OUT / model_name
	model_out_dir.mkdir(parents=True, exist_ok=True)
	print(f'Running model {model_name} at {url}...')
	for img in images:
		print(' -', img.name)
		files = {'image': (img.name, open(img, 'rb'), 'image/png')}
		# minimal expected spec: read *_tree.json if available
		tree_file = img.with_name(img.stem + '_tree.json')
		spec = {}
		if tree_file.exists():
			try:
				spec = json.load(open(tree_file))
			except Exception:
				spec = {}
		data = {'model': m.get('model', model_name), 'spec': json.dumps(spec)}
		try:
			resp = requests.post(url, files=files, data=data, timeout=10)
			out_path = model_out_dir / (img.stem + '.json')
			with open(out_path, 'w', encoding='utf-8') as outf:
				json.dump({'status_code': resp.status_code, 'response': resp.json()}, outf, indent=2)
		except Exception as e:
			with open(model_out_dir / (img.stem + '.json'), 'w') as outf:
				json.dump({'error': str(e)}, outf)

print('Benchmark run complete. Results in', OUT)
