using System;
using System.Collections.Generic;
using WindowsFormsApp8.Pieces.Bases;
using WindowsFormsApp8.Pieces.Shapes;

namespace WindowsFormsApp8.Game
{
    public class Field
    {
        public int[,] Board { get; private set; }
        public int CellSize { get; private set; } = 16;

        public int Score = 0;
        public int HighScore = 0;

        private Random random = new Random();
        private Queue<Piece> PieceBag = new Queue<Piece>();
        public Piece CurrentPiece { get; private set; } = null;
        public Piece NextPiece { get; private set; } = null;
        Dictionary<int, int> PointsPerRow = new Dictionary<int, int>
        {
            { 1, 100 },
            { 2, 300 },
            { 3, 500 },
            { 4, 800 },
        };

        public event Action<int> ScoreChangedEvent;
        public event Action<int> HighScoreChangedEvent;
        public event Action GameOverEvent;

        private bool ValidateBoard()
        {
            if (Board == null)
                throw new ArgumentNullException(nameof(Board), "The board is not intialized yet");

            if (Board.GetLength(0) == 0 || Board.GetLength(1) == 0)
                throw new ArgumentOutOfRangeException(nameof(Board), "Board loaded with invalid range");

            return true;
        }

        private bool ValidatePiece()
        {
            if (CurrentPiece == null)
                return false;

            if (CurrentPiece.Bottom > Board.GetLength(0))
            {
                Console.WriteLine("CurrentPiece height out of range");
                return false;
            }

            if (CurrentPiece.X < 0 || CurrentPiece.Right > Board.GetLength(1))
            {
                Console.WriteLine("Piece width out of range");
                return false;
            }

            return true;
        }

        public void InitializeBoard(int columns, int rows)
        {
            if (columns <= 0)
                throw new ArgumentOutOfRangeException(nameof(columns), "Columns out of range");

            if (rows <= 0)
                throw new ArgumentOutOfRangeException(nameof(rows), "Rows out of range");

            Board = new int[rows, columns];
        }

        public void InitializeBag()
        {
            FillPieceBag();
        }

        public void ClearGame()
        {
            if (Score > HighScore)
            {
                HighScore = Score;
                HighScoreChangedEvent?.Invoke(HighScore);
            }

            CurrentPiece = null;
            NextPiece = null;

            PieceBag.Clear();

            Score = 0;
            ScoreChangedEvent?.Invoke(Score);

            for (int x = 0; x < Board.GetLength(1); x++)
            {
                for (int y = 0; y < Board.GetLength(0); y++)
                {
                    Board[y, x] = 0;
                }
            }
        }

        public void ApplyPieceToBoard()
        {
            if (!ValidateBoard())
            {
                Console.WriteLine("Invalid Board");
                return;
            }

            if (!ValidatePiece())
            {
                Console.WriteLine("Invalid piece");
                return;
            }

            for (int x = 0; x < CurrentPiece.Body.GetLength(1); x++)
            {
                for (int y = 0; y < CurrentPiece.Body.GetLength(0); y++)
                {
                    if (CurrentPiece.Body[y, x] != 0)
                    {
                        int boardX = x + CurrentPiece.X;
                        int boardY = y + CurrentPiece.Y;

                        Board[boardY, boardX] = (int)CurrentPiece.Type;
                    }
                }
            }
        }

        public void ApplyTestBoard()
        {
            for (int i = Board.GetLength(0) - 1; i >= Board.GetLength(0) - 9; i -= 2)
            {
                for (int j = Board.GetLength(1) - 1; j >= 0; j--)
                {
                    Board[i, j] = 1;
                }
            }
        }

        private int CheckAndRemoveCompletedRows()
        {
            int clearedRows = 0;

            for (int y = Board.GetLength(0) - 1; y >= CurrentPiece.Y; y--)
            {
                bool clear = true;

                for (int x = 0; x < Board.GetLength(1); x++)
                {
                    if (Board[y, x] == 0)
                    {
                        clear = false;
                        break;
                    }
                }

                if (clear)
                {
                    for (int x = 0; x < Board.GetLength(1); x++)
                    {
                        Board[y, x] = 0;
                    }
                    MoveToNextRow(y);
                    y++;
                    clearedRows++;
                }
            }

            return clearedRows;
        }

        public void ApplyPoints(int rows)
        {
            if (rows == 0)
                return;

            bool pointFound = false;

            if (PointsPerRow.TryGetValue(rows, out var points))
            {
                pointFound = true;
                Score += points;
            }

            //fallback if somehow someone manage to clear up to 5 rows or more
            if (!pointFound)
                Score += 800;

            Console.WriteLine("Points: {0}", Score);
            ScoreChangedEvent?.Invoke(Score);
        }

        private void MoveToNextRow(int startRow)
        {
            if (!ValidateBoard())
                return;

            if (startRow <= 0 || startRow > Board.GetLength(0))
                return;

            for (int y = startRow; y >= 0; y--)
            {
                for (int x = 0; x < Board.GetLength(1); x++)
                {
                    if (y == 0)
                        Board[y, x] = 0;
                    else
                        Board[y, x] = Board[y - 1, x];
                }
            }
        }

        public bool TryMovePieceLeft()
        {
            if (!ValidateBoard())
            {
                Console.WriteLine("Invalid Board");
                return false;
            }

            if (!ValidatePiece())
            {
                Console.WriteLine("Invalid piece");
                return false;
            }

            bool validMove = false;

            if (ValidatePiecePosition(-1, 0, CurrentPiece.Body))
            {
                validMove = true;
                CurrentPiece.X -= 1;
            }

            return validMove;
        }

