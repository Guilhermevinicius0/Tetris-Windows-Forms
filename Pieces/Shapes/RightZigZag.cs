using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp8.Pieces.Bases;

namespace WindowsFormsApp8.Pieces.Shapes
{
    public class RightZigZag : Piece
    {
        public override int[,] Body { get; protected set; }
        public RightZigZag(int x, int y) : base(x, y)
        {
            Body = new int[,]
            {
                { 0, 1, 1 },
                { 1, 1, 0 }
            };

            Type = PieceType.RightZigZag;
        }
    }
}
