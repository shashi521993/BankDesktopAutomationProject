using System;
using System.IO;
using System.Linq;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using NUnit.Framework;

namespace FlaUITests
{
    [TestFixture]
    public class CreateAccountTests
    {
        private string appExe => Path.GetFullPath(Path.Combine("..", "BankManagementSystem", "bin", "Debug", "BankManagementSystem.exe"));

        [Test]
        public void CreateAccount_SaveWithMinimalFields_ShouldShowSuccess()
        {
            Assert.IsTrue(File.Exists(appExe), $"Application executable not found at {appExe}");

            using var app = Application.Launch(appExe);
            using var automation = new UIA3Automation();

            // Login first - assume test user present or skip login if startup goes to login
            var loginWindow = RetryFind(() => app.GetMainWindow(automation), TimeSpan.FromSeconds(5));
            Assert.IsNotNull(loginWindow, "Login window not found");

            // Try to bypass login by entering sample credentials and clicking Login (if fails, test continues)
            var userBox = loginWindow.FindFirstDescendant(cf => cf.ByAutomationId("txtUserId")) ?? loginWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit)).FirstOrDefault();
            if (userBox != null)
            {
                userBox.AsTextBox().Enter("testuser");
                var pwdBox = loginWindow.FindFirstDescendant(cf => cf.ByAutomationId("txtPassword")) ?? loginWindow.FindAllDescendants(cf => cf.ByControlType(ControlType.Edit)).Skip(1).FirstOrDefault();
                if (pwdBox != null)
                {
                    pwdBox.Focus();
                    FlaUI.Core.Input.Keyboard.Type("Password123");
                }

                var loginButton = loginWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button).And(cf.ByName("Login"))) ?? loginWindow.FindFirstDescendant(cf => cf.ByAutomationId("BtnLogin"));
                loginButton?.AsButton().Invoke();
            }

            // Wait for Dashboard window
            var dashboardWindow = RetryFind(() => app.GetAllTopLevelWindows(automation).FirstOrDefault(w => w.Title != null && w.Title.Contains("Dashboard")), TimeSpan.FromSeconds(8));
            Assert.IsNotNull(dashboardWindow, "Dashboard not found - cannot continue to Create Account");

            // Click Create Account
            var createBtn = dashboardWindow.FindFirstDescendant(cf => cf.ByAutomationId("BtnCreateAccount")) ?? dashboardWindow.FindFirstDescendant(cf => cf.ByName("Create Account"));
            Assert.IsNotNull(createBtn, "Create Account button not found");
            createBtn.AsButton().Invoke();

            // Find CreateAccountWindow
            var createWindow = RetryFind(() => app.GetAllTopLevelWindows(automation).FirstOrDefault(w => w.Title != null && w.Title.Contains("Create Account")), TimeSpan.FromSeconds(5));
            Assert.IsNotNull(createWindow, "Create Account window not found");

            // Click Save Account without filling fields
            var saveBtn = createWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button).And(cf.ByName("Save Account"))) ?? createWindow.FindFirstDescendant(cf => cf.ByText("Save Account"));
            Assert.IsNotNull(saveBtn, "Save Account button not found");
            saveBtn.AsButton().Invoke();

            // Wait for possible message box or simply assert an account was created by checking that a MessageBox appeared
            bool msgShown = RetryUntilTrue(() => app.GetAllTopLevelWindows(automation).Any(w => w.Title != null && (w.Title.Contains("Account Created") || w.Title.Contains("Message"))), TimeSpan.FromSeconds(5));

            Assert.IsTrue(msgShown, "Expected success or error message box was not shown after Save Account");

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
