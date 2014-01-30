using EasyPaint.Helpers;
using EasyPaint.Messages;
using EasyPaint.Model.UI;
using EasyPaint.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace EasyPaint.View
{
    public partial class CreditsPage : PhoneApplicationPage
    {

        public CreditsPage()
        {
            InitializeComponent();
            Loaded += CreditsPage_Loaded;
            Unloaded += CreditsPage_Unloaded;
        }


        void CreditsPage_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        void CreditsPage_Loaded(object sender, RoutedEventArgs e)
        {
            AppViewModel.CurrentPage = this;
            //MessagingHelper.GetInstance().CurrentDispatcher = Dispatcher;
            InitPage();
        }

        private void InitPage()
        {
        }
    }
}