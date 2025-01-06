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
    public partial class LoginWindow : Page
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public LoginWindow()
        {
            InitializeComponent();
        }

        //private void LoginButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        string userId = UserIdTextBox.Text; // Get User ID from TextBox
        //        int id = int.TryParse(userId, out id) ? id : 0;
        //        string password = PasswordBox.Password; // Get Password from PasswordBox

        //        // Validation Logic
        //        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(password))
        //        {
        //            ErrorMessageTextBlock.Text = "Please fill in all fields.";
        //            return;
        //        }
        //        if (id < 200000000 || id > 400000000)
        //        {
        //            ErrorMessageTextBlock.Text = "User ID must be a valid number.";
        //            ErrorMessageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
        //            return;
        //        }

        //        // Try logging in and handling different user types
        //        string role = s_bl.Volunteer.Login(id, password);
        //        if (role == "Admin")
        //        {
        //            // Navigate to AdminWindow
        //            new MainWindow(id).Show();
        //        }
        //        else if (role == "Volunteer")
        //        {
        //            // Navigate to VolunteerWindow
        //            new MainVolunteerWindow(id).Show();
        //        }
        //        else
        //        {
        //            ErrorMessageTextBlock.Text = "Invalid User ID or Password.";  // Error message for invalid credentials
        //            ErrorMessageTextBlock.Foreground = new SolidColorBrush(Colors.Red);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle unexpected errors
        //        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}


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
            PasswordBox.Visibility = Visibility.Collapsed;
            PasswordTextBox.Visibility = Visibility.Visible;
            PasswordTextBox.Text = PasswordBox.Password;
        }

        // Function when checkbox is unchecked
        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordBox.Visibility = Visibility.Visible;
            PasswordTextBox.Visibility = Visibility.Collapsed;
            PasswordBox.Password = PasswordTextBox.Text;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
