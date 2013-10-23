






using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Telerik.Windows.Controls;
using EasyPaint.ViewModel;

namespace EasyPaint
{
    public partial class PicSelectorPage : PhoneApplicationPage
    {
        public PicSelectorPage()
        {
            InitializeComponent();

            this.DataContext = App.ViewModel;
        }

        private void RadDataBoundListBox_ItemTap_1(object sender, Telerik.Windows.Controls.ListBoxItemTapEventArgs e)
        {
            PicViewModel selectedItem = (PicViewModel)(sender as RadDataBoundListBox).SelectedItem;

            App.ViewModel.SelectedPicture = selectedItem;

            NavigationService.Navigate(new Uri("/PainterPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
