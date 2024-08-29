using Caro.Models.Enums;
using Caro.Models.GameStrategy;
using Caro.ViewModels.Sound;
using Caro.ViewModels.Theme;
using Caro.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caro.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public BoardViewModel BoardViewModel { get; }       = new BoardViewModel();
        public ThemeViewModel ThemeViewModel { get; set; }  = new ThemeViewModel();

        public SoundViewModel SoundViewModel { get; set; }  = new SoundViewModel();

        public int      BoardRatio      { get; set; } = 9;
        public Mode     Mode            { get; set; } = Mode.PVP;
        public AILevel  AILevel         { get; set; } = AILevel.Easy;

        private CellState FirstTurn     { get; set; } = CellState.O;

        public MainViewModel() { }

        public void RunMainWindow(int _boardRatio,  Mode _mode, AILevel _aiLevel)
        {
            BoardRatio = _boardRatio;
            BoardViewModel.RunBoardGame(BoardRatio, _mode, FirstTurn, _aiLevel);
        }

        public void SetMainWindowForBoardViewModel(MainWindow mainWindow)
        {
            BoardViewModel.MainWindow = mainWindow;
        }
    }
}
