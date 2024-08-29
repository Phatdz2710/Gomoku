using Caro.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caro.Models
{
    public class PlayResult
    {
        public FPoint Position { get;  set; }
        public CellState Piece { get;  set; }
        public GameState GameState { get;  set; }

        public PlayResult(FPoint position, CellState piece, GameState gameState)
        {
            this.Position = position;
            this.Piece = piece;
            this.GameState = gameState;
        }
    }
}
