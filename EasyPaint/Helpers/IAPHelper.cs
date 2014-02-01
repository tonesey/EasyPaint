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
        }
    }
}
