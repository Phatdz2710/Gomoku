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
        //private int MAX_DEPTH = 1;
        private AILevel _mode = AILevel.Easy;
        private Evaluate AIEvaluate;
        #endregion
        public int MyPiece { get; set; }

        int maxDepth = 4;
        int radius = 1;
        


        #region Constructor
        public AIMovement(Board board, CellState myPiece, AILevel lvl)
        {
            _board      = board;
            MyPiece     = (int)myPiece;
            AIEvaluate  = new Evaluate(); 
            _mode   = lvl;

            if (_mode == AILevel.Easy)
            {
                maxDepth = 3;
                radius = 2;
            }
            else
            {
                maxDepth = 4;
                radius = 1;
            }
        }

        private Dictionary<ulong, KeyValuePair<int, int>> zobristMap = new Dictionary<ulong, KeyValuePair<int, int>>();

        private Dictionary<ulong, TranspositionTable> transpositionMap = new Dictionary<ulong, TranspositionTable>();
        #endregion

        /// <summary>
        /// Find best move for BOT
        /// </summary>
        /// <returns FPoint="move"></returns>
        /// 


        public FPoint FindBestMove()
        {
            // Random move if board is empty
            if (_board.IsBoardEmpty())
            {
                int x = -1;
                int y = -1;

                Random randomMove = new Random();
                x = randomMove.Next(_board.BoardRatio / 3, (_board.BoardRatio / 3) * 2);
                y = randomMove.Next(_board.BoardRatio / 3, (_board.BoardRatio / 3) * 2);
                return new FPoint(x, y);
            } 
            // Random move if board has only one move
            else if (_board.GetHistoryCount() == 1)
            {
                List<FPoint> allPossibleMoves = _board.generateNeighboreMoves(1).ToList();
                Random randomMove = new Random();

                return allPossibleMoves[randomMove.Next(0, allPossibleMoves.Count - 1)];
            }

            // Find winning move
            FPoint botWinningMovePos = SearchBotWinningMove();
            if (botWinningMovePos != FPoint.NULL)
            {
                return botWinningMovePos;
            }

            FPoint opponentWinningMovePos = SearchOpponentWinningMove();
            if (opponentWinningMovePos != FPoint.NULL)
            {
                return opponentWinningMovePos;
            }

            //if (_board.GetHistoryCount() > 30) maxDepth = 3;
            //else maxDepth = 4;

            KeyValuePair<FPoint, int> minimax = MinimaxAlphaBeta(maxDepth, int.MinValue, int.MaxValue, true, 0);

            return minimax.Key;
        }

        //private KeyValuePair<FPoint, int> MinimaxAlphaBeta(int depth, int alpha, int beta, bool isBotTurn, int score)
        //{
        //    KeyValuePair<FPoint, int> moveScore = new KeyValuePair<FPoint, int>(FPoint.NULL, -1);
        //    if (depth == 0)
        //    {
        //        moveScore = new KeyValuePair<FPoint, int>(FPoint.NULL, score);
        //        return moveScore;
        //    }
        //    List<FPoint> allPossibleMoves = new List<FPoint>();


        //    allPossibleMoves = _board.generateNeighboreMoves(2).ToList();
        //    if (allPossibleMoves.Count > 25)
        //    {
        //        allPossibleMoves = _board.generateNeighboreMoves(1).ToList();
        //    }

        //    KeyValuePair<FPoint, int> bestMove = new KeyValuePair<FPoint, int>(allPossibleMoves[0],
        //                                                                            isBotTurn ? int.MinValue :
        //                                                                                        int.MaxValue);

        //    if (allPossibleMoves.Count == 0)
        //    {
        //        return bestMove;
        //    }

        //    foreach (FPoint pos in allPossibleMoves)
        //    {
        //        if (isBotTurn)
        //        {
        //            _board.DoPlayAt(pos, true);
        //            ulong zbHasingValue = _board.GetZobristHashValue();

        //            if (_mode == AILevel.Mode1 && zobristMap.ContainsKey(zbHasingValue) && zobristMap[zbHasingValue].Value == depth)
        //            {
        //                //if (zobristMap[zbHasingValue].Value == depth)
        //                //{
        //                moveScore = new KeyValuePair<FPoint, int>(pos, zobristMap[zbHasingValue].Key);
        //                //}
        //            }
        //            else
        //            {
        //                score += AIEvaluate.getScore(_board);
        //                moveScore = MinimaxAlphaBeta(depth - 1, alpha, beta, !isBotTurn, score);

        //                if (_mode == AILevel.Mode1)
        //                {
        //                    if (zobristMap.ContainsKey(zbHasingValue))
        //                    {
        //                        zobristMap[zbHasingValue] = new KeyValuePair<int, int>(moveScore.Value, depth);
        //                    }
        //                    else
        //                        zobristMap.Add(zbHasingValue, new KeyValuePair<int, int>(moveScore.Value, depth));
        //                }
        //            }

        //            _board.Undo(true);

        //            if (moveScore.Value > bestMove.Value)
        //            {
        //                bestMove = new KeyValuePair<FPoint, int>(pos, moveScore.Value);
        //            }
        //            alpha = Math.Max(alpha, moveScore.Value);
        //        }
        //        else
        //        {
        //            _board.DoPlayAt(pos, true);

        //            ulong zbHasingValue = _board.GetZobristHashValue();

        //            if (_mode == AILevel.Mode1 && zobristMap.ContainsKey(zbHasingValue) && zobristMap[zbHasingValue].Value == depth)
        //            {
        //                //if (zobristMap[zbHasingValue].Value == depth)
        //                //{
        //                moveScore = new KeyValuePair<FPoint, int>(pos, zobristMap[zbHasingValue].Key);
        //                //}
        //            }
        //            else
        //            {
        //                score -= AIEvaluate.getScore(_board);

        //                moveScore = MinimaxAlphaBeta(depth - 1, alpha, beta, !isBotTurn, score);
        //                if (_mode == AILevel.Mode1)
        //                {
        //                    if (zobristMap.ContainsKey(zbHasingValue))
        //                    {
        //                        zobristMap[zbHasingValue] = new KeyValuePair<int, int>(moveScore.Value, depth);
        //                    }
        //                    else
        //                        zobristMap.Add(zbHasingValue, new KeyValuePair<int, int>(moveScore.Value, depth));
        //                }
        //            }
        //            _board.Undo(true);

        //            if (moveScore.Value < bestMove.Value)
        //            {
        //                bestMove = new KeyValuePair<FPoint, int>(pos, moveScore.Value);
        //            }
        //            beta = Math.Min(beta, moveScore.Value);
        //        }

        //        if (beta <= alpha) break;
        //    }

        //    return bestMove;
        //}

        private KeyValuePair<FPoint, int> MinimaxAlphaBeta(int depth, int alpha, int beta, bool isBotTurn, int score)
        {
            KeyValuePair<FPoint, int> moveScore = new KeyValuePair<FPoint, int>(FPoint.NULL, -1);
            if (depth == 0)
            {
                moveScore = new KeyValuePair<FPoint, int>(FPoint.NULL, score);
                return moveScore;
            }
            List<FPoint> allPossibleMoves = new List<FPoint>();


            
            allPossibleMoves = _board.generateNeighboreMoves(radius).ToList();
            //if (allPossibleMoves.Count > 25)
            //{
            //    allPossibleMoves = _board.generateNeighboreMoves(1).ToList();
            //}

            KeyValuePair<FPoint, int> bestMove = new KeyValuePair<FPoint, int>(allPossibleMoves[0],
                                                                                    isBotTurn ? int.MinValue :
                                                                                                int.MaxValue);

            if (allPossibleMoves.Count == 0)
            {
                return bestMove;
            }

            foreach (FPoint pos in allPossibleMoves)
            {
                if (isBotTurn)
                {
                    _board.DoPlayAt(pos, true);
                    ulong zbHasingValue = _board.GetZobristHashValue();

                    if (transpositionMap.ContainsKey(zbHasingValue)
                        && transpositionMap[zbHasingValue].Depth >= depth && transpositionMap[zbHasingValue].PosMove == pos)
                    {
                        moveScore = new KeyValuePair<FPoint, int>(pos, transpositionMap[zbHasingValue].Score);
                    }
                    else
                    {
                        score += AIEvaluate.getScore(_board);
                        moveScore = MinimaxAlphaBeta(depth - 1, alpha, beta, !isBotTurn, score);

                        if (transpositionMap.ContainsKey(zbHasingValue))
                        {
                            transpositionMap[zbHasingValue] = new TranspositionTable()
                            {
                                Score = moveScore.Value,
                                Depth = depth,
                                PosMove = pos,
                            };
                        }
                        else
                        {
                            transpositionMap.Add(zbHasingValue, new TranspositionTable()
                            {
                                Score = moveScore.Value,
                                Depth = depth,
                                PosMove = pos,
                            });
                        }
                    }

                    _board.Undo(true);

                    if (moveScore.Value > bestMove.Value)
                    {
                        bestMove = new KeyValuePair<FPoint, int>(pos, moveScore.Value);
                    }
                    alpha = Math.Max(alpha, moveScore.Value);
                }
                else
                {
                    _board.DoPlayAt(pos, true);

                    ulong zbHasingValue = _board.GetZobristHashValue();

                    if (transpositionMap.ContainsKey(zbHasingValue)
                        && transpositionMap[zbHasingValue].Depth >= depth && transpositionMap[zbHasingValue].PosMove == pos)
                    {
                        moveScore = new KeyValuePair<FPoint, int>(pos, transpositionMap[zbHasingValue].Score);
                    }
                    else
                    {
                        score -= AIEvaluate.getScore(_board);
                        moveScore = MinimaxAlphaBeta(depth - 1, alpha, beta, !isBotTurn, score);

                        if (transpositionMap.ContainsKey(zbHasingValue))
                        {
                            transpositionMap[zbHasingValue] = new TranspositionTable()
                            {
                                Score = moveScore.Value,
                                Depth = depth,
                                PosMove = pos,
                            };
                        }
                        else
                        {
                            transpositionMap.Add(zbHasingValue, new TranspositionTable()
                            {
                                Score = moveScore.Value,
                                Depth = depth,
                                PosMove = pos,
                            });
                        }
                    }
                    _board.Undo(true);

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

        private FPoint SearchOpponentWinningMove()
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


            if (maxScore >= 1000000000)
            {
                return bestMove;
            }
            return FPoint.NULL;
        }

        public void ClearZobristMap()
        {
            transpositionMap.Clear();
        }
    }
}
