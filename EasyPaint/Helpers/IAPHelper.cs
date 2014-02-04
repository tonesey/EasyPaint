using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace EasyPaint.Helpers
{
    class IAPHelper
    {

        //http://developer.nokia.com/community/wiki/Increase_your_revenue_with_In-App_Purchase_on_Windows_Phone_8

        //async void LoadProductListingsByProductIds()
        //{
        //    // First, retrieve the list of some products by their IDs.
        //    ListingInformation listings = await CurrentApp.LoadListingInformationByProductIdsAsync(
        //                                    new string[] { "Bag of 50 gold", "Bag of 100 gold" });
        //    // Then, use the flat list of products as the data source for a
        //    // list box containing data-bound list items.
        //    await CurrentApp.RequestProductPurchaseAsync(productId, false);
        //    CurrentApp.ReportProductFulfillment(license.ProductId);
        //}


        private async static void Test()
        {
            string key = "testitem";
            if (!CurrentApp.LicenseInformation.ProductLicenses[key].IsActive)
            {
                ListingInformation li = await CurrentApp.LoadListingInformationAsync();
                string pID = li.ProductListings[key].ProductId;
                string receipt = await CurrentApp.RequestProductPurchaseAsync(pID, false);
            }


            //if (!CurrentApp.LicenseInformation.ProductLicenses[key].IsActive)
            //{
            //    ListingInformation li = await CurrentApp.LoadListingInformationAsync();
            //    string pID = li.ProductListings[key].ProductId;
            //    string receipt = await CurrentApp.RequestProductPurchaseAsync(pID, false);
            //}

            //#if DEBUG
            //            MockIAPLib.ListingInformation listings = await MockIAPLib.CurrentApp.LoadListingInformationByProductIdsAsync(
            //                                    new string[] { key });
            //#else
            //            ListingInformation listings = await CurrentApp.LoadListingInformationByProductIdsAsync(
            //                                    new string[] { "testitem" });
            //#endif
            //            string receipt = null;
            //            try
            //            {
            //#if DEBUG
            //                receipt = await MockIAPLib.CurrentApp.RequestProductPurchaseAsync(listings.ProductListings.ToList()[0].Value.ProductId, false);
            //#else
            //            receipt = await CurrentApp.RequestProductPurchaseAsync(listings.ProductListings.ToList()[0].Value.ProductId, false);
            //#endif
            //            }
            //            catch (Exception)
            //            {
            //                //item non acquistato
            //                return;
            //            }
        }
    }
}
