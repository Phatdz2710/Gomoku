using Caro.Models.Enums;
using Caro.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Caro.Models.Player
{
    public class AIPlayer : IPlayer
    {
        private AIMovement  AIMovement;
        private AILevel     AILevel;

        private CancellationTokenSource _cancellationTokenSource;

        public override CellState Piece
        {
            get => _piece;
            set
            {
                _piece = value;
                AIMovement.MyPiece = (int)_piece;
            }
        }

        public AIPlayer(Board _board, CellState _cellState, AILevel _level) 
            : base(_board, _cellState, "🤖 BOT")
        {
            AILevel     =   _level;
            AIMovement  =   new AIMovement(_board, _cellState, (int)_level);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public override void MakeMove(FPoint pos)
        {
            Task thinking   = Task.Run(() => {
                var token       = _cancellationTokenSource.Token;
                IsThinking      = true;
                FPoint bestMove = FPoint.NULL;

                try
                {
                    bestMove = FindBestMove();
                    _board.DoPlayAt(bestMove);
                }
                catch (OperationCanceledException)
                { }
                finally 
                { }
                
            }, _cancellationTokenSource.Token);

            thinking.ContinueWith(t =>
            {
                IsThinking = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public override void CancelMove()
        {
            _cancellationTokenSource.Cancel();
        }
        protected override FPoint FindBestMove()
        {
            return AIMovement.FindBestMove();
        }

    }
}
