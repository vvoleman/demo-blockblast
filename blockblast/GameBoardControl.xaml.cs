using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace blockblast
{
    /// <summary>
    /// Interaction logic for GameBoardControl.xaml
    /// </summary>
    public partial class GameBoardControl : UserControl
    {
        // make const
        public const int BlockSize = 50;
        public const int BoardSize = 8;

        private Block?[,] _boardData = new Block?[BoardSize, BoardSize];

        public GameBoardControl()
        {
            InitializeComponent();
            SizeChanged += (_, _) => RenderBoard();

            SetBoard(GenerateDummyBlocks());
            RenderBoard();
        }

        public void SetBoard(Block?[,] boardData)
        {
            _boardData = boardData;
            RenderBoard();
        }

        private void RenderBoard()
        {
            if (_boardData == null)
                return;

            GameCanvas.Children.Clear();

            int rows = _boardData.GetLength(0);
            int cols = _boardData.GetLength(1);

            double width = BoardSize * BlockSize;
            double height = BoardSize * BlockSize;

            GameCanvas.Width = width;
            GameCanvas.Height = height;


            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (_boardData[row, col] != null)
                    {
                        Block block = _boardData[row, col]!;

                        block.Visibility = Visibility.Hidden;
                        GameCanvas.Children.Add(block);

                        // Set position
                        Canvas.SetLeft(block, col * block.Size);
                        Canvas.SetTop(block, row * block.Size);

                        block.Visibility = Visibility.Visible;

                    }
                }
            }
        }

        private Block?[,] GenerateDummyBlocks()
        {
            int size = 50;

            Color[] colors = new Color[]
            {
                Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow,
            };
            int?[,] values = new int?[8, 8]
            {
                { 0, 0, 0, null, null, null, null, 2 },
                { 0, 1, 1, 1, 1, null, null, 2 },
                { null, 1, null, 1, 1, null, null, null },
                { null, null, 1, null, null, null, null, null },
                { null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null },
                { null ,null ,null ,null ,null ,null ,null ,null }
            };

            Block?[,] blocks = new Block?[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (values[i, j] != null)
                    {
                        SolidColorBrush color = new SolidColorBrush(colors[(int)values[i, j]! % colors.Length]);
                        blocks[i, j] = new Block(color, size);
                    }
                    else
                    {
                        blocks[i, j] = null;
                    }
                }
            }

            return blocks;

        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            Point dropPosition = e.GetPosition(GameCanvas);
            ref var block = ref _boardData[(int)(dropPosition.Y / BlockSize), (int)(dropPosition.X / BlockSize)];
            if (block == null && e.Data.GetDataPresent(typeof(Block)))
            {
                block = e.Data.GetData(typeof(Block)) as Block;
                if (block != null)
                {
                    block.Visibility = Visibility.Visible;
                    Canvas.SetLeft(block, (int)(dropPosition.X / BlockSize) * BlockSize);
                    Canvas.SetTop(block, (int)(dropPosition.Y / BlockSize) * BlockSize);
                }
            }
        }
    }
}
