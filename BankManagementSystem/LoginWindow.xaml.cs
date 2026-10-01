using System;
using System.Linq;
using System.Windows;

namespace BankManagementSystem
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            using (BankDBEntities db = new BankDBEntities())
            {
                string userid = txtUserId.Text;
                string password = txtPassword.Password;

                var user = db.Users
                             .FirstOrDefault(u =>
                                 u.UserId == userid &&
                                 u.Password == password);

                if (user != null)
                {
                    MessageBox.Show("Login Successful");

                    DashboardWindow dashboard = new DashboardWindow();
                    dashboard.Show();

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid User ID or Password");
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            txtUserId.Clear();
            txtPassword.Clear();
        }
    }
}