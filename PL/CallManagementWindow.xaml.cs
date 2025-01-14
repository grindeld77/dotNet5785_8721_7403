using BO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;
using PL.Call;
using PL.Volunteer;

namespace PL
{
    public partial class CallManagementWindow : Window
    {
        public string ButtonText { get; set; }

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        int id;

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallManagementWindow), new PropertyMetadata(null));
        public IEnumerable<BO.CallInList> CallList
        {
            get => (IEnumerable<BO.CallInList>)GetValue(CallListProperty);
            set => SetValue(CallListProperty, value);
        }

        public static readonly DependencyProperty SelectedFilterProperty =
            DependencyProperty.Register("SelectedFilter", typeof(BO.CallStatus), typeof(CallManagementWindow),
            new PropertyMetadata(BO.CallStatus.ALL));

        public BO.CallStatus SelectedFilter
        {
            get => (BO.CallStatus)GetValue(SelectedFilterProperty);
            set => SetValue(SelectedFilterProperty, value);
        }

        public static readonly DependencyProperty SelectedSortProperty =
    DependencyProperty.Register("SelectedSort", typeof(BO.CallInListFields), typeof(CallManagementWindow),
    new PropertyMetadata(BO.CallInListFields.CallId));

        public BO.CallInListFields SelectedSort
        {
            get => (BO.CallInListFields)GetValue(SelectedSortProperty);
            set => SetValue(SelectedSortProperty, value);
        }

        public CallManagementWindow(int Id)
        {
            id = Id;
            InitializeComponent();
            this.DataContext = this;
            Loaded += Window_Loaded;
            Closed += Window_Closed;
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is BO.CallInList selectedCallInList)
            {
                try
                {
                    // קבלת המתנדב המלא לפי ID
                    var selectedCall = s_bl.Call.GetCallDetails(selectedCallInList.CallId);
                    var volunteerWindow = new CallWindow(selectedCallInList.CallId);  
                    volunteerWindow.Show();
                    queryCallList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error fetching volunteer details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(queryCallList);
            queryCallList();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(queryCallList);
        }
        private void queryCallList()
        {
            try
            {
                if (SelectedFilter == BO.CallStatus.ALL)
                {
                    CallList = s_bl.Call.GetCalls(BO.CallStatus.ALL, null, SelectedSort);
                }
                else
                {
                    CallList = s_bl.Call.GetCalls(SelectedFilter, null, SelectedSort);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error querying call list: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var callWindow = new CallWindow(-1);
            callWindow.Show();

            // Refresh the list after closing the window
            queryCallList();
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is BO.CallInList selectedCallInList)
            {
                if (MessageBox.Show("Are you sure you want to delete the selected call?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Call.DeleteCall(selectedCallInList.CallId);
                        queryCallList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void UnassignButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is BO.CallInList selectedCallInList)
            {
                if (MessageBox.Show("Are you sure you want to unassign the selected call?", "Confirm Unassignment", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Call.CancelCallAssignment(id, selectedCallInList.CallId);
                        queryCallList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error unassigning call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

        }
    }
}
