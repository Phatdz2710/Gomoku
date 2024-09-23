using Caro.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Caro.Models.Player
{
    public abstract class IPlayer
    {
        public int Score { get; set; } 
        public string Name { get; set; }

        protected Board _board;
        public bool IsThinking = false;
        protected CellState _piece;

        public virtual CellState Piece { get => _piece; set => _piece = value; }

        public IPlayer(Board _board, CellState _piece, string _name)
        {
            this.Name = _name;
            this._board = _board;
            this._piece = _piece;
            this.Score = 0;
        }

        public abstract void MakeMove(FPoint pos);

        protected abstract FPoint FindBestMove();

        public void SetPiece(CellState piece)
        {
            this._piece = piece; 
        }

        public abstract void CancelMove();

        public abstract void ClearZobristMap();

    }
}
