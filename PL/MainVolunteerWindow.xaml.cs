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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainVolunteerWindow.xaml
    /// </summary>
    public partial class MainVolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int volunteerId;
        public BO.Volunteer CurrentVolunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(MainVolunteerWindow));

        public MainVolunteerWindow(int id)
        {
            InitializeComponent();
            if (id == 0)
            {
                // מצב הוספה
                CurrentVolunteer = new BO.Volunteer();

            }
            else
            {
                // מצב עדכון
                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);

            }
            volunteerId = id;
            DataContext = this; // קישור ל-DataContext של החלון
        }

        // Finish Call Button Click Handler
        private void FinishCall_Click(object sender, RoutedEventArgs e)
        {

        }

        // Cancel Call Button Click Handler
        private void CancelCall_Click(object sender, RoutedEventArgs e)
        {

        }

        // View Call History Button Click Handler
        private void ViewCallHistory_Click(object sender, RoutedEventArgs e)
        {

        }

        // Select Call Button Click Handler
        private void SelectCall_Click(object sender, RoutedEventArgs e)
        {

        }

        // Update Volunteer Button Click Handler
        private void UpdateVolunteer_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
