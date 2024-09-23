using Caro.Models.Enums;
using Caro.Models.ModelLogic;
using Caro.Models.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Caro.Models.Player
{
    public class HumanPlayer : IPlayer
    {
        public HumanPlayer(Board _board, CellState _piece, string _name)
            : base(_board, _piece, _name) { }

        public override void MakeMove(FPoint pos)
        {
            _board.DoPlayAt(pos);
        }

        protected override FPoint FindBestMove()
        {
            return FPoint.NULL;
        }

        public override void CancelMove()
        {
            throw new NotImplementedException();
        }

        public override void ClearZobristMap()
        {
            throw new NotImplementedException();
        }
    }
}
