using Caro.Models.Enums;
using Caro.Models.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caro.Models.GameStrategy
{
    public class PVPGameStrategy : IGameStrategy
    {

        public PVPGameStrategy(Board board) : base(board)
        {
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

            _currentPlayer = _player1.Piece == CellState.X ? _player1 : _player2;
            CurPiece = true;
        }

        public override void DoPlayAt(Board board, FPoint pos)
        {
            _currentPlayer.MakeMove(pos);
        }

        public override void Undo(Board board)
        {
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
