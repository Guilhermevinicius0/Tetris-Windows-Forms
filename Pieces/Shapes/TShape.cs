using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp8.Pieces.Bases;

namespace WindowsFormsApp8.Pieces.Shapes
{
    public class TShape : Piece
    {
        public override int[,] Body { get; protected set; }
        public TShape(int x, int y) : base(x, y)
        {
            Body = new int[,]
            {
                { 0, 1, 0 },
                { 1, 1, 1 }
            };

            Type = PieceType.TShape;
        }
    }
}
