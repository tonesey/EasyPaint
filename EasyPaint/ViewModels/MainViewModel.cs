using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.Text;
using EasyPaint.Model;
using Telerik.Windows.Controls;
using System.Threading;

namespace EasyPaint.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public List<Character> AllCharacters { get; set; }

        public List<Character> CurrentCultureKnownCharacters {

            get {
                return AllCharacters.Where(c => c.Cultures.Contains("*") || c.Cultures.Contains(Thread.CurrentThread.CurrentUICulture.Name)).ToList();
            }

        }

        public bool IsDataLoaded { get; set; }

        public MainViewModel() {
            IsDataLoaded = false;
        }

        private MyPicture _selectedPicture;
        public MyPicture SelectedPicture
        {
            get
            {
                return _selectedPicture;
            }
            set
            {
                _selectedPicture = value;
                OnPropertyChanged("SelectedPicture");
            }
        }

        private bool _IsBusy = false;
        public bool IsBusy
        {
            get
            {
                return _IsBusy;
            }
            set
            {
                _IsBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        public void LoadData() {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream("EasyPaint.Assets.cfg.xml");
            var doc = XDocument.Load(stream);

            List<Character> chars = new List<Character>();
            foreach (XElement element in doc.Element("root").Element("characters").Elements("item"))
            {
                Character c = new Character();
                c.Id = element.Attribute("id").Value;
                c.Cultures = element.Element("cultures").Value.Split(',').ToList<string>();
               // List<MyPicture> picList = new List<MyPicture>();

                if (element.Element("pics") != null)
                {
                    foreach (var picNode in element.Element("pics").Elements("item"))
                    {
                        MyPicture p = new MyPicture();
                        p.FileName = picNode.Attribute("source").Value;
                        c.Pics.Add(p);
                    }
                    chars.Add(c);
                }
            }

            AllCharacters = chars;
            SelectedPicture = CurrentCultureKnownCharacters.FirstOrDefault(c => c.Pics.Count > 0).Pics.First();
        }


        
    }
}
