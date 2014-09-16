using System.Windows;
using EasyPaint.Data;
using EasyPaint.Helpers;
using EasyPaint.Messages;
using EasyPaint.Model;
using EasyPaint.Model.UI;
using EasyPaint.Settings;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Tasks;
using Telerik.Windows.Controls;
using Wp8Shared.UserControls;


namespace EasyPaint.ViewModel
{
    public class LevelSelectorViewModel : AppViewModel
    {
        public RelayCommand GotoHomepageCommand { get; private set; }
        public RelayCommand SetLevelEasyCommand { get; private set; }
        public RelayCommand SetLevelMediumCommand { get; private set; }
        public RelayCommand SetLevelHardCommand { get; private set; }

        public LevelSelectorViewModel(IDataService dataService)
        {
            _dataService = dataService;
            //_dataService.GetData(
            //    (item, error) =>
            //    {
            //        if (error != null)
            //        {
            //            throw new Exception("invalid data source: " + error.Message);
            //        }
            //    });

            GotoHomepageCommand = new RelayCommand(() => GotoHomepage());

            SetLevelEasyCommand = new RelayCommand(() => SetGameLevel(GameLevel.Easy));
            SetLevelMediumCommand = new RelayCommand(() => SetGameLevel(GameLevel.Medium));
            SetLevelHardCommand = new RelayCommand(() => SetGameLevel(GameLevel.Hard));
        }

        private object SetGameLevel(GameLevel gameLevel)
        {
            AppSettings.GameLevel = gameLevel;
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_GroupSeletor);
            return null;
        }

        private object GotoHomepage()
        {
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Homepage);
            return null;
        }

    }
}
