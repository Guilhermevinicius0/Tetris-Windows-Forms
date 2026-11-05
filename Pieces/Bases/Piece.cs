using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp8.Pieces.Bases
{
    public abstract class Piece
    {
        public abstract int[,] Body { get; protected set; }
        public int X { get; set; }
        public int Y { get; set; }

        public int Bottom { get => Y + Body.GetLength(0) - 1; }
        public int Right { get => X + Body.GetLength(1) - 1; }

        public PieceType Type { get; protected set; }

        public Piece(int x, int y)
        {
            X = x;
            Y = y;
        }

        public virtual bool IsOccupied(int x, int y)
        {
            if (x < X || x > Right || y < Y || y > Bottom)
                return false;

            return Body[y - Y, x - X] != 0;
        }

        public int[,] GetRotatedMatrix() 
        {
            var result = new int[Body.GetLength(1), Body.GetLength(0)];

            for (int x = 0; x < Body.GetLength(1); x++) 
            {
                for (int y = Body.GetLength(0) - 1; y >= 0; y--) 
                {
                    result[x, y] = Body[Body.GetLength(0) - 1 - y, x];
                } 
            }

            return result;
        }

        public void RotatePiece() 
        {
            Body = GetRotatedMatrix();
        }
    }

}
