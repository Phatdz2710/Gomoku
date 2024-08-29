using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caro.Models.Enums;
using Caro.Models.GameStrategy;
using Caro.Models.ModelLogic;
using Caro.Models.Network;
using Caro.Models.Player;
using Caro.Models.Services;

namespace Caro.Models
{
    public class Board
    {
        #region Event Field
        public EventHandler<PlayResult>? PlayResult; 
        public EventHandler<PlayResult>? UndoEvent;

        #endregion

        #region Field
        private GameLogic       _gameLogic;
        private List<FPoint>    _history;
        private PlayResult      _playResult;
        private IGameStrategy?  _currentStrategy;

        #endregion

        #region Properties

        public int BoardRatio   { get; set; }
        public int CellSize     { get; set; }

        public List<List<int>>  Cells { get; set; }
        #endregion

        #region Constuctor
        public Board()
        {
            BoardRatio      = 9;
            CellSize        = 600 / 9;
            Cells           = new List<List<int>>();
            _history        = new List<FPoint>();
            _gameLogic      = new GameLogic(Cells, BoardRatio);
            _playResult     = new PlayResult(FPoint.NULL, CellState.X, GameState.InProgess);
        }

        #endregion

        #region Create or resize board
        public void ResizeBoard(int boardRatio)
        {
            this.BoardRatio = boardRatio;
            Cells.Clear();
            // Create a 2D list of cells
            for (int i = 0; i < boardRatio; i++)
            {
                Cells.Add(new List<int>());
                for (int j = 0; j < boardRatio; j++)
                {
                    Cells[i].Add(0);
                }
            } 
            _history.Clear();
            _gameLogic.SetBoardRatio(BoardRatio);
        }

        public void CreateNewBoard(int boardRatio, Mode gameMode, CellState firstTurn)
        {
            ResizeBoard(boardRatio);
            switch(gameMode)
            {
                case Mode.PVP:
                    _currentStrategy = new PVPGameStrategy(this);
                    break;
                case Mode.PVE:
                    _currentStrategy = new PVEGameStrategy(this);
                    break;
                case Mode.LAN:
                    _currentStrategy = new LANGameStrategy(this);
                    _currentStrategy.SendJoinRequestToServer(boardRatio);
                    break;
                default:
                    _currentStrategy = new PVPGameStrategy(this);
                    break;
            }
        }

        public void ExitGame()
        {
            if (_currentStrategy == null) return;
            _currentStrategy.DisconnectedToServer();
        }

        public void Reset()
        {
            for (int i = 0; i < BoardRatio; i++)
            {
                for (int j = 0; j < BoardRatio; j++)
                {
                    Cells[i][j] = 0;
                }
            }
            _gameLogic.WinCells.Clear();
            _history.Clear();
            _currentStrategy?.MakeNewRound();
            _playResult.GameState = GameState.InProgess;
        }

        #endregion

        #region Base Methods
        public void DoPlayAt(FPoint pos, bool isSimMove = false)
        {
            if (pos.X < 0 || pos.X >= BoardRatio || pos.Y < 0 || pos.Y >= BoardRatio
                || Cells[pos.X][pos.Y] != 0) 
                return;
            
            if (_currentStrategy == null) 
                return;

            CellState nowTurn       = _currentStrategy.CurPiece ? CellState.X 
                                                                : CellState.O;
            Cells[pos.X][pos.Y]     = (int)nowTurn;
            _history.Add(pos);
            _playResult.Position    = pos;
            _playResult.Piece       = nowTurn;

            SwapPlayer();
            
            // Use this if you want do simulator move (Don't send event to ViewModel)
            // Remember to Undo after this
            if (!isSimMove)
            {
                UpdateBoardState(pos);
                SendPlayResultToViewModel();
            }
        }

        public void Undo(bool isSimUndo = false)
        {
            if (_history.Count == 0) 
                return;

            FPoint pos = _history[_history.Count - 1];
            Cells[pos.X][pos.Y] = 0;
            _history.RemoveAt(_history.Count - 1);
            _playResult.Position = pos;

            SwapPlayer();

            // Use this if you want do simulator undo (Don't send event to ViewModel)
            // Remember to DoPlayAt again after this
            if (!isSimUndo)
            {
                SendUndoResultToViewModel();
            }
        }

        #endregion

        #region Requests from BoardViewModel
        public void PlayAtPointRequest(FPoint pos)
        {
            if (_currentStrategy == null) return;
            _currentStrategy.DoPlayAt(this, pos);
        }

        public void UndoRequest()
        {
            if (_currentStrategy == null) return;
            _currentStrategy.Undo(this);
        }

        // Set event handler for GameStrategy (Only use for LANGameStrategy)
        public void SetEventHandlerForGameStrategy(EventHandler events)
        {
            if (_currentStrategy == null) return;
            _currentStrategy.PlayerDisconnected += events;
        }

