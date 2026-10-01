FROM python:3.10-slim
WORKDIR /app
COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt
COPY service/app.py ./app.py
ENV PORT=8003
EXPOSE 8003
CMD ["python", "app.py"]
