using Caro.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
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
            public int Stone3       = 0;
            public int Stone2       = 0;
            public int Stone4Block  = 0;
            public int Stone4OneSpace = 0;
            public int Stone3Block  = 0;
            public NumberofScorePattern() { }

            public NumberofScorePattern(int winning, int stone4, int stone4OneSpace, int stone3, int stone2, int stone4Block, int stone3Block)
            {
                Winning     = winning;
                Stone4      = stone4;
                Stone4OneSpace = stone4OneSpace;
                Stone3      = stone3;
                Stone2      = stone2;
                Stone4Block = stone4Block;
                Stone3Block = stone3Block;
            }
        }

        public class ListStonePattern
        {
            public static ListStonePattern Create(bool isForX)
            {
                ListStonePattern list = new ListStonePattern();
                if (isForX) return list;

                list.Stone3Block    = convertListPattern(list.Stone3Block);
                list.Stone4Block    = convertListPattern(list.Stone4Block);
                list.Stone4OneSpace = convertListPattern(list.Stone4OneSpace);
                list.Stone2NoBlock  = convertListPattern(list.Stone2NoBlock);
                list.Stone3NoBlock  = convertListPattern(list.Stone3NoBlock);
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
           
            public List<string> Stone3NoBlock   = new List<string>()
            {
                ".XXX..", "..XXX.", ".X.XX.", ".XX.X.", ".X.X.X."
            };
            public List<string> Stone2NoBlock   = new List<string>()
            {
                "..XX..", ".X.X..", "..X.X.", ".XX...", "...XX.", ".X..X."
            };
            public List<string> Stone4Block     = new List<string>()
            {
                "OX.XXX", "OXX.XX", "OXXX.X", "OXXXX.", ".XXXXO", "X.XXXO", "XX.XXO", "XXX.XO"
            };
            public List<string> Stone3Block     = new List<string>()
            {
                "OXXX..", "OXX.X.", "OX.XX.", "..XXXO", ".X.XXO", ".XX.XO",
                "OX.X.XO", "O.XXX.O", "OXX..XO", "OX..XXO",
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
                    scorePattern.Stone3++;

                else if (IsAnyInString(directionString, listPattern.Stone4OneSpace))
                    scorePattern.Stone4OneSpace++;

                else if (IsAnyInString(directionString, listPattern.Stone4Block))
                    scorePattern.Stone4Block++;

                else if (IsAnyInString(directionString, listPattern.Stone3Block))
                    scorePattern.Stone3Block++;

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
            int _winGuarantee = 100000000;
            if (numberofPattern.Winning > 0)
            {
                return 1000000000;
            }

            if (numberofPattern.Stone4 > 1)
            {
                return _winGuarantee;
            }


            if (numberofPattern.Stone4 > 0)
            {
                return _winGuarantee / 10;
            }


            if (numberofPattern.Stone4Block > 1)
            {
                return _winGuarantee / 10;
            }


            if (numberofPattern.Stone4OneSpace > 1)
            {
                return _winGuarantee / 10;
            }

            if (numberofPattern.Stone4OneSpace > 0 && numberofPattern.Stone4Block > 0)
            {
                return _winGuarantee / 10;
            }

            if (numberofPattern.Stone4OneSpace > 0 && numberofPattern.Stone3 > 0)
            {
                return _winGuarantee / 20;
            }

            if (numberofPattern.Stone3 > 0 && numberofPattern.Stone4Block > 0)
            {
                return _winGuarantee / 50;
            }

            if (numberofPattern.Stone3 > 1)
            {
                //MessageBox.Show("Stone3");
                return _winGuarantee / 50;
            }

            if (numberofPattern.Stone3 == 1)
            {
                switch(numberofPattern.Stone2)
                {
                    case 3:     return 40000;
                    case 2:     return 38000;
                    case 1:     return 35000;
                    default:    return 3450;
                }
            }

            if (numberofPattern.Stone4Block == 1)
            {
                switch (numberofPattern.Stone3)
                {
                    case 3: return 1000000;
                    case 2: return 400000;
                    case 1: return 100000;
                }

                switch (numberofPattern.Stone3Block)
                {
                    case 3: return 100000;
                    case 2: return 40000;
                    case 1: return 1000;
                }

                switch (numberofPattern.Stone2)
                {
                    case 3:     return 4500;
                    case 2:     return 4200;
                    case 1:     return 4100;
                    default:    return 4050;
                }
            }

            switch (numberofPattern.Stone3Block)
            {
                case 3:
                    if (numberofPattern.Stone2 == 1) return 280;
                    break;
                case 2:
                    switch (numberofPattern.Stone2)
                    {
                        case 2: return 3000;
                        case 1: return 2900;

                    }
                    break;
                case 1:
                    switch (numberofPattern.Stone2)
                    {
                        case 3: return 3400;
                        case 2: return 3300;
                        case 1: return 3100;

                    }
                    break;
            }

            switch (numberofPattern.Stone2)
            {
                case 4: return 2700;
                case 3: return 2500;
                case 2: return 2000;
                case 1: return 1000;

            }
            return 0;
        }

        private int heuristic (Board board)
        {
            FPoint latestPos = board.GetLastestMove();

            //Check if this move is player move
            CellState playerCellValue       = board.GetCellStateAt(latestPos);
            List<string> allDirection       = GetAllListDirection(board, latestPos, playerCellValue);
            bool isForX = (playerCellValue == CellState.X);
            NumberofScorePattern scorePattern = valuePosition(allDirection, isForX);
            int playerValue = getScoreByPattern(scorePattern);

            //Check if this move is enemy move
            CellState opponentCellValue     = playerCellValue == CellState.X ? CellState.O : CellState.X;
            board.Cells[latestPos.X][latestPos.Y] = opponentCellValue == CellState.X ? 1 : -1;
            List<string> enemyAllDirection  = GetAllListDirection(board, latestPos, opponentCellValue);
            isForX = !isForX;
            NumberofScorePattern enemyScorePattern = valuePosition(enemyAllDirection, isForX);
            int enemyValue = getScoreByPattern(enemyScorePattern);

            //reset board cell at pos 
            board.Cells[latestPos.X][latestPos.Y] = opponentCellValue == CellState.X ? -1 : 1;

            return playerValue * 2 + enemyValue;
        }

        public int getScore(Board board)
        {
            return heuristic(board);
        }
    }
}
