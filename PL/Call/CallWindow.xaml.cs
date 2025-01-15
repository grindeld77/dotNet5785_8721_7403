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
    public partial class CallWindow : Window
    {
        private int callId;

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public bool IsAddMode { get; set; }

        public static readonly DependencyProperty ButtonProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(CallWindow));
        public string Button
        {
            get => (string)GetValue(ButtonProperty);
            set => SetValue(ButtonProperty, value);
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow));

        public BO.Call CurrentCall
        {
            get => (BO.Call)GetValue(CurrentCallProperty);
            set => SetValue(CurrentCallProperty, value);
        }

        public static readonly DependencyProperty SelectedCallTypeProperty =
            DependencyProperty.Register("SelectedCallType", typeof(BO.CallType), typeof(CallWindow));
        public static readonly DependencyProperty SelectedCallTypeInCallWindowProperty =
            DependencyProperty.Register("SelectedCallTypeInCallWindow", typeof(BO.CallType), typeof(CallWindow));

        public BO.CallType SelectedCallType
        {
            get => (BO.CallType)GetValue(SelectedCallTypeProperty);
            set => SetValue(SelectedCallTypeProperty, value);
        }
        public BO.CallType SelectedCallTypeInCallWindow
        {
            get => (BO.CallType)GetValue(SelectedCallTypeInCallWindowProperty);
            set => SetValue(SelectedCallTypeInCallWindowProperty, value);
        }

        public CallWindow(int id)
        {
            InitializeComponent();
            if (id == -1)
            {
                // מצב הוספה
                CurrentCall = new BO.Call();
                Button = "Add";
                IsAddMode = true;
            }
            else
            {
                // מצב עדכון
                CurrentCall = s_bl.Call.GetCallDetails(id);
                Button = "Update";
                IsAddMode = false;
                callId = id;
            }
            SelectedCallType = CurrentCall.Type;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Button == "Add")
                {
                    s_bl.Call.AddCall(CurrentCall);
                }
                else
                {
                    s_bl.Call.UpdateCall(CurrentCall);
                }

                MessageBox.Show("Call saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (callId == -1)
            {
                MessageBox.Show("Cannot delete a Call that hasn't been added yet.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this Call?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Call.DeleteCall(callId);
                    MessageBox.Show("Call deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Deletion canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        public bool IsDeleteButtonVisible
        {
            get
            {
                return callId != -1;
            }
        }
    }
}