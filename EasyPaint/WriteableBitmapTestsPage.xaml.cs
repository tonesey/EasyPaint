﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using EasyPaint.View;

namespace EasyPaint
{
    public partial class WriteableBitmapTestsPage : PhoneApplicationPage
    {

        Popup _resultPopup = null;
        ResultPopup _resultPopupChild = null;


        private void ShowResultPopup(int percentage)
        {
            _resultPopupChild.Percentage = percentage;
            _resultPopupChild.PageOrientation = Orientation;
            _resultPopup.IsOpen = true;
        }

        private void InitPopup()
        {
            _resultPopup = new Popup();
            ////_resultPopup.Height = Application.Current.Host.Content.ActualHeight;
            ////_resultPopup.Width  = Application.Current.Host.Content.ActualWidth;

            _resultPopup.Height = 500;
            _resultPopup.Width = 400;
            _resultPopupChild = new ResultPopup(_resultPopup);

            _resultPopupChild.PopupClosedEvent -= exportPopup_PopupClosedEvent;
            _resultPopupChild.PopupClosedEvent += exportPopup_PopupClosedEvent;
            _resultPopupChild.ActionPerformedEvent -= exportPopup_ActionPerformedEvent;
            _resultPopupChild.ActionPerformedEvent += exportPopup_ActionPerformedEvent;

            _resultPopup.Child = _resultPopupChild;
        }

        void exportPopup_ActionPerformedEvent(GameAction action)
        {
            //TODO inviare  messaggio 
            MessageBox.Show("TODO");
        }

        void exportPopup_PopupClosedEvent()
        {
        }

        public WriteableBitmapTestsPage()
        {
            InitializeComponent();

            //TestWB();

            InitPopup();

            ShowResultPopup(65);
        }

        private void TestWB()
        {
            int size = 400;

            WriteableBitmap lineart = BitmapFactory.New(size, size).FromResource("Assets/lres/groups/3/canguro lineart.png");
            // var testpixels = lineart.Pixels.Where(p => p != 0);

            //1
            WriteableBitmap userpicture1 = BitmapFactory.New(size, size).FromResource("Assets/testImages/test.png"); //disegno utente
            userpicture1.Blit(
                     new Rect(0, 0, size, size),
                     lineart,
                     new Rect(0, 0, size, size),
                     WriteableBitmapExtensions.BlendMode.Alpha);
            userpicture1.Invalidate();
            ImageTest1.Source = userpicture1;

            ////2 --- OK
            //WriteableBitmap userpicture2 = BitmapFactory.New(size, size).FromResource("Assets/testImages/test.png"); //disegno utente
            //userpicture2.Blit(
            //         new Rect(0, 0, size, size),
            //         lineart,
            //         new Rect(0, 0, size, size),
            //         WriteableBitmapExtensions.BlendMode.Alpha);
            //ImageTest2.Source = userpicture2;

            ////3
            //WriteableBitmap userpicture3 = BitmapFactory.New(size, size).FromResource("Assets/testImages/test.png"); //disegno utente
            //userpicture3.Blit(
            //         new Rect(0, 0, size, size),
            //         lineart,
            //         new Rect(0, 0, size, size),
            //         WriteableBitmapExtensions.BlendMode.ColorKeying);
            //ImageTest3.Source = userpicture3;

            ////4
            //WriteableBitmap userpicture4 = BitmapFactory.New(size, size).FromResource("Assets/testImages/test.png"); //disegno utente
            //userpicture4.Blit(
            //         new Rect(0, 0, size, size),
            //         lineart,
            //         new Rect(0, 0, size, size),
            //         WriteableBitmapExtensions.BlendMode.Mask);
            //ImageTest4.Source = userpicture4;

            ////5
            //WriteableBitmap userpicture5 = BitmapFactory.New(size, size).FromResource("Assets/testImages/test.png"); //disegno utente
            //userpicture5.Blit(
            //         new Rect(0, 0, size, size),
            //         lineart,
            //         new Rect(0, 0, size, size),
            //         WriteableBitmapExtensions.BlendMode.Multiply);
            //ImageTest5.Source = userpicture5;

            ////6
            //WriteableBitmap userpicture6 = BitmapFactory.New(size, size).FromResource("Assets/testImages/test.png"); //disegno utente
            //userpicture6.Blit(
            //         new Rect(0, 0, size, size),
            //         lineart,
            //         new Rect(0, 0, size, size),
            //         WriteableBitmapExtensions.BlendMode.None);
            //ImageTest6.Source = userpicture6;

            ////7
            //WriteableBitmap userpicture7 = BitmapFactory.New(size, size).FromResource("Assets/testImages/test.png"); //disegno utente
            //userpicture7.Blit(
            //         new Rect(0, 0, size, size),
            //         lineart,
            //         new Rect(0, 0, size, size),
            //         WriteableBitmapExtensions.BlendMode.Subtractive);
            //ImageTest7.Source = userpicture7;
        }
    }
}