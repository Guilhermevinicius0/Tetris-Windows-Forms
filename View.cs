using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp8.Game;
using WindowsFormsApp8.Pieces.Bases;

namespace WindowsFormsApp8
{
    public partial class View : Form
    {
        private Field _field = new Field();
        private Timer _timer = new Timer();

        private Dictionary<Keys, Func<bool>> Actions;
        private Dictionary<PieceType, Color> Colors;

        public View()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.KeyPreview = true;
        }

        private void View_Load(object sender, EventArgs e)
        {
            Invalidate();
            _field.InitializeBoard(10, 20);
            _field.InitializeBag();
            _timer.Interval = 1000;

            Actions = new Dictionary<Keys, Func<bool>>()
            {
                { Keys.Left, OnLeftPressed},
                { Keys.Right, OnRightPressed},
                { Keys.Down, OnDownPressed},
                { Keys.Up, OnSpacePressed}
            };

            Colors = new Dictionary<PieceType, Color>()
            {
                { PieceType.Default, Color.White },
                { PieceType.LeftLShape, Color.Navy },
                { PieceType.LeftZigZag, Color.Red },
                { PieceType.Line, Color.DodgerBlue },
                { PieceType.RightLShape, Color.Orange },
                { PieceType.RightZigZag, Color.Green },
                { PieceType.Square, Color.Gold },
                { PieceType.TShape, Color.Purple },
            };

            _field.ScoreChangedEvent += OnScoreChanged;
            _field.HighScoreChangedEvent += OnHighScoreChanged;
            _field.GameOverEvent += OnGameover;

            _timer.Tick += _field.TimerUpdate;
            _timer.Tick += (s, eArgs) => { this.Invalidate(); };
            _timer.Start();
        }

        private void OnScoreChanged(int score) 
        {
            if (this.InvokeRequired) 
            {
                this.BeginInvoke(new Action(() => OnScoreChanged(score)));
                return;
            }

            lbl_Score.Text = score.ToString();
        }

        private void OnHighScoreChanged(int score)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => OnHighScoreChanged(score)));
                return;
            }

            lbl_HighScore.Text = score.ToString();
        }

        public void OnGameover() 
        {
            if (this.InvokeRequired) 
            {
                this.BeginInvoke(new Action(() => OnGameover()));
                return;
            }

            _timer.Stop();

            if (MessageBox.Show(this, "Want to try again?", "GameOver", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            RestartGame();
        }

        private void RestartGame()
        {
            _field.Restart();
            _timer.Start();
        }

        private bool OnSpacePressed()
            => _field.TryRotatePiece();
        

        private bool OnLeftPressed()
           => _field.TryMovePieceLeft();


        private bool OnRightPressed()
            => _field.TryMovePieceRight();

        private bool OnDownPressed()
            => _field.TryMovePieceDown();

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawBoard(_field.Board, _field.CellSize, e.Graphics);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (Actions.TryGetValue(e.KeyCode, out var action))
            {
                if (action.Invoke())
                    this.Invalidate();
            }

            if (e.KeyCode == Keys.F) 
            {
                _field.ApplyTestBoard();
            }

            if (e.KeyCode == Keys.R) 
            {
                RestartGame();
            }
        }

        private void DrawBoard(int[,] board, int cellSize, Graphics g)
        {
            if (board == null)
                throw new ArgumentNullException(nameof(board), "the board must be not null");

            if (board.GetLength(0) == 0 || board.GetLength(1) == 0)
                throw new ArgumentOutOfRangeException(nameof(board), "the board must contain colunms and rows");

            if (g == null)
                throw new ArgumentNullException(nameof(g), "The graphics GDI must be not null");

            if (cellSize <= 0)
                throw new ArgumentException("The cell size must be greater the zero", nameof(cellSize));

            int starterX = (this.ClientSize.Width / 2) - board.GetLength(1) * cellSize / 2;
            int starterY = (this.ClientSize.Height / 2) - (board.GetLength(0) + 1) * cellSize / 2;

            for (int x = 0; x < board.GetLength(1); x++)
            {
                for (int y = 0; y < board.GetLength(0); y++)
                {
                    int currentX = x + (cellSize * x) + starterX;
                    int currentY = y + (cellSize * y) + starterY;

                    bool isOccupiedBoard = board[y, x] != 0;
                    bool isOccupiedPiece = (_field.CurrentPiece?.IsOccupied(x, y) ?? false);

                    //To use at home so i can see how its going to look in the final render

                    var currentColor = Color.White;

                    if (!isOccupiedBoard && isOccupiedPiece)
                        currentColor = Colors.TryGetValue(_field?.CurrentPiece?.Type ?? PieceType.Default, out var color) ? color : Color.White;
                    else if (isOccupiedBoard && !isOccupiedPiece)
                        currentColor = Colors.TryGetValue((PieceType)board[y, x], out var color) ? color : Color.White;

                    using (var brush = new SolidBrush(currentColor))
                        g.FillRectangle(brush, currentX, currentY, cellSize, cellSize);

                    //To Use at Work so people think is something serious

                    //var currentCell = "0";

                    //if (!isOccupiedBoard && isOccupiedPiece)
                    //    currentCell = ((int)(_field.CurrentPiece?.Type ?? PieceType.Default)).ToString();
                    //else if (isOccupiedBoard && !isOccupiedPiece)
                    //    currentCell = board[y, x].ToString();

                    //using (var brush = new SolidBrush(Color.Black))
                    //    g.DrawString(currentCell, this.Font, brush, currentX, currentY);
                }
            }
        }

        private void View_FormClosing(object sender, FormClosingEventArgs e)
        {
            _field.ScoreChangedEvent -= OnScoreChanged;
            _field.HighScoreChangedEvent -= OnHighScoreChanged;
            _field.GameOverEvent -= OnGameover;
        }
    }
}
