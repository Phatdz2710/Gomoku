using Caro.Models.Enums;
using Caro.Models.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Caro.Models.GameStrategy
{
    public abstract class IGameStrategy
    {
        protected   IPlayer   _player1;
        protected   IPlayer   _player2;
        protected   IPlayer   _currentPlayer;
        protected   Board     _board;
        public      EventHandler? PlayerDisconnected;

        private Dictionary<int, int> zobristTable = new Dictionary<int, int>();

        public bool CurPiece { get; set; }
        public IGameStrategy(Board board)
        {
            _board      = board;
            _player1    = new HumanPlayer(_board, CellState.X, "👤 Player 1");
            _player2    = new HumanPlayer(_board, CellState.O, "👤 Player 2");
            _currentPlayer = _player1;
            
            CurPiece    = true;
        }

        public abstract void SendJoinRequestToServer(int boardRatio);

        public abstract void MakeNewRound();

        public abstract void DoPlayAt(Board board, FPoint pos);
        public abstract void Undo(Board board);
        public void SwapPlayer()
        {
            _currentPlayer  = _currentPlayer == _player1 ? _player2 : _player1;
            CurPiece        = _currentPlayer.Piece == CellState.O ? false : true;
        }

        public string Player1Info()
        {
            return _player1.Name + " (" + (_player1.Piece == CellState.X ? "X) : " : "O) : ") + "     " + _player1.Score.ToString();
        }
        public string Player2Info()
        {
            return _player2.Name + " (" + (_player2.Piece == CellState.X ? "X) : " : "O) : ") + "     " + _player2.Score.ToString();
        }

        public bool Player1Turn()
        {
            return _player1.Piece == CellState.X ? true : false;
        }

        public bool Player2Turn()
        {
            return _player2.Piece == CellState.X ? true : false;
        }

        public bool IsBotThinking()
        {
            return _player2.IsThinking;
        }

        public abstract bool IsConnectingToServer();
        public abstract bool IsReadyToStart();
        public abstract void DisconnectedToServer();

    }
}
