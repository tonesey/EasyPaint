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
    class AppSettings
    {
        public static string AppRes = "lres";

        public static bool IsDataLoading { get; set; }

        #region user settings
        public const string UserScoreKey = "USER_SCORE";
        public static string UserScoreValue = string.Empty;
        #endregion

        #region global app settings
        public static string AppVersion;
        public static string AppName;

        public static string IAPItem_FullTraining_ProductId { get; set; }
        public static bool IAPItem_FullTraining_ProductLicensed { get; set; }

        public static bool AlreadyAskedRating { get; set; }

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
        }

        //public async static Task LoadAppSettingsAsync()
        //{
        //    if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
        //    {
        //        await LoadAppAttributesAsync();
        //        LoadUserSettings();
        //    }
        //}

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
            UserScoreValue = StorageHelper.GetSetting<string>(UserScoreKey);
        }

        private static async Task LoadAppAttributesAsync()
        {
            //global app settings
            var appEl = XDocument.Load("WMAppManifest.xml").Root.Element("App");
            var ver = new Version(appEl.Attribute("Version").Value);
            AppVersion = string.Format("{0}.{1}.{2}", ver.Major, ver.Minor, ver.Build);
            AppName = appEl.Attribute("Title").Value;

            await CheckIAPLicenseInfosAsync();
        }

        private static async Task CheckIAPLicenseInfosAsync()
        {
            try
            {
#if DEBUG
#if UNLOCKED
                IAPItem_FullTraining_ProductLicensed = true;
                //IAPItem_ContinentsUnlocker_ProductLicensed = true;
#else

                try
                {
                    if (MockIAPLib.CurrentApp.LicenseInformation.ProductLicenses.ContainsKey(Constants.IAPItem_FullTraining))
                    {
                        IAPItem_FullTraining_ProductLicensed = MockIAPLib.CurrentApp.LicenseInformation.ProductLicenses[Constants.IAPItem_FullTraining].IsActive;
                    }
                }
                catch (Exception)
                {
                }

                //try
                //{
                //    if (MockIAPLib.CurrentApp.LicenseInformation.ProductLicenses.ContainsKey(Constants.IAPItem_ContinentsUnlocker))
                //    {
                //        IAPItem_ContinentsUnlocker_ProductLicensed = MockIAPLib.CurrentApp.LicenseInformation.ProductLicenses[Constants.IAPItem_ContinentsUnlocker].IsActive;
                //    }
                //}
                //catch (Exception)
                //{
                //}
#endif
#else
                try
                {
                    if (Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses.ContainsKey(Constants.IAPItem_FullTraining))
                    {
                        IAPItem_FullTraining_ProductLicensed = Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses[Constants.IAPItem_FullTraining].IsActive;
                    }
                }
                catch (Exception)
                {
                }

                //try
                //{
                //    if (Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses.ContainsKey(Constants.IAPItem_ContinentsUnlocker))
                //    {
                //        IAPItem_ContinentsUnlocker_ProductLicensed = Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses[Constants.IAPItem_ContinentsUnlocker].IsActive;
                //    }
                //}
                //catch (Exception)
                //{
                //}
#endif
                if (!IAPItem_FullTraining_ProductLicensed)
                {
#if DEBUG
                    MockIAPLib.ListingInformation li = await MockIAPLib.CurrentApp.LoadListingInformationAsync(); ;
#else
                    Windows.ApplicationModel.Store.ListingInformation li = await Windows.ApplicationModel.Store.CurrentApp.LoadListingInformationAsync(); ;
#endif
                    //try
                    //{
                    //    IAPItem_ContinentsUnlocker_ProductId = li.ProductListings[Constants.IAPItem_ContinentsUnlocker].ProductId;
                    //}
                    //catch (Exception)
                    //{
                    //}

                    try
                    {
                        IAPItem_FullTraining_ProductId = li.ProductListings[Constants.IAPItem_FullTraining].ProductId;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
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
                UserScoreValue = AppDataManager.GetInstance().GetUserScoreStrValue();
            }
            StorageHelper.StoreSetting(UserScoreKey, UserScoreValue, true);
            // StorageHelper.StoreSetting(SoundOnKey, SoundOnValue, true);
        }


        public static bool? ExternalMusicAllowed { get; set; }
    }
}
