using PL.Volunteer;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        int tampUserId;

        private volatile DispatcherOperation? _observerOperation = null; //stage 7


        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow), new PropertyMetadata(1));

        public static readonly DependencyProperty IsSimulatorRunningProperty =
            DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public int Interval
        {
            get => (int)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        public bool IsSimulatorRunning
        {
            get => (bool)GetValue(IsSimulatorRunningProperty);
            set => SetValue(IsSimulatorRunningProperty, value);
        }


        public MainWindow(int userId)
        {
            InitializeComponent();
            CurrentTime = s_bl.Admin.GetClock();
            TimeSpan timeSpan = s_bl.Admin.GetMaxRange();
            MaxYearRange = (int)(timeSpan.TotalDays / 365); // המרה לשנים
            Loaded += Window_Loaded;
            Closed += Window_Closed;
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
            tampUserId= userId;
            UpdateCallCounts();
        }
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

        // Event Handlers for Clock Advancement
        private void AddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);
        }

        private void AddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);
        }

        private void AddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Day);
        }
        private void AddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Month);
        }

        private void AddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Year);
        }

        // Dependency Property for Call Counts
        public int[] CallCountsByStatus
        {
            get { return (int[])GetValue(CallCountsByStatusProperty); }
            set { SetValue(CallCountsByStatusProperty, value); }
        }

        public static readonly DependencyProperty CallCountsByStatusProperty =
            DependencyProperty.Register("CallCountsByStatus", typeof(int[]), typeof(MainWindow), new PropertyMetadata(new int[6]));

        // Method to Update Call Counts
        private void UpdateCallCounts()
        {
            CallCountsByStatus = s_bl.Call.GetCallCountsByStatus();
        }

        // Dependency Property for Risk Range
        public static readonly DependencyProperty RiskRangeProperty =
            DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow), new PropertyMetadata(TimeSpan.FromHours(1)));

        public TimeSpan RiskRange
        {
            get { return (TimeSpan)GetValue(RiskRangeProperty); }
            set { SetValue(RiskRangeProperty, value); }
        }

        public int MaxYearRange
        {
            get { return (int)GetValue(MaxYearRangeProperty); }
            set { SetValue(MaxYearRangeProperty, value); }
        }

        public static readonly DependencyProperty MaxYearRangeProperty =
            DependencyProperty.Register("MaxYearRange", typeof(int), typeof(MainWindow));

        // Event Handler for Risk Range Update Button
        private void UpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetMaxRange(RiskRange);
        }

        private void clockObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    CurrentTime = s_bl.Admin.GetClock();
                });
        }

        private void configObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    TimeSpan timeSpan = s_bl.Admin.GetMaxRange();

                    MaxYearRange = (int)(timeSpan.TotalDays / 365);
                });
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCallCounts(); // Ensure call counts are updated on load
            CurrentTime = s_bl.Admin.GetClock();
            TimeSpan RiskRange = s_bl.Admin.GetMaxRange();
            MaxYearRange = (int)(RiskRange.TotalDays / 365); 
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (IsSimulatorRunning)
            {
                s_bl.Admin.StopSimulator();
            }
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }
        private void btnVolunteer_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerListWindow(tampUserId).Show();
        }
        private void btnInitializeDB_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to initialize the database?", "Confirm Initialization", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window != this)
                            window.Close();
                    }

                    s_bl.Admin.InitializeDB();
                    UpdateCallCounts(); // Ensure call counts are updated on load
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }
        private void btnResetDB_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reset the database?", "Confirm Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window != this)
                            window.Close();
                    }
                    s_bl.Admin.ResetDB();
                    UpdateCallCounts(); // Ensure call counts are updated on load
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private void btnManageCalls_Click(object sender, RoutedEventArgs e)
        {
            new CallManagementWindow(tampUserId).Show();
            UpdateCallCounts();
        }

        private void StartSimulator_Click(object sender, RoutedEventArgs e)
        {
            if (IsSimulatorRunning)
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
                UpdateCallCounts();
            }
            else
            {
                s_bl.Admin.StartSimulator(Interval);
                IsSimulatorRunning = true;
                UpdateCallCounts();
            }
        }
    }
}
