using EasyPaint.Data;
using EasyPaint.Model;
using EasyPaint.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Wp7Shared.Helpers;

namespace EasyPaint.Helpers
{
    public class ModelHelper
    {

        public static AppData BuildAppData()
        {

            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream("EasyPaint.cfg.xml");
            var doc = XDocument.Load(stream);

//#if DEBUG
//            string userScoreDebug = "canguro colore.png-0;clamidosauro colori.png-50;coccodrillo colore.png-1";
//            StorageHelper.StoreSetting(AppSettings.UserScoreKey, userScoreDebug, true);
//#endif

            var userScoreValue = AppSettings.UserScoreValue;

            #region first start items unlock
            if (string.IsNullOrEmpty(userScoreValue)) 
            {
                string tmp = string.Empty;
                var defaultUnlockedItems = doc.Element("root").Element("unlocked_items").Attribute("value").Value.Split(',');
                for (int i = 0; i < defaultUnlockedItems.Length; i++)
                {
                    var item = defaultUnlockedItems[i];
                    tmp += string.Format("{0}-{1}", item, Item.MINIMUM_UNLOCK_PERCENTAGE_REQUIRED);
                    if (i < defaultUnlockedItems.Length - 1) {
                        tmp += ";";
                    }
                }
                AppSettings.UserScoreValue = tmp;
                AppSettings.SaveSettings();
                AppSettings.LoadSettings();
            }
            #endregion

            //loading dictionary with <filename-score> t-uple
            Dictionary<string, int> _userScore = new Dictionary<string,int>();
            string userScore = AppSettings.UserScoreValue;
            string[] spl = userScore.Split(';');
            foreach (var item in spl)
	        {
		        string[] spl1 = item.Split('-');
                _userScore.Add(spl1[0], int.Parse(spl1[1]));
	        }

            #region cfg data
            AppData data = new AppData();

            List<Group> groups = new List<Group>();

            string uiMode = doc.Element("root").Element("ui").Attribute("mode").Value;

            AppSettings.AppRes = uiMode;

            groups = new List<Group>();
            foreach (XElement element in doc.Element("root").Elements("group"))
            {
                Group g = new Group();
                g.Id = element.Attribute("id").Value;
                g.Key = element.Attribute("key").Value;
                g.ImgFilename = element.Attribute("imgname").Value;
                if (element.Element("items") != null)
                {
                    foreach (var itemNode in element.Element("items").Elements("item"))
                    {
                        Item p = new Item();
                        p.ImgFilename = itemNode.Attribute("imgname").Value;
                        var userScoreItem = _userScore.Keys.FirstOrDefault(fn => fn == p.ImgFilename);
                        if (userScoreItem != null) {
                            p.UserMaximumScore = _userScore[userScoreItem];
                        }
                        p.Key = itemNode.Attribute("key").Value;
                        g.Items.Add(p);
                    }
                    groups.Add(g);
                }
            }
            data.Groups = groups;
            data.UIMode = uiMode;
            #endregion
            
            return data;
        }

    }
}
