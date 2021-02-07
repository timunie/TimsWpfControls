using ControlzEx.Theming;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using TimsWpfControls.Model;

namespace TimsWpfControls_Demo.Model
{
    public class MainViewModel : ObservableObject
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


        public static List<Person> People { get; } = new List<Person>()
        {
            new Person(){ FirstName="Donald", LastName="Duck", Age=-1 },
            new Person(){ FirstName="Tim", LastName="U", Age=32 },
            new Person(){ FirstName="Person 1", LastName="Person A", Age=5 },
        };



        #region ColorPicker
        public List<Color> TintedColors
        {
            get
            {
                var list = new List<Color>
                {
                    // Add GrayScale
                    Colors.Transparent
                };
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
            set { _AccentColor = value; OnPropertyChanged(nameof(AccentColor)); }
        }


        private Color _HighlightColor = Colors.Orange;

        public Color HighlightColor
        {
            get { return _HighlightColor; }
            set { _HighlightColor = value; OnPropertyChanged(nameof(HighlightColor)); }
        }


        public ReadOnlyObservableCollection<string> BaseThemes => ThemeManager.Current.BaseColors;
        private string _BaseTheme = "Light";

        public string BaseTheme
        {
            get { return _BaseTheme; }
            set { _BaseTheme = value; OnPropertyChanged(nameof(BaseTheme)); ChangeAppTheme(); }
        }

        private bool _UseSolidAccent;
        public bool UseSolidAccent
        {
            get { return _UseSolidAccent; }
            set { _UseSolidAccent = value; OnPropertyChanged(nameof(UseSolidAccent)); ChangeAppTheme(); }
        }


        internal void ChangeAppTheme()
        {
            RuntimeThemeGenerator.Current.Options.UseHSL = UseSolidAccent;

            Theme newTheme = new Theme("Custom",
                                       "Custom",
                                       BaseTheme,
                                       AccentColor.ToString(),
                                       AccentColor,
                                       new SolidColorBrush(AccentColor),
                                       true,
                                       false);

            newTheme.Resources["MahApps.Colors.Highlight"] = HighlightColor;
            newTheme.Resources["MahApps.Brushes.Highlight"] = new SolidColorBrush(HighlightColor);

            ThemeManager.Current.ChangeTheme(App.Current, newTheme);
        }

        #endregion ThemeMananger

        public static List<Color> AccentColors = new List<Color>()
        {
            (Color)App.Current.Resources["MahApps.Colors.Accent"],
            (Color)App.Current.Resources["MahApps.Colors.Accent2"],
            (Color)App.Current.Resources["MahApps.Colors.Accent3"],
            (Color)App.Current.Resources["MahApps.Colors.Accent4"],
            (Color)App.Current.Resources["MahApps.Colors.Highlight"],
        };

    }
}