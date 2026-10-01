using System;
using System.Linq;
using System.Windows;

namespace BankManagementSystem
{
    public partial class DepositWindow : Window
    {
        public DepositWindow()
        {
            InitializeComponent();
        }

        private void BtnDeposit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int accNo =
                    Convert.ToInt32(txtAccountNumber.Text);

                decimal amount =
                    Convert.ToDecimal(txtAmount.Text);

                using (BankDBEntities db = new BankDBEntities())
                {
                    var account = db.Accounts
                                    .FirstOrDefault(a =>
                                        a.AccountNumber == accNo);

                    if (account != null)
                    {
                        // IF BALANCE IS NULL MAKE IT ZERO
                        if (account.Balance == null)
                        {
                            account.Balance = 0;
                        }

                        // ADD DEPOSIT AMOUNT
                        account.Balance =
                            account.Balance + amount;

                        db.SaveChanges();

                        MessageBox.Show(
                            "Amount Deposited Successfully");

                        txtAccountNumber.Clear();
                        txtAmount.Clear();
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