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
using System.Collections.ObjectModel;

namespace EasyPaint.ViewModels
{
    public class MainViewModel : MyViewModelBase
    {
        private ObservableCollection<CharacterViewModel> _characters;
        public ObservableCollection<CharacterViewModel> Characters
        {
            get
            {
                return this._characters;
            }
            private set
            {
                this._characters = value;
            }
        }

        public MainViewModel()
        {

            IsDataLoaded = false;

            if (IsInDesignModeStatic)
            {

                ObservableCollection<CharacterViewModel> c = new ObservableCollection<CharacterViewModel>() {
                
                    new CharacterViewModel(new Character() { Id = "baby_looney_toones", 
                                                             FriendlyName = "Baby looney toons",
                                                             Pics = new List<MyPicture>() { 
                                                                new MyPicture() { FileName = "c1.PNG"}, 
                                                                new MyPicture() { FileName = "c2.PNG"},
                                                                new MyPicture() { FileName = "c3.PNG"}
                                                             } } ),
                    new CharacterViewModel(new Character() { Id = "beauty_beast", 
                                                             FriendlyName = "Beauty and Beast",
                                                             Pics = new List<MyPicture>() { 
                                                                new MyPicture() { FileName = "c1.PNG"}, 
                                                                new MyPicture() { FileName = "c2.PNG"},
                                                                new MyPicture() { FileName = "c3.PNG"}
                                                             } } ),
                    new CharacterViewModel(new Character() { Id = "cars", 
                                                             Pics = new List<MyPicture>() { 
                                                                new MyPicture() { FileName = "c1.PNG"}, 
                                                                new MyPicture() { FileName = "c2.PNG"},
                                                                new MyPicture() { FileName = "c3.PNG"}     
                                                             
                                                                                                                     } } )
                };

                _characters = c;
            }

        }

        public ObservableCollection<CharacterViewModel> CurrentCultureKnownCharacters
        {
            get
            {
                return new ObservableCollection<CharacterViewModel>(_characters.Where(c => c.Cultures.Contains("*") || c.Cultures.Contains(Thread.CurrentThread.CurrentUICulture.Name)));
            }
        }

        public bool IsDataLoaded { get; set; }


        private PicViewModel _selectedPicture;
        public PicViewModel SelectedPicture
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

        private void InitializeCharacters(List<Character> chars)
        {
            _characters = new ObservableCollection<CharacterViewModel>();
            foreach (var character in chars)
            {
                CharacterViewModel newItem = new CharacterViewModel(character);
                _characters.Add(newItem);
            }
        }

        public void LoadData()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream("EasyPaint.Assets.cfg.xml");
            var doc = XDocument.Load(stream);

            List<Character> chars = new List<Character>();
            foreach (XElement element in doc.Element("root").Element("characters").Elements("item"))
            {
                Character c = new Character();
                c.Id = element.Attribute("id").Value;
                c.Cultures = element.Element("cultures").Value.Split(',').ToList<string>();
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

            InitializeCharacters(chars);
            SelectedPicture = Characters.First().Pics.Last();
            OnPropertyChanged("Characters");
            IsDataLoaded = true;
        }
    }



}
