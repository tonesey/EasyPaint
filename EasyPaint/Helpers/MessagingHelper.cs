using EasyPaint.Messages;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Threading;
using Wp8Shared.Helpers;

namespace EasyPaint.Helpers
{
    //alternative
    //http://blog.galasoft.ch/posts/2011/01/navigation-in-a-wp7-application-with-mvvm-light/
    //http://stackoverflow.com/questions/9290269/mvvm-light-toolkit-galasoft-for-wpf-navigation-through-windows

    class MessagingHelper
    {
        public Dispatcher CurrentDispatcher { get; set; }
        public NavigationHelper CurrentNavigationService { get; set; }

        bool _messagesRegistered = false;

        private static MessagingHelper _curInstance = null;
        
        internal static MessagingHelper GetInstance()
        {
            if (_curInstance == null)
            {
                _curInstance = new MessagingHelper();
            }
            return _curInstance;
        }
    
        internal void RegisterMessages()
        {
            if (!_messagesRegistered)
            {
               // Messenger.Default.Register<GoToPageMessage>(this, (action) => ReceiveMessage(action));
                Messenger.Default.Register<ToggleSoundMessage>(this, (action) => ReceiveMessage(action));
                _messagesRegistered = true;
            }
        }

        private object ReceiveMessage(BaseMessage action)
        {
            //if (action is GoToPageMessage)
            //{
            //    GenericHelper.Navigate(CurrentNavigationService, CurrentDispatcher, (action as GoToPageMessage).PageName);
            //    return null;
            //}
            //else 
                
            if (action is ToggleSoundMessage)
            {
                App.Current.ToggleIsMute();
            }
            return null;
        }
    }
}
