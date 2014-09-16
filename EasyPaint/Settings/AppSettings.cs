using EasyPaint.Data;
using EasyPaint.Helpers;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Wp8Shared.Helpers;

namespace EasyPaint.Settings
{

    public enum GameLevel
    {
        Easy,
        Medium,
        Hard
    }

    class AppSettings
    {
        public const string AppGuid = "c2e057e9-1b3c-4a13-b722-ad744c5d7ddf";
        
        public static string AppRes = "lres";

        public static bool IsDataLoading { get; set; }

        #region user settings

        //HARD
        public const string UserScoreKey_HARD = "USER_SCORE"; //USER_SCORE maintenied for backward compatibility
        public static string UserScoreValue_HARD = string.Empty;

        public const string RecordScoreKey_HARD = "RECORD_SCORE"; //RECORD_SCORE maintenied for backward compatibility
        public static long RecordScoreValue_HARD = 0;

        //MEDIUM
        public const string UserScoreKey_MEDIUM = "USER_SCORE_MEDIUM";
        public static string UserScoreValue_MEDIUM = string.Empty;

        public const string RecordScoreKey_MEDIUM = "RECORD_SCORE_MEDIUM";
        public static long RecordScoreValue_MEDIUM = 0;

        //EASY
        public const string UserScoreKey_EASY = "USER_SCORE_EASY";
        public static string UserScoreValue_EASY = string.Empty;

        public const string RecordScoreKey_EASY = "RECORD_SCORE_EASY";
        public static long RecordScoreValue_EASY = 0;
        #endregion

        #region global app settings
        public static string AppVersion;
        public static string AppName;
        public static string SupportEmailAddress = "centapp@hotmail.com";

        public static string IAPItem_FullTraining_ProductId { get; set; }
        public static bool IAPItem_FullTraining_ProductLicensed { get; set; }

        public static bool AlreadyAskedRating { get; set; }
        public static GameLevel GameLevel { get; set; }

        //public static string IAPItem_ContinentsUnlocker_ProductId { get; set; }
        //public static bool IAPItem_ContinentsUnlocker_ProductLicensed { get; set; }

        public static bool FreePlayingMode
        {
            get
            {
                return IAPItem_FullTraining_ProductLicensed;
                //return IAPItem_FullTraining_ProductLicensed || IAPItem_ContinentsUnlocker_ProductLicensed;
                //return !IAPItem_ContinentsUnlocker_ProductLicensed;
            }
        }

        #endregion

        //public const string SoundOnKey = "SOUND_ON";
        //public static bool SoundOnValue = false;

        public AppSettings()
        {
            ExternalMusicAllowed = null;
            GameLevel = GameLevel.Easy;
        }

        public static void LoadAppSettings()
        {
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                var task = Task.Run(async () => { await LoadAppAttributesAsync(); });
                task.Wait();

                LoadUserSettings();
            }
        }

        public static void LoadUserSettings()
        {
            //user settings
            UserScoreValue_HARD = StorageHelper.GetSetting<string>(UserScoreKey_HARD);
            RecordScoreValue_HARD = StorageHelper.GetSetting<long>(RecordScoreKey_HARD, 0);

            UserScoreValue_MEDIUM = StorageHelper.GetSetting<string>(UserScoreKey_MEDIUM);
            RecordScoreValue_MEDIUM = StorageHelper.GetSetting<long>(RecordScoreKey_MEDIUM, 0);

            UserScoreValue_EASY = StorageHelper.GetSetting<string>(UserScoreKey_EASY);
            RecordScoreValue_EASY = StorageHelper.GetSetting<long>(RecordScoreKey_EASY, 0);
        }

        private static async Task LoadAppAttributesAsync()
        {
            //global app settings
            var appEl = XDocument.Load("WMAppManifest.xml").Root.Element("App");
            var ver = new Version(appEl.Attribute("Version").Value);
            AppVersion = string.Format("{0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
            AppName = appEl.Attribute("Title").Value;

            //await CheckIAPLicenseInfosAsync();
        }

        //public static async Task SaveSettings(bool rebuildData)
        public static void SaveSettings(bool rebuildData)
        {
            if (IsDataLoading)
            {
                return;
            }

            if (ViewModelBase.IsInDesignModeStatic)
            {
                return;
            }

            if (rebuildData)
            {
                var pointsVal = AppDataManager.GetInstance().GetUserScoreStrValue();
                switch (AppSettings.GameLevel)
                {
                    case GameLevel.Easy:
                        UserScoreValue_EASY = pointsVal;
                        StorageHelper.StoreSetting(UserScoreKey_EASY, UserScoreValue_EASY, true);
                        break;
                    case GameLevel.Medium:
                        UserScoreValue_MEDIUM = pointsVal;
                        StorageHelper.StoreSetting(UserScoreKey_MEDIUM, UserScoreValue_MEDIUM, true);
                        break;
                    case GameLevel.Hard:
                        UserScoreValue_HARD = pointsVal;
                        StorageHelper.StoreSetting(UserScoreKey_HARD, UserScoreValue_HARD, true);
                        break;
                }
            }

            long totalPoints = AppDataManager.GetInstance().GetTotalPoints();

            switch (AppSettings.GameLevel)
            {
                case GameLevel.Easy:
                    if (totalPoints > RecordScoreValue_EASY)
                    {
                        RecordScoreValue_EASY = totalPoints;
                        StorageHelper.StoreSetting(RecordScoreKey_EASY, RecordScoreValue_EASY, true);
                    }
                    break;
                case GameLevel.Medium:
                    if (totalPoints > RecordScoreValue_MEDIUM)
                    {
                        RecordScoreValue_MEDIUM = totalPoints;
                        StorageHelper.StoreSetting(RecordScoreKey_MEDIUM, RecordScoreValue_MEDIUM, true);
                    }
                    break;
                case GameLevel.Hard:
                    if (totalPoints > RecordScoreValue_HARD)
                    {
                        RecordScoreValue_HARD = totalPoints;
                        StorageHelper.StoreSetting(RecordScoreKey_HARD, RecordScoreValue_HARD, true);
                    }
                    break;
            }

            // StorageHelper.StoreSetting(SoundOnKey, SoundOnValue, true);
        }


        public static bool? ExternalMusicAllowed { get; set; }
    }
}
