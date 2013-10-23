using EasyPaint.Helpers;
using EasyPaint.Messages;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace EasyPaint.View
{
    /// <summary>
    /// Description for Homepage.
    /// </summary>
    public partial class Homepage : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the Homepage class.
        /// </summary>
        public Homepage()
        {
            InitializeComponent();
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<GoToPageMessage>(this, (action) => ReceiveMessage(action));
            //Messenger.Default.Register<RateAppMessage>(this, (action) => ReceiveMessage(action));
            // Messenger.Default.Register<PollCompletedMessage>(this, (action) => ReceiveMessage(action));
        }

        private object ReceiveMessage(BaseMessage action)
        {
            if (action is GoToPageMessage)
            {
                GenericHelper.Navigate(NavigationService, Dispatcher, (action as GoToPageMessage).PageName);
                return null;
            }

            return null;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //foreach (JournalEntry item in NavigationService.BackStack.Reverse())
            //{
            //    NavigationService.RemoveBackEntry();
            //}

            RegisterMessages();
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult mb = MessageBox.Show("$Are you sure you want to exit?", "$Warning", MessageBoxButton.OKCancel);
            if (mb != MessageBoxResult.OK)
            {
                e.Cancel = true;
            }
        }
    }
}