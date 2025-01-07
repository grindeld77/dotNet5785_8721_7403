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
using System.Windows.Shapes;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for SelectCallWindow.xaml
    /// </summary>
    public partial class SelectCallWindow : Window
    {
        private int UserId;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public static readonly DependencyProperty OpenCallListProperty =
            DependencyProperty.Register("OpenCallListProperty", typeof(IEnumerable<BO.OpenCallInList>), typeof(SelectCallWindow));

        public IEnumerable<BO.OpenCallInList> OpenList
        {
            get => (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallListProperty);
            set => SetValue(OpenCallListProperty, value);
        }

        public static readonly DependencyProperty SelectedCallTypeProperty =
            DependencyProperty.Register("SelectedCallType", typeof(BO.CallType), typeof(SelectCallWindow),
            new PropertyMetadata(BO.CallType.None, OnSelectedCallTypeChanged));

        public BO.CallType SelectedCallType
        {
            get => (BO.CallType)GetValue(SelectedCallTypeProperty);
            set => SetValue(SelectedCallTypeProperty, value);
        }

        private static void OnSelectedCallTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as SelectCallWindow;
            window?.queryOpenCallList();
        }

        public static readonly DependencyProperty SelectedSortProperty =
            DependencyProperty.Register("SelectedSort", typeof(BO.OpenCallInListFields), typeof(SelectCallWindow));

        public BO.OpenCallInListFields SelectedSort
        {
            get => (BO.OpenCallInListFields)GetValue(SelectedSortProperty);
            set => SetValue(SelectedSortProperty, value);
        }

        private void queryOpenCallList()
        {
            if (SelectedCallType == BO.CallType.None)
            {
                if (SelectedSort == BO.OpenCallInListFields.Id)
                    OpenList = s_bl.Call.GetOpenCallsForVolunteer(UserId, BO.CallType.None, null);
                else
                    OpenList = s_bl.Call.GetOpenCallsForVolunteer(UserId, BO.CallType.None, SelectedSort);
            }
            else
            {
                if (SelectedSort == BO.OpenCallInListFields.Id)
                    OpenList = s_bl.Call.GetOpenCallsForVolunteer(UserId, SelectedCallType, null);
                else
                    OpenList = s_bl.Call.GetOpenCallsForVolunteer(UserId, SelectedCallType, SelectedSort);
            }
        }

        // Constructor updated to initialize UserId
        public SelectCallWindow(int id)
        {
            InitializeComponent();
            try
            {
                UserId = id; // Initialize UserId here
                OpenList = s_bl.Call.GetOpenCallsForVolunteer(id, SelectedCallType, SelectedSort);

                // Register event handlers
                Loaded += Window_Loaded;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load calls details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close(); // Close the window if an error occurs
            }
        }

        // Event handler for when the window is opened
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(queryOpenCallList);
            queryOpenCallList();
        }

        // Event handler for when the window is closed
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(queryOpenCallList);
        }

        private void CallTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryOpenCallList();
        }

        private void SelectCall_Click(object sender, RoutedEventArgs e)
        {
            // Handle the select call action
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryOpenCallList();
        }
    }
}
