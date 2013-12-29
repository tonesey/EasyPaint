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
using Wp8Shared.Helpers;

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
            //            string userScoreDebug = "canguro colore.png-0-0;clamidosauro colori.png-50-100;coccodrillo colore.png-10-45";
            //            StorageHelper.StoreSetting(AppSettings.UserScoreKey, userScoreDebug, true);
            //#endif

            var userScoreValue = AppSettings.UserScoreValue;

            //#region first start items unlock
            //if (string.IsNullOrEmpty(userScoreValue)) 
            //{
            //    string tmp = string.Empty;
            //    var defaultUnlockedItems = doc.Element("root").Element("unlocked_items").Attribute("value").Value.Split(',');
            //    for (int i = 0; i < defaultUnlockedItems.Length; i++)
            //    {
            //        var item = defaultUnlockedItems[i];
            //        tmp += string.Format("{0}-{1}-{2}", item, Item.MINIMUM_UNLOCK_PERCENTAGE_REQUIRED, Item.MINIMUM_UNLOCK_PERCENTAGE_REQUIRED);
            //        if (i < defaultUnlockedItems.Length - 1) {
            //            tmp += ";";
            //        }
            //    }
            //    AppSettings.UserScoreValue = tmp;
            //    AppSettings.SaveSettings();
            //    AppSettings.LoadSettings();
            //}
            //#endregion

            //loading dictionary with <filename-userscore-record> t-uple
            Dictionary<string, int[]> _userScore = new Dictionary<string, int[]>();
            string userScore = AppSettings.UserScoreValue;
            if (!string.IsNullOrEmpty(userScore))
            {
                string[] spl = userScore.Split(';');
                foreach (var item in spl)
                {
                    string[] spl1 = item.Split('-');
                    _userScore.Add(spl1[0], new[] { int.Parse(spl1[1]), int.Parse(spl1[2]) });
                }
            }

            #region cfg data
            AppData data = new AppData();

            List<Group> groups = new List<Group>();

            string uiMode = doc.Element("root").Element("ui").Attribute("mode").Value;

            AppSettings.AppRes = uiMode;

            groups = new List<Group>();
            bool isFirstElement = true;

            //Item prevItem = null;
            foreach (XElement element in doc.Element("root").Elements("group"))
            {
                Group g = new Group();
                g.Id = element.Attribute("id").Value;
                g.Key = element.Attribute("key").Value;
                g.ImgFilename = element.Attribute("imgname").Value;

                var protagonistNode = element.Element("protagonist");
                if (protagonistNode != null)
                {
                    g.ProtagonistImageName = protagonistNode.Attribute("imgname").Value;
                    g.GridRow = protagonistNode.Attribute("Grid.Row") != null ? int.Parse(protagonistNode.Attribute("Grid.Row").Value) : 0;
                    g.GridCol = protagonistNode.Attribute("Grid.Col") != null ? int.Parse(protagonistNode.Attribute("Grid.Col").Value) : 0;
                    g.GridRowSpan = protagonistNode.Attribute("Grid.RowSpan") != null ? int.Parse(protagonistNode.Attribute("Grid.RowSpan").Value) : 1;
                    g.GridColumnSpan = protagonistNode.Attribute("Grid.ColSpan") != null ? int.Parse(protagonistNode.Attribute("Grid.ColSpan").Value) : 1;
                }

                if (element.Element("items") != null)
                {
                    foreach (var itemNode in element.Element("items").Elements("item"))
                    {
                        Item currentItem = new Item();

                        currentItem.ImgFilename = itemNode.Attribute("imgname").Value;
                        var userScoreItem = _userScore.Keys.FirstOrDefault(fn => fn == currentItem.ImgFilename);
                        currentItem.IsLocked = true;
                        if (isFirstElement)
                        { 
                            //first item is always unlocked
                            currentItem.IsLocked = false;
                            isFirstElement = false;
                        }

                        if (userScoreItem != null)
                        {
                            currentItem.Score = _userScore[userScoreItem][0];
                            currentItem.RecordScore = _userScore[userScoreItem][1];
                            currentItem.IsLocked = currentItem.Score < Item.MINIMUM_UNLOCK_PERCENTAGE_REQUIRED;


                        }
                        currentItem.Key = itemNode.Attribute("key").Value;
                        g.Items.Add(currentItem);

                        //currentItem.Prev = prevItem;
                        //if (currentItem.Prev != null)
                        //{
                        //    currentItem.Prev.Next = currentItem;
                        //}
                        //prevItem = currentItem;
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
