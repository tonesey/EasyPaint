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
    public partial class ItemSelectorPage : PhoneApplicationPage
    {
        private ItemSelectorViewModel _vm;

        public ItemSelectorPage()
        {
            InitializeComponent();
            _vm = ViewModelLocator.ItemSelectorViewModelStatic;
            LoopingListDataSource ds = new LoopingListDataSource(_vm.Items.Count());
            ds.ItemNeeded += this.OnDs_ItemNeeded;
            ds.ItemUpdated += this.OnDs_ItemUpdated;
            this.loopingList.DataSource = ds;
            this.loopingList.SelectedIndexChanged += this.OnSelectedIndexChanged;
            this.loopingList.SelectedIndex = 0;
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
                (e.Item as PictureLoopingItem).Picture = (newEl as ItemViewModel).ImageSource;
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
                e.Item = new PictureLoopingItem() { Picture = (newEl as ItemViewModel).ImageSource };
            }
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            //this.UpdateCaption();
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<GoToPageMessage>(this, (action) => ReceiveMessage(action));
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

    }
}