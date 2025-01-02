//using BO;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;

//namespace PL.Volunteer
//{
//    /// <summary>
//    /// Interaction logic for VolunteerListWindow.xaml
//    /// </summary>
//    public partial class VolunteerListWindow : Window
//    {
//        public VolunteerListWindow()
//        {
//            InitializeComponent();
//        }
//        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

//        public IEnumerable<BO.VolunteerInList> VolunteerList
//        {
//            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
//            set { SetValue(VolunteerListProperty, value); }
//        }

//        public static readonly DependencyProperty VolunteerListProperty =
//            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));
//        public BO.CallType SelectedCallType { get; set; } = BO.CallType.AllergicReaction;

//        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            System.Diagnostics.Debug.WriteLine("Selection changed in ListView.");

//            System.Diagnostics.Debug.WriteLine($"SelectedCallType: {SelectedCallType}");
//            System.Diagnostics.Debug.WriteLine($"VolunteerList count: {VolunteerList?.Count()}");
//            if (SelectedCallType == BO.CallType.None)
//            {
//                VolunteerList = s_bl?.Volunteer.GetVolunteers(null, VolunteerFieldVolunteerInList.CurrentCallType, null);
//            }
//            else
//            {
//                VolunteerList = s_bl?.Volunteer.GetVolunteers(null, VolunteerFieldVolunteerInList.CurrentCallType, SelectedCallType);
//            }
//        }
//        private void queryVolunteerList()
//        {
//            if (SelectedCallType == BO.CallType.None)
//            {
//                VolunteerList = (IEnumerable<BO.VolunteerInList>)(s_bl?.Volunteer.GetVolunteers(null, VolunteerFieldVolunteerInList.CurrentCallType, null));
//            }
//            else
//            {
//                VolunteerList = (IEnumerable<BO.VolunteerInList>)(s_bl?.Volunteer.GetVolunteers(null, VolunteerFieldVolunteerInList.CurrentCallType, SelectedCallType));
//            }
//        }
//        private void volunteerListObserver()
//        {
//            queryVolunteerList();
//        }
//        private void Window_Loaded(object sender, RoutedEventArgs e)
//        {
//            s_bl.Volunteer.AddObserver(volunteerListObserver);
//        }
//        private void Window_Closed(object sender, EventArgs e)
//        {
//            s_bl.Volunteer.RemoveObserver(volunteerListObserver);
//        }

//    }

//}


using BO;
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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private IEnumerable<BO.VolunteerInList> _volunteerList;
        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return _volunteerList; }
            set
            {
                _volunteerList = value;
                OnPropertyChanged(nameof(VolunteerList));
            }
        }

        public BO.CallType SelectedCallType { get; set; } = BO.CallType.AllergicReaction;

        public VolunteerListWindow()
        {
            InitializeComponent();
            this.DataContext = this; // הגדרת ה-DataContext כדי לאפשר Binding
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedCallType == BO.CallType.None)
            {
                VolunteerList = s_bl?.Volunteer.GetVolunteers(null, VolunteerFieldVolunteerInList.CurrentCallType, null);
            }
            else
            {
                VolunteerList = s_bl?.Volunteer.GetVolunteers(null, VolunteerFieldVolunteerInList.CurrentCallType, SelectedCallType);
            }
        }

        private void queryVolunteerList()
        {
            if (SelectedCallType == BO.CallType.None)
            {
                VolunteerList = s_bl?.Volunteer.GetVolunteers(null, VolunteerFieldVolunteerInList.CurrentCallType, null);
            }
            else
            {
                VolunteerList = s_bl?.Volunteer.GetVolunteers(null, VolunteerFieldVolunteerInList.CurrentCallType, SelectedCallType);
            }
        }

        private void volunteerListObserver()
        {
            queryVolunteerList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;  // הגדרת ה-DataContext
            s_bl.Volunteer.AddObserver(volunteerListObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(volunteerListObserver);
        }
    }
}
