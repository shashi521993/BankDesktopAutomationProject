using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;

// Simple runner to drive the app and capture screenshots + accessibility tree for Login and CreateAccount flows
var repoRoot = args.Length>0 ? args[0] : Directory.GetCurrentDirectory();
var appExe = Path.GetFullPath(Path.Combine(repoRoot, "BankManagementSystem", "bin", "Debug", "BankManagementSystem.exe"));
if (!File.Exists(appExe))
{
    Console.Error.WriteLine($"App exe not found at {appExe}. Build the project first.");
    return 1;
}

var outDir = Path.Combine(repoRoot, "visual", "dataset", "initial");
Directory.CreateDirectory(outDir);

var connectionString = "data source=(localdb)\\MSSQLLocalDB;initial catalog=BankDB;integrated security=True;trustservercertificate=True;MultipleActiveResultSets=True;";
// ensure database and Users table exist; if not, create them
try
{
    using (var conn = new SqlConnection(connectionString))
    {
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = 'testuser') INSERT INTO Users (UserId, Password) VALUES ('testuser', 'Password123')";
        cmd.ExecuteNonQuery();
    }
}
catch (SqlException ex) // handle missing DB or missing table
{
    Console.WriteLine("BankDB not present. Creating database and Users table...");
    var masterConnStr = "data source=(localdb)\\MSSQLLocalDB;initial catalog=master;integrated security=True;trustservercertificate=True;";
    using (var conn = new SqlConnection(masterConnStr))
    {
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "IF DB_ID('BankDB') IS NULL CREATE DATABASE BankDB";
        cmd.ExecuteNonQuery();
    }
    // wait a moment for DB to appear
    Thread.Sleep(800);
    // create Users table with retries
    var created = false;
    for (int i = 0; i < 5 && !created; i++)
    {
        try
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"IF OBJECT_ID('dbo.Users') IS NULL BEGIN CREATE TABLE dbo.Users (Id INT IDENTITY(1,1) PRIMARY KEY, UserId NVARCHAR(200), Password NVARCHAR(200)); END";
                cmd.ExecuteNonQuery();
                using var ins = conn.CreateCommand();
                ins.CommandText = "IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = 'testuser') INSERT INTO Users (UserId, Password) VALUES ('testuser', 'Password123')";
                ins.ExecuteNonQuery();
                created = true;
            }
        }
        catch (SqlException)
        {
            Thread.Sleep(500);
        }
    }
}

using var app = Application.Launch(appExe);
using var automation = new UIA3Automation();
Thread.Sleep(800);
var loginWindow = app.GetAllTopLevelWindows(automation).FirstOrDefault(w => w.Title!=null && w.Title.Contains("Bank Login")) ?? app.GetMainWindow(automation);
if (loginWindow==null) loginWindow = app.GetMainWindow(automation);

Console.WriteLine("Capturing login screen...");
var loginShot = Path.Combine(outDir, "login.png");
loginWindow.CaptureToFile(loginShot);
var loginTree = DumpAutomationTree(loginWindow);
File.WriteAllText(Path.Combine(outDir, "login_tree.json"), JsonSerializer.Serialize(loginTree, new JsonSerializerOptions{WriteIndented=true}));

// enter credentials
var userBox = loginWindow.FindFirstDescendant(cf=>cf.ByAutomationId("txtUserId")) ?? loginWindow.FindAllDescendants(cf=>cf.ByControlType(ControlType.Edit)).FirstOrDefault();
userBox?.AsTextBox().Enter("testuser");
var pwdBox = loginWindow.FindFirstDescendant(cf=>cf.ByAutomationId("txtPassword")) ?? loginWindow.FindAllDescendants(cf=>cf.ByControlType(ControlType.Edit)).Skip(1).FirstOrDefault();
if (pwdBox!=null){pwdBox.Focus(); FlaUI.Core.Input.Keyboard.Type("Password123");}

var loginBtn = loginWindow.FindFirstDescendant(cf=>cf.ByControlType(ControlType.Button).And(cf.ByName("Login"))) ?? loginWindow.FindFirstDescendant(cf=>cf.ByAutomationId("BtnLogin"));
loginBtn?.AsButton().Invoke();
Thread.Sleep(1000);

var dashboard = app.GetAllTopLevelWindows(automation).FirstOrDefault(w=>w.Title!=null && w.Title.Contains("Dashboard"));
if (dashboard!=null)
{
    var dashShot = Path.Combine(outDir, "dashboard.png");
    dashboard.CaptureToFile(dashShot);
    var dashTree = DumpAutomationTree(dashboard);
    File.WriteAllText(Path.Combine(outDir, "dashboard_tree.json"), JsonSerializer.Serialize(dashTree, new JsonSerializerOptions{WriteIndented=true}));

    // click Create Account
    var createBtn = dashboard.FindFirstDescendant(cf=>cf.ByAutomationId("BtnCreateAccount")) ?? dashboard.FindFirstDescendant(cf=>cf.ByName("Create Account"));
    createBtn?.AsButton().Invoke();
    Thread.Sleep(800);
    var createWin = app.GetAllTopLevelWindows(automation).FirstOrDefault(w=>w.Title!=null && w.Title.Contains("Create Account"));
    if (createWin!=null)
    {
        createWin.CaptureToFile(Path.Combine(outDir, "createaccount.png"));
        File.WriteAllText(Path.Combine(outDir, "createaccount_tree.json"), JsonSerializer.Serialize(DumpAutomationTree(createWin), new JsonSerializerOptions{WriteIndented=true}));
    }
}

app.Close();
Console.WriteLine($"Captured visuals to {outDir}");
return 0;

object DumpAutomationTree(Window w)
{
    object Recurse(AutomationElement e)
    {
        var children = e.FindAllChildren();
        string name = null;
        string automationId = null;
        string controlType = null;
        string bounding = null;
        try { name = e.Name; } catch { name = null; }
        try { automationId = e.AutomationId; } catch { automationId = null; }
        try { controlType = e.ControlType.ToString(); } catch { controlType = null; }
        try { bounding = e.BoundingRectangle.ToString(); } catch { bounding = null; }
        return new {
            Name = name,
            AutomationId = automationId,
            ControlType = controlType,
            BoundingRectangle = bounding,
            Children = children.Select(c => Recurse(c)).ToList()
        };
    }
    return Recurse(w);
}
