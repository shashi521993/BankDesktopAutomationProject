from fastapi import FastAPI, File, UploadFile, Form
from fastapi.responses import JSONResponse
from pydantic import BaseModel
import uvicorn
import json
import os

app = FastAPI(title="LLM Vision Shim - Dummy")

class PredictResponse(BaseModel):
	model: str
	image_name: str
	controls_identified: list
	verdict: str
	confidence: float
	raw_spec: dict | None

@app.post("/predict")
async def predict(model: str = Form("unknown"), image: UploadFile = File(...), spec: str = Form(default="")):
	# Save uploaded file temporarily
	content = await image.read()
	img_name = image.filename or "uploaded.png"

	# Try parse spec
	parsed_spec = None
	if spec:
		try:
			parsed_spec = json.loads(spec)
		except Exception:
			parsed_spec = {"_raw": spec}

	# Simple deterministic heuristic for demo
	lower = img_name.lower()
	if "login" in lower:
		controls = [{"label": "Login", "type": "Button", "confidence": 0.95}, {"label": "txtUserId", "type": "Edit", "confidence": 0.92}]
		verdict = "pass"
		conf = 0.93
	elif "dashboard" in lower:
		controls = [{"label": "Create Account", "type": "Button", "confidence": 0.89}]
		verdict = "needs_human_review"
		conf = 0.72
	else:
		controls = []
		verdict = "fail"
		conf = 0.25

	resp = PredictResponse(
		model=model,
		image_name=img_name,
		controls_identified=controls,
		verdict=verdict,
		confidence=conf,
		raw_spec=parsed_spec
	)

	return JSONResponse(content=json.loads(resp.json()))

if __name__ == "__main__":
	port = int(os.environ.get("PORT", "8001"))
	uvicorn.run(app, host="0.0.0.0", port=port)
