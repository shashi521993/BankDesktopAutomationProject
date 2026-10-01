using System;
using System.Linq;
using System.Windows;

namespace BankManagementSystem
{
    public partial class WithdrawWindow : Window
    {
        public WithdrawWindow()
        {
            InitializeComponent();
        }

        private void BtnWithdraw_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int accNo = Convert.ToInt32(txtAccountNumber.Text);
                decimal amount = Convert.ToDecimal(txtAmount.Text);

                using (BankDBEntities db = new BankDBEntities())
                {
                    var account = db.Accounts
                                    .FirstOrDefault(a => a.AccountNumber == accNo);

                    if (account != null)
                    {
                        decimal currentBalance = account.Balance ?? 0;

                        if (amount > currentBalance)
                        {
                            MessageBox.Show("Insufficient Balance");
                            return;
                        }

                        account.Balance = currentBalance - amount;

                        db.SaveChanges();

                        MessageBox.Show("Amount Withdrawn Successfully");
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