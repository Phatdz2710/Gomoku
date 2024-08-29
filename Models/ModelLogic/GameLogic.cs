using Caro.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Caro.Models.Services
{
    public class GameLogic
    {
        private List<List<int>> Cells;
        public int _boardRatio;
        public List<FPoint> WinCells { get; private set; } 

        public GameLogic (List<List<int>> cells, int boardRatio)
        {
            Cells       = cells;
            _boardRatio = boardRatio;
            WinCells    = new List<FPoint>();
        }

        public void SetBoardRatio(int boardRatio)
        {
            this._boardRatio = boardRatio;
        }

        #region CHECK WIN
        public bool CheckWin(FPoint pos, int piece)
        {
            bool horizontal     = CheckHorizontal(pos, piece);
            bool vertical       = CheckVertical(pos, piece);
            bool mainDiagonal   = CheckMainDiagonal(pos, piece);
            bool secondDiagonal = CheckSecondDiagonal(pos, piece);

            if ( horizontal || vertical || mainDiagonal || secondDiagonal )
            {
                WinCells.Add(pos);
                return true;
            }
            return false;
        }

        private bool CheckHorizontal (FPoint pos, int piece)
        {
            int x = pos.X;
            int y = pos.Y;
            int count = 0;
            List<FPoint> winCellsHorizontal = new List<FPoint>();
            while(y >= 0 && Cells[x][y] == piece)
            {
                count++;
                winCellsHorizontal.Add(new FPoint(x, y));
                y--; 
            }

            y = pos.Y + 1;
            while (y < _boardRatio && Cells[x][y] == piece)
            {
                count++;
                y++;
                winCellsHorizontal.Add(new FPoint(x, y));
            }

            if (count >= 5)
            {
                foreach (FPoint cell in winCellsHorizontal)
                {
                    WinCells.Add(cell);
                }
                return true;
            }

            return false;
        }

        private bool CheckVertical(FPoint pos, int piece)
        {
            int x = pos.X;
            int y = pos.Y;
            int count = 0;
            List<FPoint> winCellsVertical = new List<FPoint>();
            while (x >= 0 && Cells[x][y] == piece)
            {
                count++;
                winCellsVertical.Add(new FPoint(x, y));
                x--;
            }

            x = pos.X + 1;
            
            while (x < _boardRatio && Cells[x][y] == piece)
            {
                count++;
                winCellsVertical.Add(new FPoint(x, y));
                x++;
                
            }

            if (count >= 5)
            {
                foreach (var cell in winCellsVertical)
                {
                    WinCells.Add(cell);
                }
                return true;
            }

            return false;
        }

        private bool CheckMainDiagonal(FPoint pos, int piece)
        {
            int x = pos.X;
            int y = pos.Y;
            int count = 0;
            List<FPoint> winCellMainDiagonal = new List<FPoint>();
            while (x >= 0 && y >= 0 && Cells[x][y] == piece)
            {
                count++;
                winCellMainDiagonal.Add(new FPoint(x, y));
                x--;
                y--;
                
            }

            x = pos.X + 1;
            y = pos.Y + 1;

            while (x < _boardRatio && y < _boardRatio && Cells[x][y] == piece)
            {
                count++;
                winCellMainDiagonal.Add(new FPoint(x, y));

                x++;
                y++;
            }

            if (count >= 5)
            {
                foreach (var cell in winCellMainDiagonal)
                {

                    WinCells.Add(cell);
                }
                return true;
            }

            return false;
        }

        private bool CheckSecondDiagonal(FPoint pos, int piece)
        {
            int x = pos.X;
            int y = pos.Y;
            int count = 0;
            List<FPoint> winCellSecondDiagonal = new List<FPoint>();
            while (x >= 0 && y < _boardRatio && Cells[x][y] == piece)
            {
                count++;
                winCellSecondDiagonal.Add(new FPoint(x, y));

                x--;
                y++;
            }

            x = pos.X + 1;
            y = pos.Y - 1;

            while (x < _boardRatio && y >= 0 && Cells[x][y] == piece)
            {
                count++;
                winCellSecondDiagonal.Add(new FPoint(x, y));

                x++;
                y--;
            }

            if (count >= 5)
            {
                foreach (var cell in winCellSecondDiagonal)
                {
                    WinCells.Add(cell);
                }
                return true;
            }

            return false;
        }

        #endregion
    }
}
