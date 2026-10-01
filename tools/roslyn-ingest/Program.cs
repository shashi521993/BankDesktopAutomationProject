using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

// Simple IR producer: scans for .xaml and .cs files to detect Window classes, XAML-declared events and Commands,
// resolves event handlers to code-behind files when possible, and detects ICommand usages.

var root = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
Console.WriteLine($"roslyn-ingest scanning: {root}");

var xamlFiles = Directory.GetFiles(root, "*.xaml", SearchOption.AllDirectories);
var csFiles = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories);

var classRegex = new Regex(@"x:Class=""(?<cls>[^""]+)""", RegexOptions.Compiled);
var eventAttrRegex = new Regex(@"(?<attr>\w+)\s*=\s*""(?<handler>[A-Za-z0-9_]+)""", RegexOptions.Compiled);
var commandBindingRegex = new Regex(@"Command=\{Binding\s+(?<cmd>[A-Za-z0-9_\.]+)\}", RegexOptions.Compiled);
var windowClassRegex = new Regex(@"class\s+(?<name>[A-Za-z0-9_]+)\s*:\s*(?<base>[A-Za-z0-9_<> ,]+)", RegexOptions.Compiled);
var methodRegex = new Regex(@"(?:public|private|protected|internal)\s+[A-Za-z0-9_<>]+\s+(?<name>[A-Za-z0-9_]+)\s*\(", RegexOptions.Compiled);
var dbRegex = new Regex(@"class\s+(?<name>[A-Za-z0-9_]+)\s*:\s*DbContext", RegexOptions.Compiled);

// Read cs texts
var csTextMap = csFiles.ToDictionary(f => f, f => File.ReadAllText(f));

var classes = new List<object>();
foreach (var cs in csFiles)
{
    try
    {
        var text = csTextMap[cs];
        foreach (Match cm in windowClassRegex.Matches(text))
        {
            var name = cm.Groups["name"].Value;
            var bas = cm.Groups["base"].Value;
            if (bas.Contains("Window") || bas.Contains("UserControl"))
            {
                var methods = methodRegex.Matches(text).Cast<Match>().Select(m => m.Groups["name"].Value).Distinct().ToList();
                classes.Add(new { file = Path.GetRelativePath(root, cs).Replace("\\","/"), name, baseClass = bas.Trim(), methods });
            }
        }
    }
    catch { }
}

var dbContexts = new List<object>();
foreach (var cs in csFiles)
{
    var text = csTextMap[cs];
    foreach (Match m in dbRegex.Matches(text))
    {
        dbContexts.Add(new { file = Path.GetRelativePath(root, cs).Replace("\\","/"), name = m.Groups["name"].Value });
    }
}

var enrichedWindows = new List<object>();
foreach (var xaml in xamlFiles)
{
    try
    {
        var text = File.ReadAllText(xaml);
        var m = classRegex.Match(text);
        if (!m.Success) continue;
        var className = m.Groups["cls"].Value;
        var eventsList = new List<object>();

        foreach (Match em in eventAttrRegex.Matches(text))
        {
            var attr = em.Groups["attr"].Value;
            var handler = em.Groups["handler"].Value;
            string handlerFile = null;
            var shortClass = className.Contains('.') ? className.Split('.').Last() : className;
            var candidate = classes.Cast<dynamic?>().FirstOrDefault(c => c != null && ((string)c.name) == shortClass);
            if (candidate != null)
            {
                var methodsList = ((IEnumerable<string>)candidate.methods).ToList();
                if (methodsList.Contains(handler)) handlerFile = (string)candidate.file;
            }

            if (handlerFile == null)
            {
                foreach (var kv in csTextMap)
                {
                    if (Regex.IsMatch(kv.Value, $"\\b{Regex.Escape(handler)}\\s*\\("))
                    {
                        handlerFile = Path.GetRelativePath(root, kv.Key).Replace("\\","/");
                        break;
                    }
                }
            }

            eventsList.Add(new { attribute = attr, handler = handler, handlerFile });
        }

        var commandList = commandBindingRegex.Matches(text).Cast<Match>().Select(cm => cm.Groups["cmd"].Value).Distinct().Select(c => new { binding = c }).ToList();

        // attempt to resolve command bindings to ICommand property implementations
        var resolvedCommands = new List<object>();
        foreach (var cmd in commandList)
        {
            var binding = (string)cmd.binding;
            var propName = binding.Contains('.') ? binding.Split('.').Last() : binding;
            string implFile = null;
            var propRegexes = new[] {
                new Regex($"ICommand\\s+{Regex.Escape(propName)}\\b", RegexOptions.Compiled),
                new Regex($"public\\s+[A-Za-z0-9_<>]+\\s+{Regex.Escape(propName)}\\s*\\{{", RegexOptions.Compiled),
                new Regex($"\\b{Regex.Escape(propName)}\\b\\s*\\=\\s*new\\s+RelayCommand", RegexOptions.Compiled)
            };

            foreach (var kv in csTextMap)
            {
                var txt = kv.Value;
                if (propRegexes.Any(rx => rx.IsMatch(txt)))
                {
                    implFile = Path.GetRelativePath(root, kv.Key).Replace("\\","/");
                    break;
                }
            }

            resolvedCommands.Add(new { binding = binding, implementationFile = implFile });
        }

        enrichedWindows.Add(new { xaml = Path.GetRelativePath(root, xaml).Replace("\\","/"), className, events = eventsList, commands = resolvedCommands });
    }
    catch { }
}

var iCommandUsages = new List<object>();
var iCommandRegex = new Regex(@"ICommand", RegexOptions.Compiled);
foreach (var kv in csTextMap)
{
    if (iCommandRegex.IsMatch(kv.Value)) iCommandUsages.Add(new { file = Path.GetRelativePath(root, kv.Key).Replace("\\","/") });
}

var ir = new { schemaVersion = "1.0", generatedAt = DateTime.UtcNow, repositoryRoot = root, windows = enrichedWindows, classes, dbContexts, iCommandUsages };

var outDir = Path.Combine(root, "generated", "ir"); Directory.CreateDirectory(outDir);
var serialized = JsonSerializer.Serialize(ir, new JsonSerializerOptions { WriteIndented = true });
using var sha1 = SHA1.Create(); var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(serialized)); var hashHex = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
var outPathHash = Path.Combine(outDir, $"ir-{hashHex}.json"); File.WriteAllText(outPathHash, serialized, Encoding.UTF8);
var outPath = Path.Combine(outDir, "ir.json"); File.WriteAllText(outPath, serialized, Encoding.UTF8);
Console.WriteLine($"IR written to: {outPath}"); Console.WriteLine($"IR (hashed) written to: {outPathHash}");

return 0;
