using BO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;


namespace PL.Volunteer
{
    public partial class VolunteerListWindow : Window
    {
        int tampUserId;
        public string ButtonText { get; set; }

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get => (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty);
            set => SetValue(VolunteerListProperty, value);
        }

        public static readonly DependencyProperty SelectedCallTypeProperty =
            DependencyProperty.Register("SelectedCallType", typeof(BO.CallType), typeof(VolunteerListWindow),
            new PropertyMetadata(BO.CallType.None, OnSelectedCallTypeChanged));

        public BO.CallType SelectedCallType
        {
            get => (BO.CallType)GetValue(SelectedCallTypeProperty);
            set => SetValue(SelectedCallTypeProperty, value);
        }

        private static void OnSelectedCallTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as VolunteerListWindow;
            window?.queryVolunteerList();
        }

        public VolunteerListWindow(int id)
        {
            InitializeComponent();
            this.DataContext = this;
            tampUserId = id;
            Loaded += Window_Loaded;
            Closed += Window_Closed;
        }

        private void queryVolunteerList()
        {
            VolunteerList = SelectedCallType == BO.CallType.None
                ? s_bl?.Volunteer.GetVolunteers(null, VolunteerFieldVolunteerInList.CurrentCallType, null)
                : s_bl?.Volunteer.GetVolunteers(null, VolunteerFieldVolunteerInList.CurrentCallType, SelectedCallType);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(queryVolunteerList);
            queryVolunteerList();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(queryVolunteerList);
        }


        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is BO.VolunteerInList selectedVolunteerInList)
            {
                try
                {
                    // קבלת המתנדב המלא לפי ID
                    var selectedVolunteer = s_bl.Volunteer.GetVolunteerDetails(selectedVolunteerInList.Id);
           
                    // פתיחת חלון עם הנתונים של המתנדב
                    var volunteerWindow = new VolunteerWindow(tampUserId, selectedVolunteer.Id);
                    volunteerWindow.Show(); 

                    // רענון הרשימה לאחר סגירת החלון
                    queryVolunteerList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error fetching volunteer details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the VolunteerWindow in add mode (ID = 0)
            var volunteerWindow = new VolunteerWindow(0);
      
            volunteerWindow.Show();

            // Refresh the list after closing the window
            RefreshVolunteerList();
        }

        private void RefreshVolunteerList()
        {
            try
            {
                // Load the updated list of volunteers
                VolunteerList = BlApi.Factory.Get().Volunteer.GetVolunteers(null, VolunteerFieldVolunteerInList.CurrentCallType, SelectedCallType);
                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing volunteer list: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
