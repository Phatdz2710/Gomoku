using Caro.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace Caro.ViewModels.Theme
{
    public class ThemeViewModel : BaseViewModel
    {

        private IniFile iniFile;

        private List<string> themeNames = new List<string>();

        private string _currentTheme;

        public string CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;
                    OnPropertyChanged(nameof(CurrentTheme));
                    ChangeTheme(CurrentTheme);
                }
            }
        }
        
        public List<string> ThemeNames
        {
            get => themeNames;
            set
            {
                themeNames = value;
                OnPropertyChanged();
            }
        }

        public ThemeViewModel()
        {
            iniFile = new IniFile("config.ini");
            ThemeNames = iniFile.GetListKeyName("ListTheme");
            _currentTheme = iniFile.GetDataFromSectionAndKey("Theme", "CurrentTheme");
            ChangeTheme(CurrentTheme);
        }

        public void ChangeTheme(string themeName)
        {
            try
            {
                string _themeName = themeName.Replace(" ", "");
                var newTheme = new ResourceDictionary
                {

                    Source = new Uri($"Themes/{_themeName}theme.xaml", UriKind.Relative)
                };

                App.Current.Resources.MergedDictionaries.Clear();
                App.Current.Resources.MergedDictionaries.Add(newTheme);
                OnPropertyChanged();
                iniFile.SetDataForKeyFromSection("Theme", "CurrentTheme", CurrentTheme);
            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot loading theme.", "Error");
            }
        }
    }


}
