using PL.Call;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainVolunteerWindow.xaml
    /// </summary>
    public partial class MainVolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int volunteerId;
        public BO.Volunteer CurrentVolunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(MainVolunteerWindow));

        public MainVolunteerWindow(int id)
        {
            InitializeComponent();
            volunteerId = id;

            try
            {
                // טוען את פרטי המתנדב לשדה CurrentVolunteer
                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                DataContext = this; // קישור ל-DataContext של החלון
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load volunteer details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close(); // סוגר את החלון במקרה של שגיאה
            }
        }


        // Finish Call Button Click Handler
        private void FinishCall_Click(object sender, RoutedEventArgs e)
        {

        }

        // Cancel Call Button Click Handler
        private void CancelCall_Click(object sender, RoutedEventArgs e)
        {

        }

        // View Call History Button Click Handler
        private void ViewCallHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // טוען את הקריאות הסגורות של המתנדב
                var closedCalls = s_bl.Call.GetClosedCallsByVolunteer(volunteerId, null, null);

                if (closedCalls == null || !closedCalls.Any())
                {
                    MessageBox.Show("No calls found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // פתיחת חלון חדש עם הקריאות
                var callListWindow = new CallListWindow(volunteerId);
                callListWindow.ShowDialog(); // 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Select Call Button Click Handler
        private void SelectCall_Click(object sender, RoutedEventArgs e)
        {

        }

        // Update Volunteer Button Click Handler
        private void UpdateVolunteer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Volunteer.UpdateVolunteer(volunteerId, CurrentVolunteer); // שימוש במזהה ששמרנו
                MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
