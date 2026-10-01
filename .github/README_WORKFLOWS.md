Workflows in this repository

1) ingest-and-generate.yml
   - Trigger: pull_request to main
   - Purpose: builds and runs the tools/roslyn-ingest and tools/test-doc-generator console apps to produce generated IR and test docs. Artifacts are uploaded to the PR run.
   - Notes: runs on ubuntu-latest and uses .NET 6 SDK to build and run the tools. Ensure the tools target net6.0.

2) scheduled-benchmark.yml
   - Trigger: daily at 02:00 UTC and manual dispatch
   - Purpose: runs the visual_models/benchmark_runner.py and visual_models/metrics.py on a self-hosted GPU runner labeled 'self-hosted-gpu'.
   - Notes: The self-hosted runner must have Python 3.10+, pip, and network access to the model endpoints defined in visual_models/models.json. If you want the workflow to start local model containers, extend the job to run docker-compose and ensure the runner has Docker.

How to set up a self-hosted GPU runner
- Follow GitHub's documentation to register a self-hosted runner on your machine and add the labels 'self-hosted' and 'self-hosted-gpu' to it.
- Make sure Python and required system dependencies are installed on the machine.

Running benchmarks locally
1. Start one or more visual model shims or servers listed in visual_models/models.json.
2. Ensure visual/dataset/initial contains images and *_tree.json files.
3. Run python visual_models/benchmark_runner.py
4. Run python visual_models/metrics.py

