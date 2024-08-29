using Caro.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caro.ViewModels.Sound
{
    public class SoundViewModel : BaseViewModel
    {
        private IniFile iniFile { get; set; }
        private bool _playSound = false; 

        public bool PlaySound
        {
            get => _playSound;
            set
            {
                _playSound = value;
                OnPropertyChanged();
            }
        }


        public SoundViewModel()
        {
            iniFile = new IniFile("config.ini");
            if (iniFile.GetDataFromSectionAndKey("Sound", "IsOpen") == "1")
            {
                PlaySound = true;
            }
            else PlaySound = false; 
        }
    }
}
