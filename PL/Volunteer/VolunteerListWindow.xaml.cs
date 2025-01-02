using BO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }
        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteersList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        
        public VolunteerListWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
            Closed += Window_Closed;
        }


        private BO.CallType _selectedCallType = BO.CallType.None;
        public BO.CallType SelectedCallType
        {
            get => _selectedCallType;
            set
            {
                _selectedCallType = value;
                queryVolunteerList();
            }
        }
        private void queryVolunteerList()
        {
            VolunteerList = SelectedCallType == BO.CallType.None
                ? s_bl?.Volunteer.GetVolunteers(null, VolunteerFieldVolunteerInList.CurrentCallType, null)
                : s_bl?.Volunteer.GetVolunteers(null, VolunteerFieldVolunteerInList.CurrentCallType, SelectedCallType);

        }

        private void volunteerListObserver()
        {
            queryVolunteerList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(volunteerListObserver);
            queryVolunteerList(); 
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(volunteerListObserver);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryVolunteerList();
        }
    }
}
