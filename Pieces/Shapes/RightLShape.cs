using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp8.Pieces.Bases;

namespace WindowsFormsApp8.Pieces.Shapes
{
    public class RightLShape : Piece
    {
        public override int[,] Body { get; protected set; }
     
        public RightLShape()
        {
            Body = new int[,]
            {
                { 1, 0 },
                { 1, 0 },
                { 1, 1 },
            };

            Type = PieceType.RightLShape;
        }
    }
}
