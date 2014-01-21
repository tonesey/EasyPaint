using EasyPaint.Data;
using EasyPaint.Helpers;
using EasyPaint.Messages;
using EasyPaint.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls;

namespace EasyPaint.ViewModel
{
    public class CreditsViewModel : AppViewModel
    {
        public RelayCommand GotoHomepageCommand { get; private set; }

        public CreditsViewModel(IDataService dataService)
        {
            //_dataService = dataService;
            //_dataService.GetData(
            //    (item, error) =>
            //    {
            //        if (error != null)
            //        {
            //            throw new Exception("invalid data source: " + error.Message);
            //        }
            //        InitGroups(item.CfgData.Groups);
            //    });

            GotoHomepageCommand = new RelayCommand(() => GotoHomepage());
        }

        private object GotoHomepage()
        {
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Homepage);
            return null;
        }
    }
}
