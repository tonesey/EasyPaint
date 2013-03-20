using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace EasyPaint.ViewModels
{
   public class PicViewModel : ViewModelBase
    {
        private Uri imageSource;
        private Uri imageThumbnailSource;
        //private string title;
        //private string information;
        //private string group;

        /// <summary>
        /// Gets or sets the image source.
        /// </summary>
        public Uri ImageSource
        {
            get
            {
                return this.imageSource;
            }
            set
            {
                if (this.imageSource != value)
                {
                    this.imageSource = value;
                    this.OnPropertyChanged("ImageSource");
                }
            }
        }

        /// <summary>
        /// Gets or sets the image thumbnail source.
        /// </summary>
        public Uri ImageThumbnailSource
        {
            get
            {
                return this.imageThumbnailSource;
            }
            set
            {
                if (this.imageThumbnailSource != value)
                {
                    this.imageThumbnailSource = value;
                    this.OnPropertyChanged("ImageThumbnailSource");
                }
            }
        }


        public string CharacterId { get; set; }

        public string FileName { get; set; }
    }
}
