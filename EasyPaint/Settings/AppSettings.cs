using EasyPaint.Data;
using EasyPaint.Helpers;
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

        public static string IapCompleteGameProductId { get; set; }
        public static bool ProductLicensed { get; set; }
        #endregion

        //public const string SoundOnKey = "SOUND_ON";
        //public static bool SoundOnValue = false;

        public AppSettings()
        {
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
            bool isProductActive = false;
#if DEBUG_
            isProductActive = MockIAPLib.CurrentApp.LicenseInformation.ProductLicenses[Constants.IapCompleteGameItemName].IsActive;
#else
            isProductActive = Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses[Constants.IapCompleteGameItemName].IsActive;
#endif
            if (!isProductActive)
            {
                ProductLicensed = false;
#if DEBUG_
                MockIAPLib.ListingInformation li = await MockIAPLib.CurrentApp.LoadListingInformationAsync(); ;
#else
                Windows.ApplicationModel.Store.ListingInformation li = await Windows.ApplicationModel.Store.CurrentApp.LoadListingInformationAsync(); ;
#endif
                AppSettings.IapCompleteGameProductId = li.ProductListings[Constants.IapCompleteGameItemName].ProductId;
            }
            else
            {
                ProductLicensed = true;
            }
        }

        public static async Task SaveSettings(bool rebuildData)
        {
            if (IsDataLoading)
            {
                return;
            }

            if (rebuildData)
            {
                UserScoreValue = (await AppDataManager.GetInstanceAsync()).GetUserScoreStrValue();
            }
            StorageHelper.StoreSetting(UserScoreKey, UserScoreValue, true);
            // StorageHelper.StoreSetting(SoundOnKey, SoundOnValue, true);
        }

     


    }
}
