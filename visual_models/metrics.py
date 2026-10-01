import json
from pathlib import Path
from collections import Counter

ROOT = Path(__file__).resolve().parent.parent
RESULTS = ROOT / 'visual' / 'benchmark' / 'results'


def load_results():
	models = {}
	for model_dir in RESULTS.iterdir():
		if not model_dir.is_dir():
			continue
		models[model_dir.name] = {}
		for j in model_dir.glob('*.json'):
			try:
				data = json.load(open(j, encoding='utf-8'))
			except Exception as e:
				data = {'error': str(e)}
			models[model_dir.name][j.name] = data
	return models


def compute_control_metrics(models_results):
	# simple metric: how many controls identified vs expected (from spec)
	rows = []
	for model_name, files in models_results.items():
		for fname, content in files.items():
			img = fname.replace('.json','')
			spec_path = ROOT / 'visual' / 'dataset' / 'initial' / (img + '_tree.json')
			expected = []
			try:
				spec = json.load(open(spec_path, encoding='utf-8'))
				# flatten spec names
				def collect_names(node, acc):
					n = node.get('Name') or node.get('name')
					if n:
						acc.append(n)
					for c in node.get('Children', []) or node.get('children', []) or []:
						collect_names(c, acc)
				collect = []
				collect_names(spec, collect)
				expected = collect
			except Exception:
				expected = []

			predicted = []
			try:
				resp = content.get('response', {})
				preds = resp.get('controls_identified') or []
				predicted = [p.get('name') for p in preds if p.get('name')]
			except Exception:
				predicted = []

			exp_set = set(expected)
			pred_set = set(predicted)
			tp = len(exp_set & pred_set)
			fp = len(pred_set - exp_set)
			fn = len(exp_set - pred_set)
			precision = tp / (tp+fp) if (tp+fp)>0 else None
			recall = tp / (tp+fn) if (tp+fn)>0 else None
			rows.append({'model': model_name, 'image': img, 'tp': tp, 'fp': fp, 'fn': fn, 'precision': precision, 'recall': recall})
	return rows


if __name__ == '__main__':
	models = load_results()
	rows = compute_control_metrics(models)
	print(json.dumps(rows, indent=2))
	out = RESULTS.parent / 'summary.json'
	json.dump(rows, open(out, 'w', encoding='utf-8'), indent=2)
	print('Wrote', out)
