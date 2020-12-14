using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TimsWpfControls
{
    public static class ThemingHelper
    {
        static Dictionary<Color?, string> _AccentColorNamesDictionary;
        public static Dictionary<Color?, string> AccentColorNamesDictionary
        {
            get
            {
                if (_AccentColorNamesDictionary == null)
                {
                    _AccentColorNamesDictionary = new Dictionary<Color?, string>();
                    var rm = new ResourceManager(typeof(Lang.AccentColorNames));
                    var resourceSet = rm.GetResourceSet(CultureInfo.CurrentCulture, true, true);
                    foreach (var entry in resourceSet.OfType<DictionaryEntry>())
                    {
                        try
                        {
                            var color = (Color)ColorConverter.ConvertFromString(entry.Key.ToString());
                            _AccentColorNamesDictionary.Add(color, entry.Value.ToString());
                        }
                        catch (Exception)
                        {
                            Console.WriteLine(entry.Key.ToString() + " is not a valid color-key");
                        }
                    }
                }
                return _AccentColorNamesDictionary;
            }
            set { _AccentColorNamesDictionary = value; }
        }


        static ObservableCollection<Color?> _AccentColorsPalette;
        public static ObservableCollection<Color?> AccentColorsPalette
        {
            get
            {
                if (_AccentColorsPalette == null)
                {
                    _AccentColorsPalette = new ObservableCollection<Color?>(
                        AccentColorNamesDictionary
                        .OrderBy(x => x.Value)
                        .Select(x => x.Key));

                    return _AccentColorsPalette;
                }
                return _AccentColorsPalette;
            }
        }
    }
}
