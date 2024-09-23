using Caro.Models;
using Caro.Models.Enums;
using Caro.Models.GameStrategy;
using Caro.Models.Player;
using Caro.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Caro.ViewModels
{
    public class BoardViewModel : BaseViewModel
    {
        #region Private Fields
        private readonly Board   _gameBoard      = new Board();
        private ObservableCollection<CellViewModel> _cells = new ObservableCollection<CellViewModel>();
        private int     _boardRatio     = 0;
        private bool    _isPlaying      = true;
        private string  _message        = String.Empty;
        private int     _size;
        private string  _p1PieceInfo    = String.Empty;
        private string  _p2PieceInfo    = String.Empty;
        private bool    _pieceOfPlayer1 = true;
        private bool    _pieceOfPlayer2 = false;
        private CellViewModel? _lastMove;

        private bool    _enableUndoAndNewGame = false;

        public Window?   MainWindow { get; set; }

        private CellState _firstTurn    = CellState.X;

        private Mode ModeGame { get; set; }
        public string[] ListBoardSize { get; } = new string[4] 
        {   
            "9x9", 
            "12x12",
            "15x15", 
            "20x20" 
        };

        private string _boardSize = String.Empty;
        public string BoardSize
        {
            //get => _boardSize;
            set
            {
                //if (_gameBoard.IsBotThinking()) return;

                string _boardSize = value;
                
                int newRatio = 0;

                if      (_boardSize == "9x9")   newRatio = 9;
                else if (_boardSize == "12x12") newRatio = 12;
                else if (_boardSize == "15x15") newRatio = 15;
                else if (_boardSize == "20x20") newRatio = 20;

                ResizeBoard(newRatio);

                OnPropertyChanged();
            }
        }

        #endregion

        #region Properties Binding
        // Message popup after End Game
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }
        // State of Game: Playing or not
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                OnPropertyChanged();
            }
        }

        public bool EnableUndoAndNewGame
        {
            get => _enableUndoAndNewGame;
            set
            {
                _enableUndoAndNewGame = value;
                OnPropertyChanged();
            }
        }
        // Update display Player 1 Score Board
        public string P1Score
        {
            get
            {
                return _p1PieceInfo;
            }
            set
            {
                _p1PieceInfo = _gameBoard.Player1Info();
                Player1Piece = _gameBoard.Player1Turn();
                OnPropertyChanged();
            }
        }

        // Update display Player 2 Score Board
        public string P2Score
        {
            get
            {
                return _p2PieceInfo;
            }
            set
            {
                _p2PieceInfo = _gameBoard.Player2Info();
                Player2Piece = _gameBoard.Player2Turn();
                OnPropertyChanged();
            }
        }

        public bool Player1Piece
        {
            get => _pieceOfPlayer1;
            set
            {
                _pieceOfPlayer1 = value;
                OnPropertyChanged();
            }
        }
        // Binding for Player 2 Score Board
        public bool Player2Piece
        {
            get => _pieceOfPlayer2;
            set
            {
                _pieceOfPlayer2 = value;
                OnPropertyChanged();
            }
        }
        // Board Game
        public ObservableCollection<CellViewModel> Cells
        {
            get => _cells;
            set
            {
                _cells = value;
                OnPropertyChanged();
            }
        }
        // Size of Cell On Board
        public int CellSize
        {
            get => _size; 
            set
            {
                _size = value;
                OnPropertyChanged();
            }
        }
        // Current Player
        public bool CurrentPlayer
        {
            get => _gameBoard.GetCurPlayer();
            set
            {
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands
        public ICommand CellClickCommand    { get; private set; }
        public ICommand NewGameCommand      { get; set; }
        public ICommand UndoCommand         { get; set; }
        public ICommand ExitCommand         { get; set; }

        #endregion

        #region Constructors

        public BoardViewModel()
        {
            CellClickCommand = new RelayCommand<FPoint>(OnCellClick);
            NewGameCommand   = new RelayCommand<object>(OnNewGame);
            UndoCommand      = new RelayCommand<object>(OnUndo);
            ExitCommand      = new RelayCommand<object>(OnExit);
            ModeGame = Mode.PVP;
            _gameBoard.PlayResult   += OnPlayResult;
            _gameBoard.UndoEvent    += OnUndoEvent;
            ReloadScoreBoard();
        }

        public void RunBoardGame(int boardRatio, Mode gameMode, CellState firstTurn, AILevel difficultLevel = AILevel.Hard)
        {
            IsPlaying   = true;
            CreateNewBoard(boardRatio, gameMode, firstTurn, difficultLevel);
            ReloadScoreBoard();
        }

        // Parameters Constructor
       
        #endregion


        #region Methods For Commands
        private void OnCellClick(FPoint pos)
        {
            _gameBoard.PlayAtPointRequest(pos);
        }

        // New game
        private void OnNewGame(object? obj)
        {
            if (_gameBoard.IsBotThinking()) return;
            IsPlaying = true;
            ResetBoard();
            CurrentPlayer = _gameBoard.GetCurPlayer();
        }

        private void OnUndo(object? obj)
        {
            Undo();
        }

        private void OnExit(object? obj)
        {
            //if (_gameBoard.IsBotThinking()) return;
            _gameBoard.ExitGame();
            if (MainWindow == null) 
                return;
            MainWindow.Close();
        }


        #endregion


        #region Event Board

        // Only set this for LANGameStrategy
        private void WaitingForOtherPlayer(object? sender, EventArgs e)
        {
            ResetBoard();
            Task.Run(() => {
                    Message = "Connecting...";
                    while(!_gameBoard.IsReadyToStart())
                    {
                        Thread.Sleep(200);
                        IsPlaying = false;
                    }
                    IsPlaying = true;  
                });
        }

        private void OnPlayResult(object? sender, PlayResult result)
        {
            if (_lastMove != null) _lastMove.IsLastMove = false;
            SetCellContent(result.Position, GetChar_CellState(result.Piece));
            _lastMove = _cells[result.Position.X * _boardRatio + result.Position.Y];
            _lastMove.IsLastMove = true;
            CurrentPlayer = _gameBoard.GetCurPlayer();

            if (result.GameState == GameState.Win)
            {
                HighLightWinCells();
                IsPlaying   = false;
                Message     = GetChar_CellState(result.Piece).ToString() + " WIN";
            }
            else if (result.GameState == GameState.Draw)
            {
                IsPlaying   = false;
                Message     = "DRAW";
            }
        }

        private void OnUndoEvent(object? sender, PlayResult result)
        {
            SetCellContent(result.Position, '\0');
            CurrentPlayer = _gameBoard.GetCurPlayer();
        }

        #endregion

        #region Private Methods

        // Only reset all cell into '\0'
        private void ResetBoard()
        {
            foreach (CellViewModel cell in _cells)
            {
                cell.Content        = '\0';
                cell.IsWinningCell  = false;
                cell.IsLastMove = false;
            }
            _gameBoard.Reset();
            ReloadScoreBoard();
        }

        private void ReloadScoreBoard()
        {
            //For Binding Properties
            P1Score = "";
            P2Score = "";   
        }

        // Resize Board and Reset all cell
        private void ResizeBoard(int newSize)
        {
            _boardRatio = newSize;
            _size       = 600 / _boardRatio;
            Cells.Clear();
            int _sizeContent = CellSize * 2 / 3;
            for (int i = 0; i < _boardRatio; i++)
            {
                for (int j = 0; j < _boardRatio; j++)
                    Cells.Add(new CellViewModel(new FPoint(i, j),
                                                '\0',
                                                _sizeContent));
            }
            _gameBoard.ResizeBoard(_boardRatio);
        }

        private void CreateNewBoard(int boardRatio, Mode gameMode, CellState firstTurn, AILevel aiLevel)
        {
            _boardRatio = boardRatio;
            _size = 600 / _boardRatio;
            Cells.Clear();
            int _sizeContent = CellSize * 2 / 3;
            for (int i = 0; i < _boardRatio; i++)
            {
                for (int j = 0; j < _boardRatio; j++)
                    Cells.Add(new CellViewModel(new FPoint(i, j),
                                                '\0',
                                                _sizeContent));
            }
            
            EnableUndoAndNewGame = gameMode == Mode.LAN ? false : true;

            _gameBoard.CreateNewBoard(_boardRatio, gameMode, firstTurn, aiLevel);
            _gameBoard.SetEventHandlerForGameStrategy(WaitingForOtherPlayer);
            _firstTurn = firstTurn;

            //First run
            Task.Run(() => {
                Message = "Connecting...";
                while(_gameBoard.IsConnectingToServer())               
                {
                    IsPlaying = false;
                    Thread.Sleep(200);
                }

                Message = "Connecting...";
                Task.Run(() => {
                    while(!_gameBoard.IsReadyToStart())
                    {
                        IsPlaying = false;
                        Thread.Sleep(200);
                    }
                    IsPlaying = true;  
                    P1Score = "";
                    P2Score = "";
                });
            });
        }

        private void SetCellContent(FPoint pos, char content)
        {
            _cells[pos.X * _boardRatio + pos.Y].Content = content;
        }    

        private void Undo()
        {
            _gameBoard.UndoRequest();
        }

        public static char GetChar_CellState(CellState cellState)
        {
            if      (cellState == CellState.X)  return 'X';
            else if (cellState == CellState.O)  return 'O';
            else                                return 'N';
        }

        private void HighLightWinCells()
        {
            List<FPoint> winCells = _gameBoard.GetListWinningCellsPosition();
            if (winCells.Count == 0) return;

            foreach (FPoint winCell in winCells)
            {
                Cells[winCell.X * _boardRatio + winCell.Y].IsWinningCell = true;
            }   
        }

        #endregion
    }
}
