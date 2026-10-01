using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BankManagementSystem
{
    public partial class ViewAccountsWindow : Window
    {
        public ViewAccountsWindow()
        {
            InitializeComponent();

            LoadAccounts();
        }

        private void LoadAccounts()
        {
            using (BankDBEntities db = new BankDBEntities())
            {
                dgAccounts.ItemsSource =
                    db.Accounts.ToList();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            int accNo = (int)btn.Tag;

            MessageBoxResult result =
                MessageBox.Show(
                    "Are you sure you want to delete this account?",
                    "Delete Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (BankDBEntities db = new BankDBEntities())
                {
                    var account = db.Accounts
                                    .FirstOrDefault(a =>
                                        a.AccountNumber == accNo);

                    if (account != null)
                    {
                        db.Accounts.Remove(account);

                        db.SaveChanges();

                        MessageBox.Show("Account Deleted Successfully");

                        LoadAccounts();
                    }
                }
            }
        }
    }
}