using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace EasyPaint.Helpers
{
    class SoundHelper
    {
        public static void PlaySound(SoundEffect soundEffect)
        {
            if (!(Application.Current as App).IsSoundEnabled) return;
            FrameworkDispatcher.Update();
            soundEffect.Play();
        }

        //private static void Play(SoundEffect soundEffect)
        //{
        //}
        //public static System.Windows.Threading.Dispatcher Dispatcher { get; set; }
    }
}
