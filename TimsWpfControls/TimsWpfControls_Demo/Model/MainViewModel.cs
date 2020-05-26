using ControlzEx.Theming;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using TimsWpfControls.Model;

namespace TimsWpfControls_Demo.Model
{
    public class MainViewModel : BaseClass
    {

        public MainViewModel()
        {
            // Create AccentColorPalette
            foreach (var accent in ThemeManager.Current.Themes.GroupBy(x => x.ColorScheme).OrderBy(a => a.Key).Select(a => a.First()))
            {
                this.AccentColorNamesDictionary.Add(accent.PrimaryAccentColor, "MahApps." + accent.ColorScheme);
            }
        }

        public static List<string> IntellisenseSource { get; } = new List<string>()
        {
            "Hello",
            "World",
            "Welcome",
            "to",
            "my",
            "short",
            "sample"
        };


        #region ColorPicker
        public List<Color> TintedColors
        {
            get
            {
                var list = new List<Color>();

                // Add GrayScale
                list.Add(Colors.Transparent);
                for (int i = 0; i < 23; i++)
                {
                    list.Add(new HSLColor(1, 0, 0, 1 - i / 22d).ToColor());
                }

                // Add fully saturated colors
                for (int i = 0; i < 360; i += 15)
                {
                    list.Add(new HSLColor(1, i, 1, 0.5).ToColor());
                }

                for (double i = 0.9; i > 0.1; i -= 0.1)
                {
                    for (int j = 0; j < 360; j += 15)
                    {
                        list.Add(new HSLColor(1, j, 1, i).ToColor());
                    }
                }

                return list;
            }
        }

        public Dictionary<Color?, string> AccentColorNamesDictionary { get; } = new Dictionary<Color?, string>();
        public IEnumerable<Color?> AccentColorsPalette => AccentColorNamesDictionary.Keys;
        #endregion

        #region ThemeMananger

        private Color _AccentColor = Colors.Blue;

        public Color AccentColor
        {
            get { return _AccentColor; }
            set { _AccentColor = value; RaisePropertyChanged(nameof(AccentColor)); }
        }


        public string AccentColorName
        {
            get { return _AccentColor.ToString(); }
            set
            {
                if (ColorConverter.ConvertFromString(value) is Color color)
                {
                    AccentColor = color;
                    ChangeAppTheme();
                }
                RaisePropertyChanged(nameof(AccentColorName));
            }
        }

        private Color _HighlightColor = Colors.Orange;

        public Color HighlightColor
        {
            get { return _HighlightColor; }
            set { _HighlightColor = value; RaisePropertyChanged(nameof(HighlightColor)); }
        }

        public string HighlightColorName
        {
            get { return HighlightColor.ToString(); }
            set
            {
                if (ColorConverter.ConvertFromString(value) is Color color)
                {
                    HighlightColor = color;
                    ChangeAppTheme();
                }
                RaisePropertyChanged(nameof(HighlightColorName));
            }
        }

        public ReadOnlyObservableCollection<string> BaseThemes => ThemeManager.Current.BaseColors;
        private string _BaseTheme = "Light";

        public string BaseTheme
        {
            get { return _BaseTheme; }
            set { _BaseTheme = value; RaisePropertyChanged(nameof(BaseTheme)); ChangeAppTheme(); }
        }

        internal void ChangeAppTheme()
        {
            Theme newTheme = new Theme("Custom",
                                       "Custom",
                                       BaseTheme,
                                       AccentColorName,
                                       AccentColor,
                                       new SolidColorBrush(AccentColor),
                                       true,
                                       false);

            newTheme.Resources["MahApps.Colors.Highlight"] = HighlightColor;
            newTheme.Resources["MahApps.Brushes.Highlight"] = new SolidColorBrush(HighlightColor);

            ThemeManager.Current.ChangeTheme(App.Current, newTheme);
        }

        #endregion ThemeMananger
    }
}