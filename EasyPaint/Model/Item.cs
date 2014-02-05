using EasyPaint.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace EasyPaint.Model
{
    public class Item
    {
        public const int MINIMUM_UNLOCK_PERCENTAGE_REQUIRED = 70;

        public string Key { get; set; }
        public string ImgFilename { get; set; }
        public string LatinName { get; set; }
        public List<Color> PaletteColors { get; set; }
        public Group ParentGroup { get; set; }

        public int _score;
        public int Score
        {
            get
            {
                return _score;
            }
            set
            {
                if (_score != value)
                {
                    _score = value;
                    AppSettings.SaveSettings(true);
                }
            }
        }

        public int _recordScore;
        public int RecordScore
        {
            get
            {
                return _recordScore;
            }
            set
            {
                if (_recordScore != value)
                {
                    _recordScore = value;
                    AppSettings.SaveSettings(true);
                }
            }
        }

        private bool _isLocked = false;
        public bool IsLocked
        {
            get { return _isLocked; }
            set
            {
                if (_isLocked != value)
                {
                    _isLocked = value;
                    AppSettings.SaveSettings(true);
                }
            }
        }

        public Item()
        {
            Key = string.Empty;
            ImgFilename = string.Empty;
            RecordScore = 0;
            PaletteColors = new List<Color>();
        }

        public override string ToString()
        {
            return Key;
        }

    }
}
