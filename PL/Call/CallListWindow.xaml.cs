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

namespace PL.Call
{
    public partial class CallListWindow : Window
    {
        public ObservableCollection<BO.ClosedCallInList> ClosedCalls { get; set; }

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

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
