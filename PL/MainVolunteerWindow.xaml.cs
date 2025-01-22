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

        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(MainVolunteerWindow));

        public BO.CallInProgress? CurrentCall
        {
            get => (BO.CallInProgress?)GetValue(CurrentCallProperty);
            set => SetValue(CurrentCallProperty, value);
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register(nameof(CurrentCall), typeof(BO.CallInProgress), typeof(MainVolunteerWindow));

        public MainVolunteerWindow(int id)
        {
            try
            {
                InitializeComponent();
                CurrentVolunteer = id != 0 ? s_bl.Volunteer.GetVolunteerDetails(id) : new BO.Volunteer();
                SetValue(CurrentCallProperty, s_bl.Volunteer.GetVolunteerDetails(id)?.CurrentCall);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load volunteer details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CurrentVolunteer = new BO.Volunteer();
                SetValue(CurrentCallProperty, null);
                DataContext = this;
            }
        }



        private void FinishCall_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer.CurrentCall == null)
            {
                MessageBox.Show("No call to finish.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                s_bl.Call.CompleteCallAssignment(CurrentVolunteer.Id, CurrentCall.Id);
                MessageBox.Show("Call finished successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id);
                CurrentCall = CurrentVolunteer.CurrentCall;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to finish call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelCall_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer.CurrentCall == null)
            {
                MessageBox.Show("No call to cancel.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                s_bl.Call.CancelCallAssignment(CurrentVolunteer.Id, CurrentCall.Id);
                MessageBox.Show("Call canceled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id);
                CurrentCall = CurrentVolunteer.CurrentCall;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to cancel call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void UpdateVolunteer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer);
                MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangeActivityStatus_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer.CurrentCall != null)
            {
                MessageBox.Show("Cannot change activity status while a call is in progress.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                CurrentVolunteer.IsActive = !CurrentVolunteer.IsActive;
                s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer);
                MessageBox.Show("Activity status updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating activity status: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectCall_Click(object sender, RoutedEventArgs e)
        {
        if (CurrentVolunteer.CurrentCall != null)
            {
                MessageBox.Show("Cannot select a new call while a call is in progress.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            SelectCallWindow selectCallWindow = new SelectCallWindow(CurrentVolunteer.Id);
            selectCallWindow.Show();
        }

        private void ViewCallHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var closedCalls = s_bl.Call.GetClosedCallsByVolunteer(CurrentVolunteer.Id, null, null);

                if (closedCalls == null || !closedCalls.Any())
                {
                    MessageBox.Show("No calls found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var callListWindow = new CallListWindow(closedCalls);
                callListWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void RefreshData()
        {
            try
            {
                if (CurrentVolunteer == null || CurrentVolunteer.Id == 0)
                {
                    MessageBox.Show("Volunteer ID is missing. Unable to refresh data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id);
                CurrentCall = CurrentVolunteer.CurrentCall;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to refresh data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
