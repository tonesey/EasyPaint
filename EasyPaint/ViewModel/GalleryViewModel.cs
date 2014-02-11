using EasyPaint.Data;
using EasyPaint.Helpers;
using EasyPaint.Messages;
using EasyPaint.Model;
using EasyPaint.Settings;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace EasyPaint.ViewModel
{
    public class GalleryViewModel : AppViewModel
    {

        private List<Group> _groups = null;

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


        private bool _fullTrainingPackAvailable = false;
        public bool FullTrainingPackAvailable
        {
            get
            {
                return _fullTrainingPackAvailable;
            }
            set
            {
                _fullTrainingPackAvailable = value;
                OnPropertyChanged("FullTrainingPackAvailable");
                OnPropertyChanged("InfoFieldContent");
            }
        }

        public string InfoFieldContent
        {
            get
            {
                return FullTrainingPackAvailable ? LocalizedResources.GalleryPage_InfoText_Purchased : LocalizedResources.GalleryPage_InfoText_NotPurchased;
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
            GetAllItemsTrainingPackCommand = new RelayCommand(() => GetAllItemsTrainingPack());
        }

        private void InitGalleryItems()
        {
            //TODO IAP 
            //- verificare se l'item è licenziato o meno
            //- mettere una prp di Description in Binding e un bool che indica se l'item è stato comprato o meno per cambiare la UI
            //- inizializzare opportunamente la lista, togliendo IsLocked se l'item è stato comprato

            Items = new ObservableCollection<ItemViewModel>();
            FullTrainingPackAvailable = AppSettings.IAPItem_FullTraining_ProductLicensed;

            if (!FullTrainingPackAvailable)
            {
                foreach (var item in _groups.SelectMany(g => g.Items).Where(it => !it.IsLocked))
                {
                    Items.Add(new ItemViewModel(item));
                }
            }
            else
            {
                foreach (var item in _groups.SelectMany(g => g.Items))
                {
                    Items.Add(new ItemViewModel(item));
                }
            }
            OnPropertyChanged("Items");
        }

        private object GotoHomepage()
        {
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Homepage);
            return null;
        }

        private object NavigateToSelectedItemCommand()
        {
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Painter);
            return null;
        }

        private async Task GetAllItemsTrainingPack()
        {
            string res = null;
            try
            {
#if DEBUG
                res = await MockIAPLib.CurrentApp.RequestProductPurchaseAsync(AppSettings.IAPItem_FullTraining_ProductId, false);
#else
                res = await Windows.ApplicationModel.Store.CurrentApp.RequestProductPurchaseAsync(AppSettings.IAPItem_FullTraining_ProductId, true);
#endif
            }
            catch (Exception)
            {
                //capita anche se l'utente fa "Annulla" sull'acquisto
                res = null;
            }

            if (res == null)
            {
                //acquisto KO
                return;
            }
            else
            {
                //acquisto OK
                FullTrainingPackAvailable = true;
                AppSettings.IAPItem_FullTraining_ProductLicensed = true;
                InitGalleryItems();
            }
        }

    }
}
