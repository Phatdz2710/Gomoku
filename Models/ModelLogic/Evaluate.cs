using Caro.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Caro.Models.ModelLogic
{
    public class Evaluate
    {
        //private Board gameBoard;
        public Evaluate()
        {
            //this.gameBoard = gameBoard;
        }
        public class NumberofScorePattern
        {
            public int Winning      = 0;
            public int Stone4       = 0;
            public int Stone2       = 0;
            public int Stone4Block  = 0;
            public int Stone4OneSpace = 0;
            public int Stone3NoBlock = 0;
            public int Stone3OneBlock  = 0;
            public int Stone3TwoBlock = 0;
            public NumberofScorePattern() { }

            public NumberofScorePattern(int winning, int stone4, int stone4OneSpace, int stone4Block,
                int stone3NoBlock, int stone3OneBlock, int stone3TwoBlock, int stone2)
            {
                Winning     = winning;
                Stone4      = stone4;
                Stone4OneSpace = stone4OneSpace;
                Stone2      = stone2;
                Stone4Block = stone4Block;
                Stone3OneBlock = stone3OneBlock;
                Stone3TwoBlock = stone3TwoBlock;
            }
        }

        public class ListStonePattern
        {
            public static ListStonePattern Create(bool isForX)
            {
                ListStonePattern list = new ListStonePattern();
                if (isForX) return list;

                list.Stone3OneBlock    = convertListPattern(list.Stone3OneBlock);
                list.Stone4Block    = convertListPattern(list.Stone4Block);
                list.Stone4OneSpace = convertListPattern(list.Stone4OneSpace);
                list.Stone2NoBlock  = convertListPattern(list.Stone2NoBlock);
                list.Stone3NoBlock  = convertListPattern(list.Stone3NoBlock);
                list.Stone3TwoBlock = convertListPattern(list.Stone3TwoBlock);
                list.Stone4NoBlock  = convertListPattern(list.Stone4NoBlock);
                list.winPattern     = convertListPattern(list.winPattern);

                return list;
            }

            public ListStonePattern() { }

            public List<string> winPattern      = new List<string>() { "XXXXX" };
            public List<string> Stone4NoBlock   = new List<string>() { ".XXXX."};
            public List<string> Stone4OneSpace  = new List<string>()
            {
                ".X.XXX.", ".XXX.X.", ".XX.XX."
            };

            public List<string> Stone4Block = new List<string>()
            {
                "OX.XXX", "OXX.XX", "OXXX.X", "OXXXX.", ".XXXXO", "X.XXXO", "XX.XXO", "XXX.XO"
            };

            public List<string> Stone3NoBlock   = new List<string>()
            {
                ".XXX..", "..XXX.", ".X.XX.", ".XX.X.",
            };

            public List<string> Stone3TwoBlock   = new List<string>()
            {
                "OX.X.XO", "O.XXX.O", "OXX..XO", "OX..XXO",
            };

            public List<string> Stone3OneBlock = new List<string>()
            {
                "OXXX..", "OXX.X.", "OX.XX.", "..XXXO", ".X.XXO", ".XX.XO",
            };

            public List<string> Stone2NoBlock = new List<string>()
            {
                "..XX..", ".X.X..", "..X.X.", ".XX...", "...XX.", ".X..X."
            };
        }

        private static List<string> convertListPattern(List<string> list)
        {
            int size = 0;

            string newPattern;
            for (int j = 0; j < list.Count; j++)
            {
                size = list[j].Length;
                newPattern = new string('O', size);
                char[] chars = newPattern.ToCharArray();
                for (int i = 0; i < size; i++)
                {
                    if (list[j][i] == 'O') chars[i] = 'X';
                    else if (list[j][i] == '.') chars[i] = '.';
                }

                list[j] = new string(chars);
            }

            return list;
        }

        private bool IsAnyInString(string source, List<string> patterns)
        {
            foreach (string pattern in patterns)
                if (source.Contains(pattern)) return true;

            return false;
        }

        public NumberofScorePattern valuePosition(List<string> direction, bool isForX)
        {
            NumberofScorePattern scorePattern = new NumberofScorePattern();
            ListStonePattern listPattern = isForX ? ListStonePattern.Create(true) 
                                                    : ListStonePattern.Create(false);
            for (int i = 0; i < direction.Count; i++)
            {
                string directionString = direction[i];
                if (IsAnyInString(directionString, listPattern.winPattern))
                    scorePattern.Winning++;

                else if (IsAnyInString(directionString, listPattern.Stone4NoBlock))
                    scorePattern.Stone4++;

                else if (IsAnyInString(directionString, listPattern.Stone3NoBlock))
                    scorePattern.Stone3NoBlock++;

                else if (IsAnyInString(directionString, listPattern.Stone4OneSpace))
                    scorePattern.Stone4OneSpace++;

                else if (IsAnyInString(directionString, listPattern.Stone4Block))
                    scorePattern.Stone4Block++;

                else if (IsAnyInString(directionString, listPattern.Stone3OneBlock))
                    scorePattern.Stone3OneBlock++;

                else if (IsAnyInString(directionString, listPattern.Stone3TwoBlock))
                    scorePattern.Stone3TwoBlock++;

                else if (IsAnyInString(directionString, listPattern.Stone2NoBlock))
                    scorePattern.Stone2++;

            }

            return scorePattern;
        }

        private string GetCellValueInDirection(Board board, FPoint checkPos, CellState cellValue, FPoint posDelta)
        {
            string result = String.Empty;
            for (int i = 1; i < 5; i++)
            {
                FPoint nextPos = new FPoint(checkPos.X - posDelta.X * i,
                                            checkPos.Y - posDelta.Y * i);
                if (nextPos.X < 0 || nextPos.X >= board.BoardRatio 
                   || nextPos.Y < 0 || nextPos.Y >= board.BoardRatio) break;

                if (board.GetCellStateAt(nextPos) == CellState.X)       result = "X" + result;
                else if (board.GetCellStateAt(nextPos) == CellState.O)  result = "O" + result;
                else result = "." + result;
            }

            result = result + (cellValue == CellState.X ? "X" : "O");

            for (int i = 1; i < 5; i++)
            {
                FPoint nextPos = new FPoint(checkPos.X + posDelta.X * i,
                                            checkPos.Y + posDelta.Y * i);
                if (nextPos.X < 0 || nextPos.X >= board.BoardRatio || nextPos.Y < 0 || nextPos.Y >= board.BoardRatio) break;

                if (board.GetCellStateAt(nextPos) == CellState.X) result += 'X';
                else if (board.GetCellStateAt(nextPos) == CellState.O) result += 'O';
                else result += '.';
            }

            return result;
        }

        private List<string> GetAllListDirection(Board board, FPoint checkPos, CellState cellValue)
        {
            FPoint rowDeltaDirection = new FPoint(1, 0);
            FPoint colDeltaDirection = new FPoint(0, 1);
            FPoint mainDiagonalDeltaDirection   = new FPoint(1, 1);
            FPoint secondDiagonalDeltaDirection = new FPoint(1, -1);

            string row          = GetCellValueInDirection(board, checkPos, cellValue, rowDeltaDirection);
            string col          = GetCellValueInDirection(board, checkPos, cellValue, colDeltaDirection);
            string mainDiagonal = GetCellValueInDirection(board, checkPos, cellValue, mainDiagonalDeltaDirection);
            string secondDiagonal = GetCellValueInDirection(board, checkPos, cellValue, secondDiagonalDeltaDirection);

            List<string> allDirection = new List<string>()
            {
                row, col, mainDiagonal, secondDiagonal
            };

            return allDirection;
        }

        private int getScoreByPattern(NumberofScorePattern numberofPattern)
        {
            int _winGuarantee = 10000000;

            //Chac chan win
            if (numberofPattern.Winning > 0)
            {
                return 1000000000;
            }

            //if (numberofPattern.Stone4 > 0 || numberofPattern.Stone4OneSpace > 1 || numberofPattern.Stone4Block > 1)
            //{
            //    return _winGuarantee * (numberofPattern.Stone4Block + numberofPattern.Stone4OneSpace + numberofPattern.Stone4);
            //}

            if (numberofPattern.Stone4 > 0)
            {
                return _winGuarantee * numberofPattern.Stone4;
            }

            if (numberofPattern.Stone4OneSpace > 1)
            {
                return _winGuarantee * numberofPattern.Stone4OneSpace / 20;
            }

            if (numberofPattern.Stone4Block > 1)
            {
                return _winGuarantee * numberofPattern.Stone4Block / 20;
            }

            if (numberofPattern.Stone4OneSpace > 0 && numberofPattern.Stone4Block > 0)
            {
                return _winGuarantee * (numberofPattern.Stone4OneSpace + numberofPattern.Stone4Block) / 20;
            }

            if (numberofPattern.Stone4OneSpace > 0 && numberofPattern.Stone3NoBlock > 0)
            {
                return _winGuarantee * (numberofPattern.Stone4OneSpace + numberofPattern.Stone3NoBlock / 2) / 40;
            }

            if (numberofPattern.Stone3NoBlock > 0 && numberofPattern.Stone4Block > 0)
            {
                return _winGuarantee * (numberofPattern.Stone4Block + numberofPattern.Stone3NoBlock / 2) / 40;
            }

            if (numberofPattern.Stone3NoBlock > 1)
            {
                return _winGuarantee * numberofPattern.Stone3NoBlock / 80;
            }



            if (numberofPattern.Stone4Block == 1)
            {
                if (numberofPattern.Stone3OneBlock == 3) return 80000;
                if (numberofPattern.Stone3OneBlock == 2) return 40000;

                if (numberofPattern.Stone2 == 3) return 25000;
                if (numberofPattern.Stone3TwoBlock == 3) return 24000;
                
                if (numberofPattern.Stone3OneBlock == 1) return 30000;

                if (numberofPattern.Stone2 == 2) return 20000;
                if (numberofPattern.Stone3TwoBlock == 2) return 18000;

                if (numberofPattern.Stone3TwoBlock == 1) return 10000;
                if (numberofPattern.Stone2 == 1) return 7000;

                return 3450;
            }

            if (numberofPattern.Stone3NoBlock == 1)
            {
                if (numberofPattern.Stone3OneBlock == 3) return 80000;
                if (numberofPattern.Stone3OneBlock == 2) return 30000;

                if (numberofPattern.Stone3TwoBlock == 3) return 30000;
                if (numberofPattern.Stone2 == 3) return 25000;

                if (numberofPattern.Stone3TwoBlock == 2) return 20000;
                if (numberofPattern.Stone2 == 2) return 18000;
                if (numberofPattern.Stone2 == 1) return 15000;
                if (numberofPattern.Stone3OneBlock == 1) return 12000;
                if (numberofPattern.Stone3TwoBlock == 1) return 10250;

                return 2000;
            }


            switch (numberofPattern.Stone3OneBlock)
            {
                case 3:
                    if (numberofPattern.Stone3TwoBlock == 1) return 9000;
                    if (numberofPattern.Stone2 == 1) return 8000;
                    break;
                case 2:
                    if (numberofPattern.Stone2 == 2) return 9000;
                    if (numberofPattern.Stone3TwoBlock == 2) return 8500;
                    if (numberofPattern.Stone2 == 1) return 2000;
                    if (numberofPattern.Stone3TwoBlock == 1) return 1800;
                    break;
                case 1:
                    switch (numberofPattern.Stone2)
                    {
                        case 3: return 3400;
                        case 2: return 3300;
                        case 1: return 3100;
                    }
                    switch (numberofPattern.Stone3TwoBlock)
                    {
                        case 3: return 3000;
                        case 2: return 2000;
                        case 1: return 1000;
                    }
                    break;
                default: return 500;
            }

            switch (numberofPattern.Stone2)
            {
                case 4: return 2700;
                case 3: return 2500;
                case 2: return 2000;
                case 1: return 200;
            }

            return 0;
        }


        private int heuristic(Board board)
        {
            FPoint latestPos = board.GetLastestMove();

            //Check if this move is player move
            CellState playerCellValue = board.GetCellStateAt(latestPos);
            List<string> allDirection = GetAllListDirection(board, latestPos, playerCellValue);
            bool isForX = (playerCellValue == CellState.X);
            NumberofScorePattern scorePattern = valuePosition(allDirection, isForX);
            int playerValue = getScoreByPattern(scorePattern);

            return playerValue;
        }

        public int getScore(Board board)
        {
            return heuristic(board);
        }
    }
}
