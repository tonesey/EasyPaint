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
    public class PainterPageViewModel : AppViewModel
    {

        public PainterPageViewModel()
        {
            //RaisePropertyChanged("DrawingboardWidth");
        }

        public int DrawingboardWidth
        {
            get { return 400; }
        }

        public int DrawingboardHeigth
        {
            get { return 400; }
        }

        public int PaletteItemTranslateX
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic)
                {
                    return 0;
                }

                return -85;
            }
        }

        public int CountDownTranslateY
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic)
                {
                    return 0;
                }
                return -150;
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
                return -240;
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
                return -330;
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