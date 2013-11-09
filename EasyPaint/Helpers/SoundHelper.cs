using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPaint.Helpers
{
    class SoundHelper
    {

        public static void PlaySound(SoundEffect soundEffect)
        {
            FrameworkDispatcher.Update();
            soundEffect.Play();
        }

    }
}
