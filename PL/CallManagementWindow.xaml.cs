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
using System.Windows.Threading;

namespace PL
{
    public partial class CallManagementWindow : Window
    {
        public string ButtonText { get; set; }

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        int id;

        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallManagementWindow), new PropertyMetadata(null));
        public IEnumerable<BO.CallInList> CallList
        {
            get => (IEnumerable<BO.CallInList>)GetValue(CallListProperty);
            set => SetValue(CallListProperty, value);
        }

        public static readonly DependencyProperty SelectedFilterProperty =
            DependencyProperty.Register("SelectedFilter", typeof(BO.CallStatus), typeof(CallManagementWindow),
            new PropertyMetadata(BO.CallStatus.ALL, onSelectedFilterChange));

        private static void onSelectedFilterChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as CallManagementWindow;
            window?.queryCallList();
        }

        public BO.CallStatus SelectedFilter
        {
            get => (BO.CallStatus)GetValue(SelectedFilterProperty);
            set => SetValue(SelectedFilterProperty, value);
        }

        public static readonly DependencyProperty SelectedSortProperty =
    DependencyProperty.Register("SelectedSort", typeof(BO.CallInListFields), typeof(CallManagementWindow),
    new PropertyMetadata(BO.CallInListFields.CallId, onSelectedSorChange));

        private static void onSelectedSorChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as CallManagementWindow;
            window?.queryCallList();
        }

        public BO.CallInListFields SelectedSort
        {
            get => (BO.CallInListFields)GetValue(SelectedSortProperty);
            set => SetValue(SelectedSortProperty, value);
        }

        // DependencyProperty for SelectedCall
        public static readonly DependencyProperty SelectedCallProperty = DependencyProperty.Register(
            "SelectedCall", typeof(BO.CallInList), typeof(CallManagementWindow), new PropertyMetadata(null));

        // Property wrapper for SelectedCall
        public BO.CallInList SelectedCall
        {
            get { return (BO.CallInList)GetValue(SelectedCallProperty); }
            set { SetValue(SelectedCallProperty, value); }
        }

        public CallManagementWindow(int Id)
        {
            id = Id;
            InitializeComponent();
            Loaded += Window_Loaded;
            Closed += Window_Closed;
        }
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView && listView.SelectedItem is BO.CallInList selectedCallInList)
            {
                try
                {
                    var selectedCall = s_bl.Call.GetCallDetails(selectedCallInList.CallId);
                    var volunteerWindow = new CallWindow(selectedCallInList.CallId);
                    volunteerWindow.Show();
                    callListObserver();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error fetching volunteer details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(callListObserver);
            callListObserver();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(callListObserver);
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

        private void callListObserver() //stage 7
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryCallList();
                });
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var callWindow = new CallWindow(-1);
            callWindow.Show();

            // Refresh the list after closing the window
            callListObserver();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall != null)
            {
                try
                {
                    if (MessageBox.Show("Are you sure you want to delete the selected call?", "Confirm Deletion",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        s_bl.Call.DeleteCall(SelectedCall.CallId); //to do: fix this, this is not the correct way to delete a call
                        callListObserver();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void UnassignButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall != null)
            {
                try
                {
                    if (MessageBox.Show("Are you sure you want to unassign the selected call?", "Confirm Unassignment",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        
                        s_bl.Call.CancelCallAssignment(id , SelectedCall.CallId); //to do: fix this, this is not the correct way to unassign a call
                        callListObserver();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error unassigning call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
