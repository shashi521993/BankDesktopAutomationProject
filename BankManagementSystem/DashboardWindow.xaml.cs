using System.Windows;

namespace BankManagementSystem
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
        }

        // CREATE ACCOUNT

        private void BtnCreateAccount_Click(object sender, RoutedEventArgs e)
        {
            CreateAccountWindow create =
                new CreateAccountWindow();

            create.ShowDialog();
        }

        // DEPOSIT

        private void BtnDeposit_Click(object sender, RoutedEventArgs e)
        {
            DepositWindow deposit =
                new DepositWindow();

            deposit.ShowDialog();
        }

        // WITHDRAW

        private void BtnWithdraw_Click(object sender, RoutedEventArgs e)
        {
            WithdrawWindow withdraw =
                new WithdrawWindow();

            withdraw.ShowDialog();
        }

        // CHECK BALANCE

        private void BtnCheckBalance_Click(object sender, RoutedEventArgs e)
        {
            CheckBalanceWindow check =
                new CheckBalanceWindow();

            check.ShowDialog();
        }

        // MODIFY ACCOUNT

        private void BtnModifyAccount_Click(object sender, RoutedEventArgs e)
        {
            ModifyAccountWindow modify =
                new ModifyAccountWindow();

            modify.ShowDialog();
        }

        // VIEW ACCOUNTS

        private void BtnViewAccounts_Click(object sender, RoutedEventArgs e)
        {
            ViewAccountsWindow view =
                new ViewAccountsWindow();

            view.ShowDialog();
        }

        // LOGOUT

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login =
                new LoginWindow();

            login.Show();

            this.Close();
        }
    }
}