using System;
using System.Linq;
using System.Windows;

namespace BankManagementSystem
{
    public partial class ModifyAccountWindow : Window
    {
        public ModifyAccountWindow()
        {
            InitializeComponent();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int accNo = Convert.ToInt32(txtAccountNumber.Text);

                using (BankDBEntities db = new BankDBEntities())
                {
                    var account = db.Accounts
                                    .FirstOrDefault(a => a.AccountNumber == accNo);

                    if (account != null)
                    {
                        txtFirstName.Text = account.FirstName;
                        txtMiddleName.Text = account.MiddleName;
                        txtLastName.Text = account.LastName;
                        txtMobile.Text = account.MobileNumber;
                        txtCurrentAddress.Text = account.CurrentAddress;
                        txtPermanentAddress.Text = account.PermanentAddress;
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

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int accNo = Convert.ToInt32(txtAccountNumber.Text);

                using (BankDBEntities db = new BankDBEntities())
                {
                    var account = db.Accounts
                                    .FirstOrDefault(a => a.AccountNumber == accNo);

                    if (account != null)
                    {
                        account.FirstName = txtFirstName.Text;
                        account.MiddleName = txtMiddleName.Text;
                        account.LastName = txtLastName.Text;
                        account.MobileNumber = txtMobile.Text;
                        account.CurrentAddress = txtCurrentAddress.Text;
                        account.PermanentAddress = txtPermanentAddress.Text;

                        db.SaveChanges();

                        MessageBox.Show("Account Updated Successfully");
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