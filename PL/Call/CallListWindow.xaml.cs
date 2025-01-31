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
using DO;

namespace PL.Call
{
    public partial class CallListWindow : Window
    {
        public ObservableCollection<ClosedCallInList> ClosedCalls { get; set; } = new ObservableCollection<ClosedCallInList>();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public int VolunteerId { get; set; }

        public CallListWindow(IEnumerable<ClosedCallInList> closedCalls,int id)
        {
            try
            {
                ClosedCalls = new ObservableCollection<ClosedCallInList>(closedCalls);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load closed calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            InitializeComponent();
            VolunteerId = id;
        }

        private void queryClosedCallList()
        {
            try
            {
                var calls = s_bl.Call.GetClosedCallsByVolunteer(VolunteerId, null, null); // Get closed calls from the BL
                foreach (var call in calls)
                {
                    ClosedCalls.Add(call); // Add each call to the ViewModel's collection
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to query call list: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void closedCallListObserver() //stage 7
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryClosedCallList();
                });
            }
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


/*
 using BO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PL.viewModel;

public class VolunteerCallsHistoryVM : ViewModelBase
{
private readonly int volunteerId;
private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
public ObservableCollection<ClosedCallInList> ClosedCalls { get; set; } = new ObservableCollection<ClosedCallInList>();

public IEnumerable<CallType> CallTypes { get; } = Enum.GetValues(typeof(CallType)).Cast<CallType>();
public IEnumerable<CallField> SortOptions { get; } = Enum.GetValues(typeof(CallField)).Cast<CallField>();

private CallType? selectedFilterOption;
public CallType? SelectedFilterOption
{
get => selectedFilterOption;
set
{
if (selectedFilterOption != value)
{
selectedFilterOption = value;
OnPropertyChanged(nameof(SelectedFilterOption));
LoadClosedCalls();
}
}
}

private CallField? selectedSortOption;
public CallField? SelectedSortOption
{
get => selectedSortOption;
set
{
if (selectedSortOption != value)
{
selectedSortOption = value;
OnPropertyChanged(nameof(SelectedSortOption));
LoadClosedCalls();
}
}
}

public VolunteerCallsHistoryVM(int volunteerId)
{
this.volunteerId = volunteerId;
// הוספת משקיף לעדכון הרשימה
s_bl.Call.AddObserver(UpdateClosedCallsObserver);
LoadClosedCalls();
}
// משקיף לעדכון רשימת הקריאות הסגורות
public void UpdateClosedCallsObserver()
{
LoadClosedCalls();
}
public void LoadClosedCalls()
{
ClosedCalls.Clear();
try
{
var calls = s_bl.Call.GetClosedCallsByVolunteer(volunteerId, SelectedFilterOption, SelectedSortOption) ?? Enumerable.Empty<ClosedCallInList>();
foreach (var call in calls)
{
ClosedCalls.Add(call);
}
}
catch (Exception ex)
{
MessageBox.Show($"Error loading closed calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
}
}
}
 */




/*
 using PL.viewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PL.Calls;

/// <summary>
/// Interaction logic for VolunteerCallsHistoryWindow.xaml
/// </summary>
public partial class VolunteerCallsHistoryWindow : Window
{


// Reference to the business logic layer
static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

// ViewModel for binding data to the UI
public VolunteerCallsHistoryVM Vm { get; set; }

// ID of the volunteer whose call history is displayed
private int VolunteerId;

/// <summary>
/// Constructor for the VolunteerCallsHistoryWindow.
/// Initializes the ViewModel and loads the closed calls.
/// </summary>
/// <param name="volunteerId">ID of the volunteer</param>
public VolunteerCallsHistoryWindow(int volunteerId)
{
VolunteerId = volunteerId;
try {
Vm = new VolunteerCallsHistoryVM(volunteerId); // Initialize the ViewModel
DataContext = Vm; // Set the data context for data binding
InitializeComponent();


queryClosedCallList(); // Load the list of closed calls
}
catch (Exception ex)
{
MessageBox.Show($"Error loading closed calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
}
}

/// <summary>
/// Fetches the list of closed calls for the volunteer and updates the ViewModel.
/// </summary>
private void queryClosedCallList()
{
Vm.ClosedCalls.Clear(); // Clear existing call data in the ViewModel
try
{
var calls = s_bl.Call.GetClosedCallsByVolunteer(VolunteerId, null, null); // Get closed calls from the BL
foreach (var call in calls)
{
Vm.ClosedCalls.Add(call); // Add each call to the ViewModel's collection
}
}
catch (Exception ex)
{
MessageBox.Show($"Error loading the list of closed calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
}
}
private void Window_Closed(object sender, EventArgs e)
{
s_bl.Call.RemoveObserver(Vm.UpdateClosedCallsObserver);
}
/// <summary>
/// Event handler for when the ComboBox selection changes.
/// Applies filtering logic to the displayed call list.
/// </summary>
private void btnBack_Click(object sender, RoutedEventArgs e)
{
this.Close();
}

}

 */




/*
 <Window x:Class="PL.Calls.VolunteerCallsHistoryWindow"
xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
mc:Ignorable="d"
Title="Volunteer Calls History" Height="640" Width="800" Background="AliceBlue" FontFamily="Segoe UI" Closed="Window_Closed">

<Grid Margin="10">
<Grid.RowDefinitions>
<RowDefinition Height="Auto" />
<RowDefinition Height="*" />
</Grid.RowDefinitions>

<!-- כותרת -->
<TextBlock Text="Volunteer Calls History"
FontSize="24" FontWeight="Bold"
Foreground="DarkGreen"
HorizontalAlignment="Center"
Margin="0,0,0,10" />

<!-- Filter and Sort Section -->
<StackPanel Orientation="Horizontal" Margin="5,50,5,5" Grid.Row="0" VerticalAlignment="Top">
<!-- Filter -->
<TextBlock Text="Filter by Type:" FontWeight="Bold" VerticalAlignment="Center" Margin="0,0,10,0" />
<ComboBox Width="200"
ItemsSource="{Binding CallTypes,Mode=TwoWay}"
SelectedItem="{Binding SelectedFilterOption,Mode=TwoWay}"
Margin="0,0,10,0" />

<!-- Sort -->
<TextBlock Text="Sort By:" FontWeight="Bold" VerticalAlignment="Center" Margin="10,0,10,0" />
<ComboBox Width="200"
ItemsSource="{Binding SortOptions,Mode=TwoWay}"
SelectedItem="{Binding SelectedSortOption,Mode=TwoWay}"
Margin="0,0,10,0" />
</StackPanel>

<!-- DataGrid -->
<DataGrid Grid.Row="1"
ItemsSource="{Binding ClosedCalls,Mode=TwoWay}"
AutoGenerateColumns="False"
CanUserSortColumns="True"
Margin="5"
BorderBrush="Gray"
BorderThickness="1"
Background="White"
AlternatingRowBackground="LightGray"
RowHeaderWidth="0"
IsReadOnly="True">
<DataGrid.Columns>
<DataGridTextColumn Header="ID" Binding="{Binding Id,Mode=TwoWay}" Width="*" />
<DataGridTextColumn Header="Type" Binding="{Binding CallType,Mode=TwoWay}" Width="2*" />
<DataGridTextColumn Header="Address" Binding="{Binding Address,Mode=TwoWay}" Width="3*" />
<DataGridTextColumn Header="Open Time" Binding="{Binding OpenTime,Mode=TwoWay}" Width="2*" />
<DataGridTextColumn Header="Assignment Start" Binding="{Binding AssignmentStartTime,Mode=TwoWay}" Width="2*" />
<DataGridTextColumn Header="End Time" Binding="{Binding ActualEndTime,Mode=TwoWay}" Width="2*" />
<DataGridTextColumn Header="End Type" Binding="{Binding EndType,Mode=TwoWay}" Width="2*" />
</DataGrid.Columns>
</DataGrid>
</Grid>
</Window>

 */