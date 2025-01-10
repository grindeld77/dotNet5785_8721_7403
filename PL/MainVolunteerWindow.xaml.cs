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
                CurrentCall = s_bl.Volunteer.GetVolunteerDetails(id).CurrentCall;
                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load volunteer details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    // Add observers
        //    //s_bl.Volunteer.AddObserver(CallObserver);
        //    s_bl.Volunteer.AddObserver(VolunteerObserver);
        //}

        //private void Window_Closed(object sender, EventArgs e)
        //{
        //    // Remove observers
        //    //s_bl.Volunteer?.RemoveObserver(CallObserver);
        //    s_bl.Volunteer?.RemoveObserver(VolunteerObserver);
        //    Close();
        //}

        //private void VolunteerObserver()
        //{
        //    try
        //    {
        //        CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Failed to update volunteer details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private void CallObserver() //todo
        //{
        //    try
        //    {
        //        // Assuming GetCallDetails returns a collection of calls
        //        var callsInProgress = s_bl.Call.GetCallDetails(volunteerId);

        //        // Check if callsInProgress is a valid, non-empty collection
        //        if (callsInProgress != null && callsInProgress.Any())
        //        {
        //            var activeCall = callsInProgress.FirstOrDefault(call => call.Status == BO.CallStatus.Open || call.Status == BO.CallStatus.OpenAtRisk);
        //            CurrentCall = activeCall;
        //        }
        //        else
        //        {
        //            MessageBox.Show("No calls in progress.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Failed to update call details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

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
                s_bl.Call.CancelCallAssignment(CurrentVolunteer.Id , CurrentCall.Id);
                MessageBox.Show("Call canceled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show("You already have a call in progress.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        SelectCallWindow selectCallWindow = new SelectCallWindow(CurrentVolunteer.Id);
        selectCallWindow.Show();
        }

        private void ViewCallHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // טוען את הקריאות הסגורות של המתנדב
                var closedCalls = s_bl.Call.GetClosedCallsByVolunteer(CurrentVolunteer.Id, null, null);

                if (closedCalls == null || !closedCalls.Any())
                {
                    MessageBox.Show("No calls found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // פתיחת חלון חדש עם הקריאות
                var callListWindow = new CallListWindow(CurrentVolunteer.Id);
                callListWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
