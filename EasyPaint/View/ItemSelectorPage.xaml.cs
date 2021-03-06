﻿using EasyPaint.Helpers;
using EasyPaint.Messages;
using EasyPaint.Model.UI;
using EasyPaint.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;

namespace EasyPaint.View
{
    public partial class ItemSelectorPage : PhoneApplicationPage
    {
        private ItemSelectorViewModel _vm;
        private Dictionary<string, LoopingListDataItem> _dict = new Dictionary<string, LoopingListDataItem>();

        public ItemSelectorPage()
        {
            InitializeComponent();
            Loaded += ItemSelectorPage_Loaded;
            Unloaded += ItemSelectorPage_Unloaded;
            _dict.Clear();
            InitPage();
        }

        private void InitPage()
        {
            _vm = ViewModelLocator.ItemSelectorViewModelStatic;
            //importante mettere nel costruttore altrimenti non centra la prima immagine!
            LoopingListDataSource ds = new LoopingListDataSource(_vm.Items.Count());
            ds.ItemNeeded += this.OnDs_ItemNeeded;
            ds.ItemUpdated += this.OnDs_ItemUpdated;
            this.loopingList.DataSource = ds;
            this.loopingList.SelectedIndex = 0;
        }

        void ItemSelectorPage_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        void ItemSelectorPage_Loaded(object sender, RoutedEventArgs e)
        {
            AppViewModel.CurrentPage = this;
            MessagingHelper.GetInstance().CurrentDispatcher = Dispatcher;
            //InitPage();
        }

        private void OnDs_ItemUpdated(object sender, LoopingListDataItemEventArgs e)
        {
            if (e.Index > _vm.Items.Count)
            {
                e.Item = null;
                return;
            }

            var newEl = _vm.Items.ElementAt(e.Index);
            if (newEl != null)
            {
                //(e.Item as PictureLoopingItem).Picture = (newEl as ItemViewModel).ImageSource;
                //(e.Item as PictureLoopingItem).IsLocked = (newEl as ItemViewModel).IsLocked;
#if COLORSCHECK
                  (e.Item as PictureLoopingItem).Text = string.Format("{0}({1}%)", LocalizedResources.ResourceManager.GetString((newEl as ItemViewModel).Key), (newEl as ItemViewModel).PaletteCoverage.ToString());
//#else
//                (e.Item as PictureLoopingItem).Text = LocalizedResources.ResourceManager.GetString((newEl as ItemViewModel).Key);
#endif
                (e.Item as PictureLoopingItem).DataContext = (newEl as ItemViewModel);
            }
        }

        private void OnDs_ItemNeeded(object sender, LoopingListDataItemEventArgs e)
        {
            if (e.Index > _vm.Items.Count)
            {
                e.Item = null;
                return;
            }

            var newEl = _vm.Items.ElementAt(e.Index);
            if (newEl != null)
            {

                PictureLoopingItem item = null;
                //if (_dict.ContainsKey((newEl as ItemViewModel).Key))
                //{
                //    item = _dict[(newEl as ItemViewModel).Key] as PictureLoopingItem;
                //}
                //else
                //{
                item = new PictureLoopingItem()
                {
#if COLORSCHECK
                        Text = string.Format("{0} ({1}%)", LocalizedResources.ResourceManager.GetString((newEl as ItemViewModel).Key), (newEl as ItemViewModel).PaletteCoverage.ToString()),
#endif
                    DataContext = (newEl as ItemViewModel)
                };
                //_dict.Add((newEl as ItemViewModel).Key, item);
                //}
                e.Item = item;
            }
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _vm.SelectedItem = (((sender as Grid).DataContext as PictureLoopingItem).DataContext as ItemViewModel);

            if (!_vm.SelectedItem.IsLocked)
            {
                _vm.ItemSelectedCommand.Execute(null);
            }
        }

    }
}