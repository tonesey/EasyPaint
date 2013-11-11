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
    public class PainterPageViewModel : ViewModelBase
    {

        /// <summary>
        /// Initializes a new instance of the HomepageViewModel class.
        /// </summary>
        public PainterPageViewModel()
        {
        }

        public int DrawingboardWidth
        {
            get { return 560; }
        }

        public int DrawingboardHeigth
        {
            get { return 460; }
        }

        public int PaletteItemTranslateX
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic)
                {
                    return 0;
                }

                return -65;
            }
        }

        public int ToolBoxItem1TranslateY
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic)
                {
                    return 0;
                }
                return -220;
            }
        }
        public int ToolBoxItem2TranslateY
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic)
                {
                    return 0;
                }
                return -303;
            }
        }
        public int ToolBoxItem3TranslateY
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic)
                {
                    return 0;
                }
                return -380;
            }
        }
    }
}