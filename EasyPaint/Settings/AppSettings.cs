using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wp8Shared.Helpers;

namespace EasyPaint.Settings
{
    class AppSettings
    {
        public static string AppRes = "lres";

        public const string UserScoreKey = "USER_SCORE";
        public static string UserScoreValue = string.Empty;

        public const string SoundOnKey = "SOUND_ON";
        public static bool SoundOnValue = false;

        public AppSettings() { 
        }

        public static void LoadSettings()
        {
            UserScoreValue = StorageHelper.GetSetting<string>(UserScoreKey);
            SoundOnValue = StorageHelper.GetSetting<bool>(SoundOnKey);
        }

        public static void SaveSettings()
        {
            StorageHelper.StoreSetting(UserScoreKey, UserScoreValue, true);
            StorageHelper.StoreSetting(SoundOnKey, SoundOnValue, true);
        }

    }
}
