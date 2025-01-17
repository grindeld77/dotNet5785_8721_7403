using PL.Volunteer;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PL
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public LoginWindow()
        {
            InitializeComponent();
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string userId = UserIdTextBox.Text; // Get User ID from TextBox
                int id = int.TryParse(userId, out id) ? id : 0;
                string password = PasswordBox.Password; // Get Password from PasswordBox

                string role = s_bl.Volunteer.Login(id, password);

                // Check if login is for Admin or Volunteer
                if (AdminLoginCheckBox.IsChecked == true) // Admin login
                {
                    if (role == "Admin")
                    {
                        // Navigate to AdminWindow
                        new MainWindow(id).Show();
                    }
                    else
                    {
                        throw new Exception("This user is not an administrator..");
                    }
                }
                else // Volunteer login
                {
                    new MainVolunteerWindow(id).Show();
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Text = PasswordBox.Password;
            PasswordBox.Visibility = Visibility.Collapsed;
            PasswordTextBox.Visibility = Visibility.Visible;
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = PasswordTextBox.Text;
            PasswordTextBox.Visibility = Visibility.Collapsed;
            PasswordBox.Visibility = Visibility.Visible;
        }
    }
}
