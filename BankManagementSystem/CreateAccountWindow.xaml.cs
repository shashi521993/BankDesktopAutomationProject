using System;
using System.Windows;
using System.Windows.Controls;

namespace BankManagementSystem
{
    public partial class CreateAccountWindow : Window
    {
        public CreateAccountWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (BankDBEntities db = new BankDBEntities())
                {
                    Account account = new Account();

                    // ACCOUNT TYPE
                    account.AccountType =
                        (cmbAccountType.SelectedItem as ComboBoxItem)?
                        .Content.ToString();

                    // NAME
                    account.FirstName = txtFirstName.Text;
                    account.MiddleName = txtMiddleName.Text;
                    account.LastName = txtLastName.Text;

                    // DOB
                    account.DateOfBirth = dpDOB.SelectedDate;

                    // GENDER
                    account.Gender =
                        (cmbGender.SelectedItem as ComboBoxItem)?
                        .Content.ToString();

                    // OTHER DETAILS
                    account.PanNumber = txtPan.Text;
                    account.CurrentAddress = txtCurrentAddress.Text;
                    account.PermanentAddress = txtPermanentAddress.Text;
                    account.MobileNumber = txtMobile.Text;

                    // DEFAULT BALANCE
                    account.Balance = 0;

                    db.Accounts.Add(account);

                    db.SaveChanges();

                    MessageBox.Show("Account Created Successfully");

                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            cmbAccountType.SelectedIndex = -1;

            txtFirstName.Clear();
            txtMiddleName.Clear();
            txtLastName.Clear();

            dpDOB.SelectedDate = null;

            cmbGender.SelectedIndex = -1;

            txtPan.Clear();
            txtCurrentAddress.Clear();
            txtPermanentAddress.Clear();
            txtMobile.Clear();
        }
    }
}