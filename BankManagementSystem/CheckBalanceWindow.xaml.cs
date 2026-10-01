using System;
using System.Linq;
using System.Windows;

namespace BankManagementSystem
{
    public partial class CheckBalanceWindow : Window
    {
        public CheckBalanceWindow()
        {
            InitializeComponent();
        }

        private void BtnCheckBalance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int accNo =
                    Convert.ToInt32(txtAccountNumber.Text);

                using (BankDBEntities db = new BankDBEntities())
                {
                    var account = db.Accounts
                                    .FirstOrDefault(a =>
                                        a.AccountNumber == accNo);

                    if (account != null)
                    {
                        txtName.Text =
                            account.FirstName + " " + account.LastName;

                        decimal balance =
                            account.Balance ?? 0;

                        txtBalance.Text =
                            balance.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Account Not Found");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}