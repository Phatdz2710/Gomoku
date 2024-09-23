using Caro.Models.Enums;
using Caro.Models.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caro.Models.GameStrategy
{
    public class EVEGameStrategy : IGameStrategy
    {
        public EVEGameStrategy(Board board) : base(board)
        {
            _player1 = new AIPlayer(_board, CellState.X, AILevel.Hard, "🤖 BOT ");
            _player2 = new AIPlayer(_board, CellState.O, AILevel.Easy, "🤖 BOT HD");
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
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(2000);
                    if (_currentPlayer.IsThinking) continue;
                    _currentPlayer.MakeMove(pos);
                }
            });
        }

        public override void Undo(Board board)
        {
            return;
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
