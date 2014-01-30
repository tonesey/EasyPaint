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
            Loaded += GroupSelectorPage_Loaded;
            Unloaded += GroupSelectorPage_Unloaded;
            InitPage();
        }

        private void InitPage()
        {
            _vm = ViewModelLocator.GroupSelectorViewModelStatic;
            _listDs = new LoopingListDataSource(_vm.Groups.Count());
            _listDs.ItemNeeded += this.OnDs_ItemNeeded;
            _listDs.ItemUpdated += this.OnDs_ItemUpdated;
            this.loopingList.DataSource = _listDs;
            this.loopingList.SelectedIndex = 0;
        }

        void GroupSelectorPage_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        void GroupSelectorPage_Loaded(object sender, RoutedEventArgs e)
        {
            AppViewModel.CurrentPage = this;
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
                (e.Item as PictureLoopingItem).IsLocked = (newEl as GroupViewModel).IsLocked;
                (e.Item as PictureLoopingItem).Text = LocalizedResources.ResourceManager.GetString((newEl as GroupViewModel).Key);
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
                e.Item = new PictureLoopingItem() { Picture = (newEl as GroupViewModel).ImageSource, 
                                                    IsLocked = (newEl as GroupViewModel).IsLocked,
                                                    Text = LocalizedResources.ResourceManager.GetString((newEl as GroupViewModel).Key),
                                                    DataContext = (newEl as GroupViewModel) };
            }
        }
        
        //private void RegisterMessages()
        //{
        //    //Messenger.Default.Register<GoToPageMessage>(this, (action) => ReceiveMessage(action));
        //    //Messenger.Default.Register<RateAppMessage>(this, (action) => ReceiveMessage(action));
        //    // Messenger.Default.Register<PollCompletedMessage>(this, (action) => ReceiveMessage(action));
        //}

        //private object ReceiveMessage(BaseMessage action)
        //{
        //    if (action is GoToPageMessage)
        //    {
        //        GenericHelper.Navigate(NavigationService, Dispatcher, (action as GoToPageMessage).PageName);
        //        return null;
        //    }
        //    return null;
        //}

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //foreach (JournalEntry item in NavigationService.BackStack.Reverse())
            //{
            //    NavigationService.RemoveBackEntry();
            //}
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _vm.SelectedGroup = (((sender as Grid).DataContext as PictureLoopingItem).DataContext as GroupViewModel);

            if (!_vm.SelectedGroup.IsLocked)
            {
                ViewModelLocator.ItemSelectorViewModelStatic.SetCurrentGroup(_vm.SelectedGroup);
                _vm.GroupSelectedCommand.Execute(null);
            }
        }

    }
}