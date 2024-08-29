using Caro.Models.Enums;
using Caro.Models.ModelLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Caro.Models.Services
{
    public class AIMovement
    {
        #region Fields
        private Board _board;
        private int MAX_DEPTH = 1;
        private Evaluate AIEvaluate;
        #endregion
        public int MyPiece { get; set; }

        #region Constructor
        public AIMovement(Board board, CellState myPiece, int difficultLevel)
        {
            _board      = board;
            MyPiece     = (int)myPiece;
            AIEvaluate  = new Evaluate(); 
            MAX_DEPTH   = difficultLevel;
        }
        #endregion

        /// <summary>
        /// Find best move for BOT
        /// </summary>
        /// <returns FPoint="move"></returns>
        public FPoint FindBestMove()
        {
            if (_board.IsBoardEmpty())
            {
                int x = -1;
                int y = -1;

                Random randomMove = new Random();
                x = randomMove.Next(_board.BoardRatio / 3, (_board.BoardRatio / 3) * 2);
                y = randomMove.Next(_board.BoardRatio / 3, (_board.BoardRatio / 3) * 2);
                return new FPoint(x, y);
            }
            else if (_board.GetHistoryCount() == 1)
            {
                List<FPoint> allPossibleMoves = _board.generateNeighboreMoves(1).ToList();
                Random randomMove = new Random();

                return allPossibleMoves[randomMove.Next(0, allPossibleMoves.Count - 1)];
            }

            FPoint botWinningMovePos = SearchBotWinningMove();
            if (botWinningMovePos != FPoint.NULL)
            {
                return botWinningMovePos;
            }

            KeyValuePair<FPoint, int> minimax = MinimaxAlphaBeta(MAX_DEPTH, int.MinValue, int.MaxValue, true);

            KeyValuePair<FPoint, int> opponentWinningMovePos = SearchOpponentWinningMove();
            if (opponentWinningMovePos.Key != FPoint.NULL)
            {
                if (minimax.Value > opponentWinningMovePos.Value)
                {
                    return minimax.Key;
                }
                return opponentWinningMovePos.Key;
            }

            return minimax.Key;
        }

        private KeyValuePair<FPoint, int> MinimaxAlphaBeta(int depth, int alpha, int beta, bool isBotTurn)
        {
            KeyValuePair<FPoint, int> moveScore = new KeyValuePair<FPoint, int>(FPoint.NULL, -1);

            if (depth == 0 || (!isBotTurn && AIEvaluate.getScore(_board) > 100000000))
            {
                return new KeyValuePair<FPoint, int>(_board.GetLastestMove(), AIEvaluate.getScore(_board));
            }
            List<FPoint> allPossibleMoves = new List<FPoint>();
            if (_board.GetHistoryCount() == 1)
            {
                allPossibleMoves = _board.generateNeighboreMoves(1).ToList();
            }
            allPossibleMoves = _board.generateNeighboreMoves(2).ToList();

            if (allPossibleMoves.Count == 0)
            {
                return new KeyValuePair<FPoint, int>(_board.GetLastestMove(), AIEvaluate.getScore(_board));
            }

            KeyValuePair<FPoint, int> bestMove = new KeyValuePair<FPoint, int> ( allPossibleMoves[0], 
                                                                                    isBotTurn ? int.MinValue : int.MaxValue );

            foreach (FPoint pos in allPossibleMoves)
            {
                _board.DoPlayAt(pos, true);
                //_board.SwapPlayer();
                moveScore = MinimaxAlphaBeta(depth - 1, alpha, beta, !isBotTurn);
                _board.Undo(true);

                if (isBotTurn)
                {
                    if (moveScore.Value > bestMove.Value)
                    {
                        bestMove = new KeyValuePair<FPoint, int>(pos, moveScore.Value);
                    }
                    alpha = Math.Max(alpha, moveScore.Value);
                }
                else
                {
                    if (moveScore.Value < bestMove.Value)
                    {
                        bestMove = new KeyValuePair<FPoint, int>(pos, moveScore.Value);
                    }
                    beta = Math.Min(beta, moveScore.Value);
                }

                if (beta <= alpha) break;
            }

            return bestMove;
        }

        private FPoint SearchBotWinningMove()
        {
            HashSet<FPoint> allPossibleMoves = _board.generateNeighboreMoves(1);

            int maxScore = int.MinValue;
            FPoint bestMove = FPoint.NULL;

            foreach (FPoint move in allPossibleMoves)
            {
                _board.DoPlayAt(move, true);
                //_board.SwapPlayer();
                int score = AIEvaluate.getScore(_board);
                    
                if (score > maxScore)
                {
                    bestMove = move;
                    maxScore = score;
                }
                _board.Undo(true);
            }

            if (maxScore >= 1000000000) return bestMove;
            return FPoint.NULL;
        }

        private KeyValuePair<FPoint,int> SearchOpponentWinningMove()
        {
            HashSet<FPoint> allPossibleMoves = _board.generateNeighboreMoves(1);

            int maxScore = int.MinValue;
            FPoint bestMove = FPoint.NULL;

            foreach (FPoint move in allPossibleMoves)
            {
                _board.SwapPlayer();
                _board.DoPlayAt(move, true);
                int score = AIEvaluate.getScore(_board);

                if (score > maxScore)
                {
                    bestMove = move;
                    maxScore = score;
                }
                _board.Undo(true);
                _board.SwapPlayer();
            }


            if (maxScore > 10000)
            {
                //MessageBox.Show(maxScore.ToString());
                return new KeyValuePair<FPoint, int> (bestMove, maxScore);
            }
            return new KeyValuePair<FPoint, int> ( FPoint.NULL, maxScore);
        }

    }
}
