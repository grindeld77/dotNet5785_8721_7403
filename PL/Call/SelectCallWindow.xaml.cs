using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PL;
namespace PL.Call
{
    /// <summary>
    /// Interaction logic for SelectCallWindow.xaml
    /// </summary>
    public partial class SelectCallWindow : Window
    {
        private readonly int UserId;
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // Dependency Property for OpenCallList
        public static readonly DependencyProperty OpenCallListProperty =
            DependencyProperty.Register("OpenList", typeof(IEnumerable<BO.OpenCallInList>), typeof(SelectCallWindow));

        public IEnumerable<BO.OpenCallInList> OpenList
        {
            get => (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallListProperty);
            set => SetValue(OpenCallListProperty, value);
        }

        // Dependency Property for SelectedCallType
        public static readonly DependencyProperty SelectedCallTypeProperty =
            DependencyProperty.Register("SelectedCallType", typeof(BO.CallType), typeof(SelectCallWindow),
            new PropertyMetadata(BO.CallType.None, OnSelectedCallTypeChanged));

        public BO.CallType SelectedCallType
        {
            get => (BO.CallType)GetValue(SelectedCallTypeProperty);
            set => SetValue(SelectedCallTypeProperty, value);
        }

        // Dependency Property for SelectedSort
        public static readonly DependencyProperty SelectedSortProperty =
            DependencyProperty.Register("SelectedSort", typeof(BO.OpenCallInListFields), typeof(SelectCallWindow),
            new PropertyMetadata(BO.OpenCallInListFields.Id, OnSelectedSortChanged));

        public BO.OpenCallInListFields SelectedSort
        {
            get => (BO.OpenCallInListFields)GetValue(SelectedSortProperty);
            set => SetValue(SelectedSortProperty, value);
        }

        // Constructor
        public SelectCallWindow(int id)
        {
            InitializeComponent();
            UserId = id;
            try
            {
                // קריאה מיד לאחר אתחול הערכים
                OpenList = s_bl.Call.GetOpenCallsForVolunteer(id, null, null);

                // הגדרת האירועים
                Loaded += Window_Loaded;
                Closed += Window_Closed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load call details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }


        private void queryOpenCallList()
        {
            try
            {
                OpenList = s_bl.Call.GetOpenCallsForVolunteer(UserId,
                    SelectedCallType == BO.CallType.None ? null : SelectedCallType,
                    SelectedSort);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to query call list: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Triggered when SelectedCallType changes
        private static void OnSelectedCallTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SelectCallWindow window)
            {
                window.queryOpenCallList();
            }
        }

        // Triggered when SelectedSort changes
        private static void OnSelectedSortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SelectCallWindow window)
            {
                window.queryOpenCallList();
            }
        }

        // Event handler for when the window is loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(queryOpenCallList);
        }

        // Event handler for when the window is closed
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(queryOpenCallList);
        }

        private void SelectCall_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is BO.OpenCallInList selectedCall)
            {
                try
                {
                    s_bl.Call.AssignVolunteerToCall(UserId, selectedCall.Id);
                    MessageBox.Show($"Selected Call ID: {selectedCall.Id}", "Call Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                    queryOpenCallList();

                    if (Application.Current.Windows.OfType<MainVolunteerWindow>().FirstOrDefault() is MainVolunteerWindow volunteerWindow)
                    {
                        volunteerWindow.RefreshData();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to assign volunteer to call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Failed to retrieve the selected call.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
