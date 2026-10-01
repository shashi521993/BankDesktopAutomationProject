roslyn-ingest (lightweight)

This lightweight ingestion tool scans a repository for XAML files and C# source to produce a simple IR JSON describing Window classes, XAML-declared events and command bindings, and any detected DbContext classes. It is intentionally simple (text-based parsing) to be runnable without complex Roslyn restore; it is a first-step implementation for the Week 1 pipeline.

Usage:
  dotnet run --project tools/roslyn-ingest/roslyn-ingest.csproj -- "C:\path\to\repo"

Output:
  generated/ir/ir.json

Note: This is an initial implementation that will be extended with Roslyn-based semantic analysis in later steps.
