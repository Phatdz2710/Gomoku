using Caro.Models.Enums;
using Caro.Models.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caro.Models.GameStrategy
{
    public class PVEGameStrategy : IGameStrategy
    {
        public PVEGameStrategy(Board board) : base(board)
        {
            _player1 = new HumanPlayer(_board, CellState.X, "👤 Human");
            _player2 = new AIPlayer(_board, CellState.O, AILevel.Easy);
            _currentPlayer = _player1;
        }

        public override void MakeNewRound()
        {
            if (_board.IsWin())
            {
                if (_currentPlayer == _player2)
                {
                    _player1.Score++;
                    if (_player1.Piece == CellState.X)
                    {
                        _player1.SetPiece(CellState.O);
                        _player2.SetPiece(CellState.X);
                    }
                }
                else
                {
                    _player2.Score++;
                    if (_player2.Piece == CellState.X)
                    {
                        _player1.SetPiece(CellState.X);
                        _player2.SetPiece(CellState.O);
                    }
                }
            }

            CurPiece = true;
            if (_currentPlayer == _player2)
            {
                _currentPlayer.MakeMove(FPoint.NULL);
            }
        }

        public override void DoPlayAt(Board board, FPoint pos)
        {
            _currentPlayer.MakeMove(pos);
            if (board.IsWin()) return;
            _currentPlayer.MakeMove(pos);
            if (board.IsWin()) return;
        }

        public override void Undo(Board board)
        {
            if (board.GetSizeHistory() == 1 && _currentPlayer == _player1)
            {
                return;
            }
            board.Undo();
            board.Undo();
        }

        public override bool IsConnectingToServer()
        {
            return false;
        }

        public override bool IsReadyToStart()
        {
            return true;
        }

        public override void DisconnectedToServer()
        {
            return;
        }

        public override void SendJoinRequestToServer(int boardRatio = 9)
        {
            return;
        }

    }
}
