using Caro.Models.Enums;
using Caro.Models.GameStrategy;
using Caro.ViewModels.Sound;
using Caro.ViewModels.Theme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Caro.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        public ICommand PVPCommand      { get; set; }
        public ICommand PVECommand      { get; set; }
        public ICommand LANCommand      { get; set; }
        public ICommand ExitCommand     { get; set; }

        public MainViewModel    MainViewModel   { get; set; }
        public ThemeViewModel   ThemeViewModel  { get; set; }

        public SoundViewModel   SoundViewModel  { get; set; }

        public TcpClient? tcpClient;

        MainWindow mainWindow = new MainWindow();

        public List<string> ListBoardSize { get; } = new List<string>()
        {
            "9x9", "12x12", "15x15", "20x20"
        };

        public List<string> ListBotMode { get; } = new List<string>()
        {
            "Easy", "Hard"
        };

        private string _boardSize = String.Empty;
        private int _size = 0; 
        public string BoardSize
        {
            get => _boardSize;
            set
            {
                _boardSize = value;
                if      (_boardSize == "9x9")   _size = 9;
                else if (_boardSize == "12x12") _size = 12;
                else if (_boardSize == "15x15") _size = 15;
                else if (_boardSize == "20x20") _size = 20;
                OnPropertyChanged();
            }
        }

        private string _botMode = String.Empty;
        private AILevel aiLevel = AILevel.Easy;
        public string BotMode
        {
            get => _botMode;
            set
            {
                _botMode = value;
                if (_botMode == "Easy") aiLevel = AILevel.Easy;
                else if (_botMode == "Hard") aiLevel = AILevel.Hard;
                OnPropertyChanged();
            }
        }


        public MenuViewModel()
        {
            BoardSize       = ListBoardSize[3];
            BotMode = ListBotMode[0];
            MainViewModel   = new MainViewModel();
            ThemeViewModel  = new ThemeViewModel();
            SoundViewModel  = new SoundViewModel();

            PVPCommand      = new RelayCommand<Window>(OnPVPButton);
            PVECommand      = new RelayCommand<Window>(OnPVEButton);
            LANCommand      = new RelayCommand<Window>(OnLANButton);
            ExitCommand     = new RelayCommand<Window>(OnExitButton);

        }

        public void OnPVPButton(Window obj)
        {
            mainWindow = new MainWindow();
            mainWindow.DataContext = MainViewModel;
            MainViewModel.SetMainWindowForBoardViewModel(mainWindow);
            MainViewModel.RunMainWindow(_size, Mode.PVP, AILevel.Hard);
            
            mainWindow.Show();
            obj.Hide();
            mainWindow.Closed += (s, e) =>
            {
                obj.Show();
            };
        }

        public void OnPVEButton(Window obj)
        {
            mainWindow = new MainWindow();
            mainWindow.DataContext = MainViewModel;
            MainViewModel.SetMainWindowForBoardViewModel(mainWindow);
            MainViewModel.RunMainWindow(_size, Mode.PVE, aiLevel);

            mainWindow.Show();
            obj.Hide();
            mainWindow.Closed += (s, e) =>
            {
                obj.Show();
            };
        }


        public void OnLANButton(Window obj)
        {
            mainWindow = new MainWindow();
            mainWindow.DataContext = MainViewModel;
            MainViewModel.SetMainWindowForBoardViewModel(mainWindow);
            MainViewModel.RunMainWindow(_size, Mode.LAN, AILevel.Easy);
            
            mainWindow.Show();
            obj.Hide();
            mainWindow.Closed += (s, e) =>
            {
               obj.Show();
            };
        }

        
        public void OnExitButton(Window window)
        {
            if (window == null) return;
            window.Close();
        }
    }
}
