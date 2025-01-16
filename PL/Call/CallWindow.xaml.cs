using PL.Volunteer;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PL.Call
{
    public partial class CallWindow : Window
    {
        private int callId;

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(CallWindow), new PropertyMetadata(""));

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        public BO.Call CurrentCall
        {
            get => (BO.Call)GetValue(CurrentCallProperty);
            set => SetValue(CurrentCallProperty, value);
        }

        public static readonly DependencyProperty IsDeleteButtonVisibleProperty =
            DependencyProperty.Register("IsDeleteButtonVisible", typeof(Visibility), typeof(CallWindow), new PropertyMetadata(Visibility.Collapsed));

        public Visibility IsDeleteButtonVisible
        {
            get => (Visibility)GetValue(IsDeleteButtonVisibleProperty);
            set => SetValue(IsDeleteButtonVisibleProperty, value);
        }

        public CallWindow(int id)
        {
            InitializeComponent();

            if (id == -1)
            {
                CurrentCall = new BO.Call
                {
                    OpenTime = DateTime.Now,
                    MaxEndTime = DateTime.Now.AddHours(1),
                    Status = BO.CallStatus.Open,
                    Assignments = null,
                };
                ButtonText = "Add";
                IsDeleteButtonVisible = Visibility.Collapsed;
            }
            else
            {
                CurrentCall = s_bl.Call.GetCallDetails(id);
                ButtonText = "Update";
                IsDeleteButtonVisible = Visibility.Visible;
                callId = id;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
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
    }
}
