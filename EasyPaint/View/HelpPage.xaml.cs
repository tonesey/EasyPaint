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
    public partial class HelpPage : PhoneApplicationPage
    {

        public HelpPage()
        {
            InitializeComponent();
            Loaded += ItemSelectorPage_Loaded;
            Unloaded += ItemSelectorPage_Unloaded;
        }


        void ItemSelectorPage_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        void ItemSelectorPage_Loaded(object sender, RoutedEventArgs e)
        {
            MessagingHelper.GetInstance().CurrentDispatcher = Dispatcher;
            InitPage();
        }

        private void InitPage()
        {
        }
    }
}