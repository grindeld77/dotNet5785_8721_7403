using System;
using System.Windows;
using static PL.Helpers;

namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow), new PropertyMetadata(""));

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        public BO.Volunteer CurrentVolunteer
        {
            get => (BO.Volunteer)GetValue(CurrentVolunteerProperty);
            set => SetValue(CurrentVolunteerProperty, value);
        }

        public static readonly DependencyProperty RolesProperty =
            DependencyProperty.Register("Roles", typeof(Array), typeof(VolunteerWindow), new PropertyMetadata(null));

        public Array Roles
        {
            get => (Array)GetValue(RolesProperty);
            set => SetValue(RolesProperty, value);
        }

        private int _volunteerId;
        public bool IsAddMode { get; set; }

        public VolunteerWindow(int requesting, int id = 0)
        {
            InitializeComponent();

            if (id == 0)
            {
                CurrentVolunteer = new BO.Volunteer();
                ButtonText = "Add";
                IsAddMode = true;
            }
            else
            {
                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                ButtonText = "Update";
                IsAddMode = false;
            }

            _volunteerId = requesting;

           Roles = Enum.GetValues(typeof(BO.Role));

        }

        private void volunteerObserver()
        {
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer!.Id);
        }

        private void UpdateVolunteersList()
        {
            if (CurrentVolunteer != null)
            {
                var updatedVolunteer = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id);

                CurrentVolunteer = updatedVolunteer;
            }
        }

        //Levi, 
        // this line must be added to the constructor of the window
        // Loaded="Window_Loaded" Closed="Window_Closed"
        // fix the error by adding the following code to the window class but there is a problem 
        // with the observer pattern in the BL layer

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (CurrentVolunteer != null && CurrentVolunteer.Id > 0)
        //        {
        //            s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, volunteerObserver);
        //            s_bl.Volunteer.AddObserver(UpdateVolunteersList);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error during Window_Loaded: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private void Window_Closed(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (CurrentVolunteer != null && CurrentVolunteer.Id > 0)
        //        {
        //            s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, volunteerObserver);
        //        }
        //        s_bl.Volunteer.RemoveObserver(UpdateVolunteersList);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error during Window_Closed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
                }
                else
                {
                    s_bl.Volunteer.UpdateVolunteer(_volunteerId, CurrentVolunteer);
                }
                volunteerObserver();
                MessageBox.Show("Volunteer saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Are you sure you want to delete volunteer {CurrentVolunteer.FullName}?",
                                         "Confirm Delete",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Volunteer.DeleteVolunteer(CurrentVolunteer.Id);

                    MessageBox.Show("Volunteer deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Deletion canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
