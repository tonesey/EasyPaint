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
    public class GalleryViewModel : AppViewModel
    {

        private List<Group> _groups = null;

        LoopingListDataSource _listDs = new LoopingListDataSource(0);
        public LoopingListDataSource ListDs
        {
            get
            {
                return _listDs;
            }
            private set
            {
                _listDs = value;
                OnPropertyChanged("ListDs");
            }
        }

        private ObservableCollection<ItemViewModel> _items = new ObservableCollection<ItemViewModel>();
        public ObservableCollection<ItemViewModel> Items
        {
            get
            {
                return this._items;
            }
            private set
            {
                this._items = value;
                OnPropertyChanged("Items");
            }
        }

        private ItemViewModel _SelectedItem = null;
        public ItemViewModel SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        //private bool _fullTrainingPackAvailable = false;
        //public bool FullTrainingPackAvailable
        //{
        //    get
        //    {
        //        return _fullTrainingPackAvailable;
        //    }
        //    set
        //    {
        //        _fullTrainingPackAvailable = value;
        //        OnPropertyChanged("FullTrainingPackAvailable");
        //        OnPropertyChanged("InfoFieldContent");
        //    }
        //}

        public string InfoFieldContent
        {
            get
            {
                //return FullTrainingPackAvailable ? LocalizedResources.GalleryPage_InfoText_Purchased : LocalizedResources.GalleryPage_InfoText_NotPurchased;
                //return LocalizedResources.GalleryPage_InfoText_NotPurchased;
                return LocalizedResources.GalleryPage_InfoText_Purchased;
            }
        }

        public RelayCommand ItemSelectedCommand { get; private set; }
        public RelayCommand GotoHomepageCommand { get; private set; }
        public RelayCommand GetAllItemsTrainingPackCommand { get; private set; }

        public GalleryViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        throw new Exception("invalid data source: " + error.Message);
                    }
                    _groups = item.CfgData.Groups;
                    InitGalleryItems();
                });

            ItemSelectedCommand = new RelayCommand(() => NavigateToSelectedItemCommand());
            GotoHomepageCommand = new RelayCommand(() => GotoHomepage());
            //GetAllItemsTrainingPackCommand = new RelayCommand(() => GetAllItemsTrainingPack());
        }

        private void InitGalleryItems()
        {
            Items = new ObservableCollection<ItemViewModel>();

            //if (!AppSettings.IAPItem_FullTraining_ProductLicensed)
            //{
            //    //visualizzazione solo animali sbloccati
            //    foreach (var item in _groups.SelectMany(g => g.Items).Where(it => !it.IsLocked))
            //    {
            //        Items.Add(new ItemViewModel(item));
            //    }
            //}
            //else
            //{
            //    //visualizzazione di tutti gli animali
            //    foreach (var item in _groups.SelectMany(g => g.Items))
            //    {
            //        Items.Add(new ItemViewModel(item));
            //    }
            //}

            foreach (var item in _groups.SelectMany(g => g.Items))
            {
                Items.Add(new ItemViewModel(item));
            }

            var listDs = new LoopingListDataSource(Items.Count());
            listDs.ItemNeeded -= _listDs_ItemNeeded;
            listDs.ItemNeeded += _listDs_ItemNeeded;
            listDs.ItemUpdated -= _listDs_ItemUpdated;
            listDs.ItemUpdated += _listDs_ItemUpdated;
            ListDs = listDs;
        }

        void _listDs_ItemUpdated(object sender, LoopingListDataItemEventArgs e)
        {
            if (e.Index > Items.Count)
            {
                e.Item = null;
                return;
            }

            var newEl = Items.ElementAt(e.Index);
            if (newEl != null)
            {
                (e.Item as PictureLoopingItem).Text = LocalizedResources.ResourceManager.GetString((newEl as ItemViewModel).Key);
                (e.Item as PictureLoopingItem).DataContext = (newEl as ItemViewModel);
            }
        }

        void _listDs_ItemNeeded(object sender, LoopingListDataItemEventArgs e)
        {
            if (e.Index > Items.Count)
            {
                e.Item = null;
                return;
            }

            var newEl = Items.ElementAt(e.Index);
            if (newEl != null)
            {
                PictureLoopingItem item = null;
                item = new PictureLoopingItem()
                {
                    Text = LocalizedResources.ResourceManager.GetString((newEl as ItemViewModel).Key),
                    DataContext = (newEl as ItemViewModel)
                };
                e.Item = item;
            }
        }

        private object GotoHomepage()
        {
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Homepage);
            return null;
        }

        private object NavigateToSelectedItemCommand()
        {
            if (((App)Application.Current).IsTrial)
            {
                MessagingHelper.GetInstance().CurrentDispatcher.BeginInvoke(() => MyMsgbox.Show(CurrentPage,
                                                                                                MsgboxMode.YesNo,
                                                                                                LocalizedResources.Trial_GalleryAvailableOnlyIntoFullVersion,
                                                                                                result =>
                                                                                                {
                                                                                                    switch (result)
                                                                                                    {
                                                                                                        case MsgboxResponse.Yes:
                                                                                                            var marketplaceDetailTask = new MarketplaceDetailTask { ContentIdentifier = AppSettings.AppGuid };
                                                                                                            marketplaceDetailTask.Show();
                                                                                                            break;
                                                                                                        case MsgboxResponse.No:
                                                                                                            break;
                                                                                                    }
                                                                                                }));
                return null; //need to restart app
            }

            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Painter);
            return null;
        }

        //        private async Task GetAllItemsTrainingPack()
        //        {
        //            string res = null;
        //            try
        //            {
        //#if DEBUG
        //                res = await MockIAPLib.CurrentApp.RequestProductPurchaseAsync(AppSettings.IAPItem_FullTraining_ProductId, false);
        //#else
        //                res = await Windows.ApplicationModel.Store.CurrentApp.RequestProductPurchaseAsync(AppSettings.IAPItem_FullTraining_ProductId, true);
        //#endif
        //            }
        //            catch (Exception)
        //            {
        //                //capita anche se l'utente fa "Annulla" sull'acquisto
        //                res = null;
        //            }
        //            if (res == null)
        //            {
        //                //acquisto KO
        //                return;
        //            }
        //            else
        //            {
        //                //acquisto OK
        //                FullTrainingPackAvailable = true;
        //                AppSettings.IAPItem_FullTraining_ProductLicensed = true;
        //                InitGalleryItems();
        //            }
        //        }
    }
}
