using PL.Volunteer;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

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

        public static readonly DependencyProperty IsAssignmentVisibleProperty =
            DependencyProperty.Register("IsAssignmentVisible", typeof(Visibility), typeof(CallWindow), new PropertyMetadata(Visibility.Collapsed));

        public Visibility IsAssignmentVisible
        {
            get => (Visibility)GetValue(IsAssignmentVisibleProperty);
            set => SetValue(IsAssignmentVisibleProperty, value);
        }

        public CallWindow(int id)
        {
            InitializeComponent();

            if (id == -1)
            {
                ButtonText = "Add";
                CurrentCall = new BO.Call();
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
            if (CurrentCall == null)
            {
                MessageBox.Show("Call details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                if (ButtonText == "Add")
                {
                    CurrentCall.OpenTime = s_bl.Admin.GetClock();
                    s_bl.Call.AddCall(CurrentCall);
                    Close();
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
        private void queryCall()
        {
            int id = CurrentCall!.Id;
            CurrentCall = s_bl.Call.GetCallDetails(id);
        }
        private void callObserver()
        {
            queryCall();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentCall!.Id != -1)
                s_bl.Call.AddObserver(CurrentCall!.Id, callObserver);
        }

        /// <summary>
        /// Event handler for when the window is closed.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentCall!.Id != 0)
                s_bl.Call.RemoveObserver(CurrentCall!.Id, callObserver);
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