        // Only use for LANGameStrategy
        public bool IsConnectingToServer()
        {
            if (_currentStrategy == null) return true;
            return _currentStrategy.IsConnectingToServer();
        }

        // Only use for LANGameStrategy
        public bool IsReadyToStart()
        {
            if (_currentStrategy == null) return false;
            return _currentStrategy.IsReadyToStart();
        }

        #endregion

        #region Send event to ViewModel
        private void SendPlayResultToViewModel()
        {
            PlayResult?.Invoke(this, _playResult);
        }

        private void SendUndoResultToViewModel()
        {
            UndoEvent?.Invoke(this, _playResult);
        }

        #endregion

        public bool CheckWin(FPoint pos)
        {
            if (IsBoardEmpty())
            {
                return false;
            }
            return _gameLogic.CheckWin(pos, Cells[pos.X][pos.Y]);
        }

        public void UpdateBoardState(FPoint pos)
        { 
            if (IsDraw())
            {
                _playResult.GameState = GameState.Draw;
            }
            else if (CheckWin(pos))
            {
                _playResult.GameState = GameState.Win;
            }
            else
            {
                _playResult.GameState = GameState.InProgess;
            }
        }

        public void SwapPlayer()
        {
            if (_currentStrategy == null) return;
            _currentStrategy.SwapPlayer();
        }

        public bool IsDraw()
        {
            if (_history.Count == BoardRatio * BoardRatio) return true;
            return false;
        }

        public bool IsWin()
        {
            if (_playResult.GameState == GameState.Win) return true;
            return false;
        }

        // Get negative of CellState
        public static CellState NegativeCellState(CellState cellState)
        {
            return cellState == CellState.X ? CellState.O : CellState.X;
        }


        #region Get, Set Methods

        /// <summary>
        /// function to get the list of moves that are neighbors of the current moves
        /// </summary>
        /// <param name="radius"> Radius around current moves </param>
        /// <returns>HashSet<FPoint></returns> List of Neighbore moves <summary>
        public HashSet<FPoint> generateNeighboreMoves(int radius)
        {
            HashSet<FPoint> neighboreMoves = new HashSet<FPoint>();
            foreach (FPoint pos in _history)
            {
                for (int i = -1 * radius; i <= radius; i++)
                {
                    for (int j = -1 * radius; j <= radius; j++)
                    {
                        if ((i == 0 && j == 0) || pos.X + i < 0 || pos.X + i >= BoardRatio
                            || pos.Y + j < 0 || pos.Y + j >= BoardRatio
                            || Cells[pos.X + i][pos.Y + j] != 0) continue;
                        neighboreMoves.Add(new FPoint(pos.X + i, pos.Y + j));
                    }
                }
            }

            return neighboreMoves;
        }

        public CellState GetCellStateAt(FPoint pos)
        {
            if (pos.X < 0 || pos.X >= BoardRatio || pos.Y < 0 || pos.Y >= BoardRatio) 
            {
                return CellState.Empty;
            }

            if (Cells[pos.X][pos.Y] == -1) 
            {
                return CellState.O;
            }
            else if (Cells[pos.X][pos.Y] == 0) 
            {
                return CellState.Empty;
            }
            
            return CellState.X;
        }

        public FPoint GetLastestMove()
        {
            if (_history.Count == 0) 
                return FPoint.NULL;
            
            return _history[_history.Count - 1];
        }

        public List<FPoint> GetListWinningCellsPosition()
        {
            return _gameLogic.WinCells;
        }

        // Convert board to string
        public override string ToString()
        {
            string strBoard = "";
            for (int i = 0; i < BoardRatio;i++)
            {
                for (int j = 0;j < BoardRatio; j++)
                {
                    if (Cells[i][j] == -1) strBoard += "2";
                    else strBoard += Cells[i][j].ToString();
                }
                strBoard += "\n";
            }
            return strBoard;
        }

        public bool GetCurPlayer()
        {
            if (_currentStrategy == null) return true;
            return _currentStrategy.CurPiece;
        }

        public bool IsBoardEmpty()
        {
            if (_history.Count == 0) return true;
            return false;
        }
        
        public int GetHistoryCount()
        {
            return _history.Count;
        }

        public int GetSizeHistory()
        {
            return _history.Count; 
        }
        public string Player1Info()
        {
            if (_currentStrategy == null) return String.Empty;
            return _currentStrategy.Player1Info();
        }
        public bool Player1Turn()
        {
            if (_currentStrategy == null) return true;
            return _currentStrategy.Player1Turn();
        }
        public string Player2Info()
        {
            if (_currentStrategy == null) return String.Empty;
            return _currentStrategy.Player2Info();
        }
        public bool Player2Turn()
        {
            if (_currentStrategy == null) return true;
            return _currentStrategy.Player2Turn();
        }
        public bool IsBotThinking()
        {
            if (_currentStrategy == null) return false;
            return _currentStrategy.IsBotThinking();
        }

        #endregion
    }
}
