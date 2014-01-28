using EasyPaint.Data;
using EasyPaint.Helpers;
using EasyPaint.Messages;
using EasyPaint.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Telerik.Windows.Controls;
using Wp8Shared.UserControls;

namespace EasyPaint.ViewModel
{
    public class HelpViewModel : AppViewModel
    {
        public RelayCommand GotoHomepageCommand { get; private set; }
        public RelayCommand RateAppCommand { get; private set; }
        public RelayCommand ContactUsCommand { get; private set; }

        public HelpViewModel(IDataService dataService)
        {
            GotoHomepageCommand = new RelayCommand(() => GotoHomepage());
            RateAppCommand = new RelayCommand(() => RateApp());
            ContactUsCommand = new RelayCommand(() => ContactUs());

        }

        private object ContactUs()
        {
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    MyMsgbox.Show(AppViewModel.CurrentPage, MsgboxMode.Ok, LocalizedResources.Error_NoNetworkAvailable);
                    return null;
                }
                var email = new EmailComposeTask();
                email.To = "centapp@hotmail.com";
                email.Subject = string.Format(LocalizedResources.FeedbackOrCommentText, "The Color Hunter");
                email.Show();
            }
            catch (InvalidOperationException ignored)
            {
            }
            return null;
        }

        private object RateApp()
        {
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    MyMsgbox.Show(AppViewModel.CurrentPage, MsgboxMode.Ok, LocalizedResources.Error_NoNetworkAvailable);
                    return null;
                }
                //FeedbackHelper.Default.Reviewed();
                MarketplaceReviewTask task = new MarketplaceReviewTask();
                task.Show();
            }
            catch (InvalidOperationException ignored)
            {
            }
            return null;
        }

        private object GotoHomepage()
        {
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Homepage);
            return null;
        }
    }
}
