using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;

// Simple test document generator: consumes generated/ir/ir.json and emits one Markdown test doc per discovered window.

var root = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
var irPath = Path.Combine(root, "generated", "ir", "ir.json");
if (!File.Exists(irPath))
{
    Console.Error.WriteLine($"IR not found at {irPath}. Run roslyn-ingest first.");
    return 1;
}

var outDir = Path.Combine(root, "generated", "tests");
Directory.CreateDirectory(outDir);

using var fs = File.OpenRead(irPath);
var doc = JsonNode.Parse(fs);
var windows = doc?["windows"]?.AsArray() ?? new JsonArray();

int count = 0;
foreach (var w in windows)
{
    var className = w?["className"]?.ToString() ?? "Unknown";
    var xaml = w?["xaml"]?.ToString() ?? "";
    var events = w?["events"]?.AsArray() ?? new JsonArray();
    var commands = w?["commands"]?.AsArray() ?? new JsonArray();

    var controlNames = events.Select(e => new { attr = e?["attribute"]?.ToString(), val = e?["handler"]?.ToString() }).Where(x => x.attr == "Name").Select(x => x.val).Distinct().ToList();
    var clickHandlers = events.Where(e => e?["attribute"]?.ToString() == "Click").Select(e => new { handler = e?["handler"]?.ToString(), handlerFile = e?["handlerFile"]?.ToString() }).ToList();

    var fileName = Path.Combine(outDir, $"test-{className.Replace('.', '-')}.md");
    using var writer = new StreamWriter(fileName);

    writer.WriteLine($"# Test: {className}");
    writer.WriteLine();
    writer.WriteLine($"Source: {xaml}");
    writer.WriteLine();
    writer.WriteLine("## Purpose");
    writer.WriteLine($"Validate primary interactions on the {className} window (display, inputs, primary commands).");
    writer.WriteLine();
    writer.WriteLine("## Preconditions");
    writer.WriteLine("- Application installed or built locally\n- Test user data seeded if required (see testData)");
    writer.WriteLine();
    writer.WriteLine("## Test data");
    writer.WriteLine("- sampleUserId: testuser");
    writer.WriteLine("- samplePassword: Password123");
    writer.WriteLine();
    writer.WriteLine("## Steps");
    int step = 1;
    writer.WriteLine($"{step++}. Launch the application.");
    writer.WriteLine($"{step++}. Navigate/open the {className} window (if not the startup window) — use application menu or command sequence.");

    if (controlNames.Any())
    {
        foreach (var c in controlNames)
        {
            writer.WriteLine($"{step++}. Enter sample data into control '{c}'.");
        }
    }

    if (clickHandlers.Any())
    {
        foreach (var ch in clickHandlers)
        {
            writer.WriteLine($"{step++}. Click control that triggers handler '{ch.handler}' (defined in: {ch.handlerFile}).");
        }
    }

    writer.WriteLine($"{step++}. Observe application behavior and final state.");
    writer.WriteLine();
    writer.WriteLine("## Expected Results");
    writer.WriteLine("- Window displays without error and all controls visible.");
    writer.WriteLine("- No unhandled exception dialogs appear.");
    writer.WriteLine("- For primary actions, application transitions to the expected screen or updates persisted state as specified in traceability.");
    writer.WriteLine();
    writer.WriteLine("## Traceability");
    writer.WriteLine($"- XAML: {xaml}");
    foreach (var ch in clickHandlers)
    {
        writer.WriteLine($"- Handler: {ch.handler} -> {ch.handlerFile}");
    }

    writer.WriteLine();
    writer.WriteLine("## Notes/Flags");
    writer.WriteLine("- Automatically generated test. Review steps and data for business correctness and add DB seeds if required.");

    count++;
}

Console.WriteLine($"Generated {count} test documents to: {outDir}");
return 0;
