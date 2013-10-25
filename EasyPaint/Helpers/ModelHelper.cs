using EasyPaint.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace EasyPaint.Helpers
{
    public class ModelHelper
    {
        public static List<Group> BuildDataFromCfg()
        {
            List<Group> groups = new List<Group>();
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream("EasyPaint.cfg.xml");
            var doc = XDocument.Load(stream);

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
                        p.Key = itemNode.Attribute("key").Value;
                        g.Items.Add(p);
                    }
                    groups.Add(g);
                }
            }
            return groups;
        }

    }
}
