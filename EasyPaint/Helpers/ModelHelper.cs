using EasyPaint.Data;
using EasyPaint.Model;
using EasyPaint.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media;
using System.Xml.Linq;
using Wp8Shared.Helpers;

namespace EasyPaint.Helpers
{
    public class ModelHelper
    {

        public static string GetUserScoreValue(AppData appData)
        {
            //loading dictionary with <filename-unlocked-userscore-record> t-uple
            //string userScoreDebug = "canguro colore.png-true-0-0;clamidosauro colori.png-false-50-100;coccodrillo colore.png-false-10-45";
            string strVal = string.Empty;
            var groups = appData.Groups;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < groups.Count; i++)
            {
                for (int j = 0; j < groups.ElementAt(i).Items.Count; j++)
                {
                    var item = groups.ElementAt(i).Items.ElementAt(j);
                    sb.Append(string.Format("{0}-{1}-{2}-{3};", new string[] { item.ImgFilename, item.IsLocked.ToString(), item.Score.ToString(), item.RecordScore.ToString() }));
                }
            }
            return sb.ToString();
        }


        public static AppData BuildAppData()
        {

            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream("EasyPaint.cfg.xml");
            var doc = XDocument.Load(stream);

            //#if DEBUG
            //            string userScoreDebug = "canguro colore.png-true-0-0;clamidosauro colori.png-false-50-100;coccodrillo colore.png-false-10-45";
            //            StorageHelper.StoreSetting(AppSettings.UserScoreKey, userScoreDebug, true);
            //#endif

            #region userscore setting reading
            var userScoreValue = AppSettings.UserScoreValue;
            Dictionary<string, string[]> _userScore = new Dictionary<string, string[]>();
            string userScore = AppSettings.UserScoreValue;
            if (!string.IsNullOrEmpty(userScore))
            {
                string[] spl = userScore.Split(';');
                foreach (var item in spl)
                {
                    string[] spl1 = item.Split('-');
                    if (spl1.Length > 1)
                    {
                        _userScore.Add(spl1[0], new string[] { spl1[1], spl1[2], spl1[3] });
                    }
                }
            }
            #endregion

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

                var itemsElement = element.Element("items");
                if (itemsElement != null)
                {
                    g.SelectorGridRow = itemsElement.Attribute("SelectorGridRow") != null ? int.Parse(itemsElement.Attribute("SelectorGridRow").Value) : 1;
                    g.IsSelectorCentered = itemsElement.Attribute("IsSelectorCentered") != null ? bool.Parse(itemsElement.Attribute("IsSelectorCentered").Value) : true;

                    foreach (var itemNode in itemsElement.Elements("item"))
                    {
                        Item currentItem = new Item();

                        if (itemNode.Attribute("colors") != null)
                        {
                            string[] colArrary = itemNode.Attribute("colors").Value.Split(',');
                            foreach (var item in colArrary)
                            {
                                currentItem.PaletteColors.Add(ImagesHelper.HexStringToColor(item));
                            }
                        }

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
                            currentItem.IsLocked = bool.Parse(_userScore[userScoreItem][0]);
                            currentItem.Score = int.Parse(_userScore[userScoreItem][1]);
                            currentItem.RecordScore = int.Parse(_userScore[userScoreItem][2]);
                            //currentItem.IsLocked = currentItem.Score < Item.MINIMUM_UNLOCK_PERCENTAGE_REQUIRED;
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
