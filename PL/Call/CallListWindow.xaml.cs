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

using System.Collections.ObjectModel;
using System.Windows;
using BO;
using System.Windows.Threading;

namespace PL.Call
{
    public partial class CallListWindow : Window
    {
        public ObservableCollection<BO.ClosedCallInList> ClosedCalls { get; set; }
        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public CallListWindow(IEnumerable<BO.ClosedCallInList> closedCalls)
        {
            try
            {
                ClosedCalls = new ObservableCollection<BO.ClosedCallInList>(closedCalls);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load closed calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            InitializeComponent();
        }


        private void queryClosedCallList()
        {
            try
            {
                // ClosedCalls = new ObservableCollection<BO.ClosedCallInList>(s_bl.Call.GetClosedCallsByVolunteer());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to query call list: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void closedCallListObserver() //stage 7
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryClosedCallList();
                });
        }

        // Event handler for when the window is loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(closedCallListObserver);
        }

        // Event handler for when the window is closed
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(closedCallListObserver);
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
