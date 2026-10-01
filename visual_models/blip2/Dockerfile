FROM python:3.10-slim
WORKDIR /app
COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt
# copy the service app from the context (visual_models/service/app.py)
COPY service/app.py ./app.py
ENV PORT=8001
EXPOSE 8001
CMD ["python", "app.py"]
