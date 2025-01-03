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
        /*
         public VolunteerWindow(int id = 0)
{
    InitializeComponent();

    // אם ID שווה ל-0, מדובר במצב הוספה. אחרת, מצב עדכון.
    CurrentVolunteer = id == 0 ? new BO.Volunteer() : s_bl.Volunteer.GetVolunteerById(id);
    ButtonText = id == 0 ? "Add" : "Update";
    Roles = Enum.GetValues(typeof(BO.Role));

    // חיבור ה-DataContext עבור Binding
    DataContext = this;
}

         */
        public VolunteerListWindow()
        {
            InitializeComponent();
            this.DataContext = this;
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
                    var volunteerWindow = new VolunteerWindow(selectedVolunteer.Id);
                    volunteerWindow.ShowDialog();

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
      
            volunteerWindow.ShowDialog();

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

//using System;
//using System.Collections.Generic;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;

//namespace PL.Volunteer
//{
//    public partial class VolunteerListWindow : Window
//    {
//        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

//        public IEnumerable<BO.VolunteerInList> VolunteerList { get; set; }

//        public VolunteerListWindow()
//        {
//            InitializeComponent();
//            DataContext = this;
//            Loaded += Window_Loaded;
//            Closed += Window_Closed;
//        }

//        private void Window_Loaded(object sender, RoutedEventArgs e)
//        {
//            RefreshVolunteerList();
//        }

//        private void Window_Closed(object sender, EventArgs e)
//        {
//            // Optional: Handle cleanup if needed
//        }

//        private void RefreshVolunteerList()
//        {
//            try
//            {
//                VolunteerList = s_bl.Volunteer.GetVolunteers(null, BO.VolunteerFieldVolunteerInList.CurrentCallType, null);
//                DataContext = this;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error refreshing volunteer list: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
//        {
//            if (sender is ListView listView && listView.SelectedItem is BO.VolunteerInList selectedVolunteerInList)
//            {
//                try
//                {
//                    var volunteerWindow = new VolunteerWindow(selectedVolunteerInList.Id);
//                    volunteerWindow.ShowDialog();

//                    RefreshVolunteerList();
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Error opening volunteer details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//        }

//        private void AddButton_Click(object sender, RoutedEventArgs e)
//        {
//            var volunteerWindow = new VolunteerWindow(0);
//            volunteerWindow.ShowDialog();

//            RefreshVolunteerList();
//        }
//    }
//}