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
    public partial class GroupSelectorPage : PhoneApplicationPage
    {
        private GroupSelectorViewModel _vm;

        LoopingListDataSource _listDs = new LoopingListDataSource(0);

        public LoopingListDataSource ListDs
        {
            get { return _listDs; }
        } 

        /// <summary>
        /// Initializes a new instance of the Homepage class.
        /// </summary>
        public GroupSelectorPage()
        {
            InitializeComponent();
            _vm = ViewModelLocator.GroupSelectorViewModelStatic;
            _listDs = new LoopingListDataSource(_vm.Groups.Count());
            _listDs.ItemNeeded += this.OnDs_ItemNeeded;
            _listDs.ItemUpdated += this.OnDs_ItemUpdated;
            this.loopingList.SelectedIndexChanged += this.OnSelectedIndexChanged;
            this.loopingList.SelectedIndex = 0;
        }

        private void OnDs_ItemUpdated(object sender, LoopingListDataItemEventArgs e)
        {
            if (e.Index > _vm.Groups.Count)
            {
                e.Item = null;
                return;
            }
            var newEl = _vm.Groups.ElementAt(e.Index);
            if (newEl != null)
            {
                (e.Item as PictureLoopingItem).Picture = (newEl as GroupViewModel).ImageSource;
                (e.Item as PictureLoopingItem).DataContext = newEl;
            }
        }

        private void OnDs_ItemNeeded(object sender, LoopingListDataItemEventArgs e)
        {
            if (e.Index > _vm.Groups.Count)
            {
                e.Item = null;
                return;
            }
            var newEl = _vm.Groups.ElementAt(e.Index);
            if (newEl != null)
            {
                e.Item = new PictureLoopingItem() { Picture = (newEl as GroupViewModel).ImageSource, DataContext = (newEl as GroupViewModel) };
                
            }
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            //this.UpdateCaption();
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

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _vm.SelectedGroup = (((sender as Image).DataContext as PictureLoopingItem).DataContext as GroupViewModel);
            ViewModelLocator.ItemSelectorViewModelStatic.SetGroupItems(_vm.SelectedGroup.Group);
            _vm.GroupSelectedCommand.Execute(null);
        }

    }
}