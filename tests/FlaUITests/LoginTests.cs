using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using NUnit.Framework;

namespace FlaUITests
{
    [TestFixture]
    public class LoginTests
    {
        private string appExe => Path.GetFullPath(Path.Combine("..", "BankManagementSystem", "bin", "Debug", "BankManagementSystem.exe"));
        private string connectionString = "data source=(localdb)\\MSSQLLocalDB;initial catalog=BankDB;integrated security=True;trustservercertificate=True;MultipleActiveResultSets=True;";

        [SetUp]
        public void Setup()
        {
            // ensure test user exists
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = 'testuser') INSERT INTO Users (UserId, Password) VALUES ('testuser', 'Password123')";
            cmd.ExecuteNonQuery();
        }

        [TearDown]
        public void TearDown()
        {
            // cleanup test user
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Users WHERE UserId = 'testuser'";
            cmd.ExecuteNonQuery();
        }

        [Test]
        public void Login_Success_ShouldOpenDashboard()
        {
            Assert.IsTrue(File.Exists(appExe), $"Application executable not found at {appExe}");

            using var app = Application.Launch(appExe);
            using var automation = new UIA3Automation();

            var loginWindow = RetryFind(() => app.GetMainWindow(automation), TimeSpan.FromSeconds(5));
            Assert.IsNotNull(loginWindow, "Login window not found");

            // Find user id textbox
            var userBox = loginWindow.FindFirstDescendant(cf => cf.ByAutomationId("txtUserId"))
                          ?? loginWindow.FindFirstDescendant(cf => cf.ByName("txtUserId"))
                          ?? loginWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit)).FirstOrDefault();
            Assert.IsNotNull(userBox, "UserId textbox not found");
            userBox.AsTextBox().Enter("testuser");

            // Find password box
            var pwdBox = loginWindow.FindFirstDescendant(cf => cf.ByAutomationId("txtPassword"))
                         ?? loginWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit).And(cf.ByName("txtPassword")))
                         ?? loginWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit)).Skip(1).FirstOrDefault();
            Assert.IsNotNull(pwdBox, "Password control not found");
            // For PasswordBox we send keyboard input
            pwdBox.Focus();
            FlaUI.Core.Input.Keyboard.Type("Password123");

            // Click Login button (by text)
            var loginButton = loginWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button).And(cf.ByName("Login")))
                              ?? loginWindow.FindFirstDescendant(cf => cf.ByAutomationId("BtnLogin"))
                              ?? loginWindow.FindFirstDescendant(cf => cf.ByText("Login"));
            Assert.IsNotNull(loginButton, "Login button not found");
            loginButton.AsButton().Invoke();

            // wait for dashboard window to appear
            bool dashboardFound = RetryUntilTrue(() =>
            {
                var allWindows = app.GetAllTopLevelWindows(automation);
                return allWindows.Any(w => w.Title != null && w.Title.Contains("Dashboard"));
            }, TimeSpan.FromSeconds(8));

            Assert.IsTrue(dashboardFound, "Dashboard window did not appear after login");

            app.Close();
        }

        private T RetryFind<T>(Func<T> fn, TimeSpan timeout) where T : class
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.Elapsed < timeout)
            {
                try
                {
                    var r = fn();
                    if (r != null) return r;
                }
                catch { }
                Thread.Sleep(200);
            }
            return null;
        }

        private bool RetryUntilTrue(Func<bool> fn, TimeSpan timeout)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.Elapsed < timeout)
            {
                try
                {
                    if (fn()) return true;
                }
                catch { }
                Thread.Sleep(250);
            }
            return false;
        }
    }
}
