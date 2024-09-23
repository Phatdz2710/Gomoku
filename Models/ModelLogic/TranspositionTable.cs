using Caro.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caro.Models.ModelLogic
{
    public class TranspositionTable
    {
        public int Score;
        public int Depth;
        public FPoint PosMove = FPoint.NULL;
        public int Alpha;
        public int Beta;
    }
}
