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
            if (double.TryParse(txtRiskRange.Text, out double riskRangeInHours))
            {
                RiskRange = TimeSpan.FromHours(riskRangeInHours); // Update the RiskRange in the DependencyProperty
                s_bl.Admin.SetMaxRange(RiskRange); // Call to update the risk range in DAL
                MessageBox.Show($"Risk range updated to {RiskRange.TotalHours} hours.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Invalid number format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void clockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }

        private void configObserver()
        {
            TimeSpan timeSpan = s_bl.Admin.GetMaxRange();
          
            MaxYearRange = (int)(timeSpan.TotalDays / 365);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            TimeSpan RiskRange = s_bl.Admin.GetMaxRange();
            MaxYearRange = (int)(RiskRange.TotalDays / 365); 
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }
        private void btnVolunteer_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerListWindow().Show();
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
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }
    }
}

//  < Volunteer >
//    < Id > 203896956 </ Id >
//    < Name > Michal Levy </ Name >
//    < MobilePhone > 0587925342 </ MobilePhone >
//    < Email > michal.levy@gmail.com </ Email >
//    < Role > Volunteer </ Role >
//    < IsActive > true </ IsActive >
//    < Password > 371406185 </ Password >
//    < CurrentAddress > Jaffa St 1, Jerusalem</CurrentAddress>
//    <Latitude>31.7846145</Latitude>
//    <Longitude>35.21512</Longitude>
//    <MaxCallDistance>12</MaxCallDistance>
//    <DistancePreference>Aerial</DistancePreference>
//  </Volunteer>
//</Volunteers>