        public bool TryMovePieceRight()
        {
            if (!ValidateBoard())
            {
                Console.WriteLine("Invalid Board");
                return false;
            }

            if (!ValidatePiece())
            {
                Console.WriteLine("Invalid piece");
                return false;
            }

            bool validMove = false;

            if (ValidatePiecePosition(1, 0, CurrentPiece.Body))
            {
                validMove = true;
                CurrentPiece.X += 1;
            }

            return validMove;
        }

        public bool TryMovePieceDown()
        {
            if (!ValidateBoard())
            {
                Console.WriteLine("Invalid Board");
                return false;
            }

            if (!ValidatePiece())
            {
                Console.WriteLine("Invalid piece");
                return false;
            }

            bool validMove = false;

            if (ValidatePiecePosition(0, 1, CurrentPiece.Body))
            {
                validMove = true;
                CurrentPiece.Y += 1;
            }

            return validMove;
        }

        public bool TryRotatePiece()
        {
            if (!ValidateBoard())
                return false;

            if (!ValidatePiece())
                return false;

            //get the new matrix
            var matrix = CurrentPiece.GetRotatedMatrix();
            int matrixRight = CurrentPiece.X + matrix.GetLength(1) - 1;
            int boardRight = Board.GetLength(1) - 1;
            int xDisplacement = 0;

            //clamp the piece to the board
            if (CurrentPiece.X + matrix.GetLength(1) - 1 >= Board.GetLength(1))
                xDisplacement = boardRight - matrixRight;

            //check if its a valid position
            if (ValidatePiecePosition(xDisplacement, 0, matrix))
            {
                CurrentPiece.X += xDisplacement;
                CurrentPiece.RotatePiece();

                return true;
            }

            int displacementLimit = 2;
            int displacementLeft = 0;
            int displacementRight = 0;

            //check with an x displacement
            for (int i = 0; i < displacementLimit; i++)
            {
                displacementLeft += 1;
                displacementRight += 1;

                //validate Left
                if (CurrentPiece.X + (xDisplacement - displacementLeft) >= 0 &&
                    ValidatePiecePosition(xDisplacement - displacementLeft, 0, matrix))
                {
                    CurrentPiece.X += xDisplacement - displacementLeft;
                    CurrentPiece.RotatePiece();

                    return true;
                }

                //validate Right
                if (CurrentPiece.X + matrix.GetLength(1) + xDisplacement + displacementRight <= boardRight &&
                    ValidatePiecePosition(xDisplacement + displacementRight, 0, matrix))
                {
                    CurrentPiece.X += xDisplacement + displacementRight;
                    CurrentPiece.RotatePiece();

                    return true;
                }
            }

            return false;
        }

        private bool ValidatePiecePosition(int xDisplacement, int yDisplacement, int[,] matrix)
        {
            bool validPosition = true;

            for (int x = 0; x < matrix.GetLength(1); x++)
            {
                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    if (matrix[y, x] == 0)
                        continue;

                    int currentX = x + xDisplacement + CurrentPiece.X;
                    int currentY = y + yDisplacement + CurrentPiece.Y;

                    if (currentX < 0 || currentX >= Board.GetLength(1) ||
                        currentY >= Board.GetLength(0))
                        return false;

                    if (Board[currentY, currentX] != 0)
                    {
                        validPosition = false;
                        break;
                    }

                }
            }

            return validPosition;
        }

        private void GenerateNewPiece()
        {
            ValidateBoard();

            if (CurrentPiece != null)
                throw new ArgumentException(nameof(CurrentPiece), "Cannot generete a new piece without disposing the previews one");

            if (PieceBag.Count == 0)
                throw new Exception("Invalid amaount in bag");

            CurrentPiece = PieceBag.Dequeue();
            CurrentPiece.X = (Board.GetLength(1) - CurrentPiece.Body.GetLength(1)) / 2 + CurrentPiece.Body.GetLength(1) / 2;

            if (!ValidatePiecePosition(0, 0, CurrentPiece.Body))
            {
                GameOverEvent?.Invoke();
                return;
            }

            if (PieceBag.Count == 0)
                FillPieceBag();

            NextPiece = PieceBag.Peek();

            Console.WriteLine("piece type: {0}", CurrentPiece.GetType());
            Console.WriteLine("next piece type: {0}", NextPiece.GetType());
            Console.WriteLine("\n");
        }

        private void FillPieceBag()
        {
            var pieces = new List<Piece>()
            {
                new LeftLShape(0, 0),
                new LeftZigZag(0, 0),
                new Line(0, 0),
                new RightLShape(0, 0),
                new RightZigZag(0, 0),
                new Square(0, 0),
                new TShape(0, 0),
            };

            random = new Random();

            while (pieces.Count > 0)
            {
                int i = random.Next(0, pieces.Count);
                var piece = pieces[i];
                pieces.RemoveAt(i);
                PieceBag.Enqueue(piece);
            }
        }

        public void TimerUpdate(object sender, EventArgs e)
        {
            if (CurrentPiece == null)
            {
                GenerateNewPiece();
                return;
            }

            if (!TryMovePieceDown())
            {
                ApplyPieceToBoard();
                int rows = CheckAndRemoveCompletedRows();
                ApplyPoints(rows);
                CurrentPiece = null;
            }
        }
    }

}
