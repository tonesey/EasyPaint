using EasyPaint.Messages;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace EasyPaint.Helpers
{

    //http://blog.galasoft.ch/posts/2011/01/navigation-in-a-wp7-application-with-mvvm-light/


    class NavigationServiceHelper
    {
        public Dispatcher CurrentDispatcher { get; set; }
        public NavigationService CurrentNavigationService { get; set; }

        bool _messagesRegistered = false;

        private static NavigationServiceHelper _curInstance = null;
        
        internal static NavigationServiceHelper GetInstance()
        {
            if (_curInstance == null)
            {
                _curInstance = new NavigationServiceHelper();
            }
            return _curInstance;
        }
    
        internal void RegisterMessages()
        {
            if (!_messagesRegistered)
            {
                Messenger.Default.Register<GoToPageMessage>(this, (action) => ReceiveMessage(action));
                Messenger.Default.Register<ToggleSoundMessage>(this, (action) => ReceiveMessage(action));
                _messagesRegistered = true;
            }
        }

        private object ReceiveMessage(BaseMessage action)
        {
            if (action is GoToPageMessage)
            {
                GenericHelper.Navigate(CurrentNavigationService, CurrentDispatcher, (action as GoToPageMessage).PageName);
                return null;
            }
            else if (action is ToggleSoundMessage)
            {
                App.Current.ToggleIsMute();
            }
            return null;
        }
    }
}
