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
using System.Windows.Shapes;
using System.Windows;
using System.Globalization;
using static PL.Helpers;


namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window
    {
        public ICommand DeleteCommand { get; }

        private int _volunteerId;

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Volunteer CurrentVolunteer { get; set; }
        public string ButtonText { get; set; }
        public Array Roles { get; set; }
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

            DeleteCommand = new RelayCommand(DeleteVolunteer, CanDeleteVolunteer);

            _volunteerId = requesting;
            Roles = Enum.GetValues(typeof(BO.Role));
            DataContext = this;
        }

        private void SaveCommand_Execute(object sender, RoutedEventArgs e)
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

                MessageBox.Show("Volunteer saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
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

                MessageBox.Show("Volunteer saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this volunteer?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Volunteer.DeleteVolunteer(0);

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
        public bool IsDeleteButtonVisible
        {
            get
            {
                return _volunteerId != 0;
            }
        }
        private void DeleteVolunteer(object parameter)
        {
            // מקבל את המתנדב דרך CommandParameter
            if (parameter is BO.Volunteer volunteer)
            {
                var result = MessageBox.Show($"Are you sure you want to delete volunteer {volunteer.FullName}?",
                                             "Confirm Delete",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // שימוש ב-ID של המתנדב מתוך האובייקט שהועבר
                        s_bl.Volunteer.DeleteVolunteer(volunteer.Id);

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


        private bool CanDeleteVolunteer(object parameter)
        {
            return parameter is BO.Volunteer volunteer && volunteer.Id != 0;
        }
    }
}