using EasyPaint.Helpers;
using EasyPaint.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace EasyPaint.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class HomepageViewModel : ViewModelBase
    {
        public RelayCommand StartGameCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the HomepageViewModel class.
        /// </summary>
        public HomepageViewModel()
        {
            StartGameCommand = new RelayCommand(() => ExecStartGameCommand());
        }

        private object ExecStartGameCommand()
        {
            //navigation 
            //http://geekswithblogs.net/lbugnion/archive/2011/01/06/navigation-in-a-wp7-application-with-mvvm-light.aspx
            //string navigationString = string.Format("/Info.xaml?appName={0}&appVer={1}&isTrial={2}", new string[] { 
            //    _appName, 
            //    _appVer, 
            //    ((App)Application.Current).IsTrial.ToString()}
            //   );

            var msg = new GoToPageMessage() { PageName = Constants.View_Game };
            Messenger.Default.Send<GoToPageMessage>(msg);
            return null;
        }
    }
}