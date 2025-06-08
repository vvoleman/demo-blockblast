using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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

        public Inventory? Inventory { get; set; }

        public int Score = 0;
        private Block?[,] _boardData = new Block?[BoardSize, BoardSize];

        public GameBoardControl()
        {
            InitializeComponent();
            AllowDrop = true;
            SizeChanged += (_, _) => RenderBoard();

            //SetBoard(GenerateDummyBlocks());
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
                        // Check if canvas already has block, if not, add
                        if (!GameCanvas.Children.Contains(block))
                        {
                            GameCanvas.Children.Add(block);
                        }

                        // Set position
                        Canvas.SetLeft(block, col * block.Size);
                        Canvas.SetTop(block, row * block.Size);

                        block.Visibility = Visibility.Visible;

                    }
                }
            }
        }

        public void CheckBoard()
        {
            List<Point> clearList = new List<Point>();
            int removedLineCount = 0;

            // We need to check horizontal lines and vertical lines

            // First, columns
            for (int i = 0; i < BoardSize; i++)
            {
                bool isCleared = true;
                for (int j = 0; j < BoardSize; j++)
                {
                    if (_boardData[i, j] == null)
                    {
                        isCleared = false;
                        break;
                    }
                }

                if (!isCleared)
                {
                    continue;
                }

                removedLineCount++;

                for (int j = 0;j < BoardSize; j++)
                {
                    clearList.Add(new Point(i, j));
                }
            }

            // Second, rows
            for (int j =0; j < BoardSize;j++)
            {
                bool isCleared = true;
                for(int i  = 0; i < BoardSize; i++)
                {
                    if (_boardData[i, j] == null)
                    {
                        isCleared = false;
                        break;
                    }
                }

                if (!isCleared)
                {
                    continue;
                }

                removedLineCount++;

                for (int i = 0; i < BoardSize; i++)
                {
                    clearList.Add(new Point(i, j));
                }
            }

            // Then, we remove it
            for (int i = 0; i < clearList.Count; i++)
            {
                var p = clearList[i];
                _boardData[(int)p.X, (int)p.Y] = null;
            }

            Score += removedLineCount;
        }

        private void CheckPossibleMoves()
        {
            var groups = Inventory!.BlocksGroups;
            var emptyPlaces = new List<Point>();

            for (int i = 0; i < _boardData.GetLength(0); i++)
            {
                for(int j = 0; j < _boardData.GetLength(1); j++)
                {
                    if (_boardData[i,j] == null)
                    {
                        emptyPlaces.Add(new Point(i,j));
                    }
                }
            }

            List<BlockGroup> groupList = new List<BlockGroup>();
            int blockCount = 0;

            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i] != null)
                {
                    blockCount += groups[i]!.GetBlockCount();
                    groupList.Add(groups[i]!);
                }
            }

            bool canFit = false;
            if (emptyPlaces.Count > blockCount)
            {
                canFit = CanAnyGroupFit(emptyPlaces, groupList);
            }

            if (!canFit)
            {
                MessageBox.Show("No possible moves. Game over!");
            }
        }

        public bool CanAnyGroupFit(List<Point> filledPoints, List<BlockGroup> groups)
        {
            var pointSet = new HashSet<Point>(filledPoints);

            while (pointSet.Count > 0)
            {
                var start = pointSet.First();

                // Expand to max width
                int width = 0;
                while (pointSet.Contains(new Point(start.X + width, start.Y)))
                    width++;

                // Expand to max height
                int height = 1;
                bool stop = false;
                while (!stop)
                {
                    for (int dx = 0; dx < width; dx++)
                    {
                        if (!pointSet.Contains(new Point(start.X + dx, start.Y + height)))
                        {
                            stop = true;
                            break;
                        }
                    }
                    if (!stop) height++;
                }

                // Try each group inside this rectangle
                foreach (var group in groups)
                {
                    int gRows = group.Mask.GetLength(0);
                    int gCols = group.Mask.GetLength(1);

                    if (gCols <= width && gRows <= height)
                        return true; // Found a fit, stop immediately
                }

                // Remove the current block from set
                for (int dx = 0; dx < width; dx++)
                    for (int dy = 0; dy < height; dy++)
                        pointSet.Remove(new Point(start.X + dx, start.Y + dy));
            }

            return false; // No fits found
        }



        private bool CanPlace(List<Point> positions)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                var p = positions[i];
                // Check if position is inside of bounds of _boardData
                if (p.X < 0 || p.X >= BoardSize ||
                    p.Y < 0 || p.Y >= BoardSize)
                {
                    return false;
                }

                // Check if position is already occupied
                if (_boardData[(int)p.Y, (int)p.X] != null)
                {
                    return false;
                }
            }

            return true;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            if (e.Data.GetDataPresent(typeof(BlockGroup)))
                e.Effects = DragDropEffects.Move;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (e.Data.GetDataPresent(typeof(BlockGroup)))
            {
                var blockGroup = (BlockGroup)e.Data.GetData(typeof(BlockGroup));

                if (blockGroup.Parent is Panel oldParent)
                    oldParent.Children.Remove(blockGroup);

                //GameCanvas.Children.Add(blockGroup);

                Point dropPos = e.GetPosition(this);

                List<Point> positions = new List<Point>();
                List<Block> blockList = new List<Block>();
                var blocks = blockGroup.Blocks;
                int startX = (int)Math.Floor(dropPos.X / BlockSize);
                int startY = (int)Math.Floor(dropPos.Y / BlockSize);

                // Loop through the blocks in the group
                for (int row = 0; row < blocks.GetLength(0); row++)
                {
                    for (int col = 0; col < blocks.GetLength(1); col++)
                    {
                        if (blocks[row, col] != null)
                        {
                            int x = startX + col;
                            int y = startY + row;
                            // Calculate the position of the block in the game board
                            Point pos = new Point(x, y);
                            positions.Add(pos);
                            blockList.Add(blocks[row, col]!);
                        }
                    }
                }

                if (CanPlace(positions))
                {
                    // Take blocks, remove them from the block group and place them in the game board
                    // Then remove the block group from the game canvas
                    // Flatten blocks array
                    for (int i = 0; i < blockList.Count; i++)
                    {
                        blockGroup.RemoveBlock(blockList[i]);
                        _boardData[(int)positions[i].Y, (int)positions[i].X] = blockList[i];
                    }

                    Inventory!.Remove(blockGroup);
                } else
                {
                    // If the position is not valid, remove the block group
                    //GameCanvas.Children.Remove(blockGroup);
                    MessageBox.Show("Cannot place block group here!");
                }

               

                bool isEmpty = true;
                foreach (var group in Inventory!.BlocksGroups)
                {
                    if (group != null)
                    {
                        isEmpty = false;
                        break;
                    }
                }

                if (isEmpty)
                {
                    Inventory.GenerateGroups();
                }

                CheckBoard();

                Inventory.Render();
                var inventoryGroups = Inventory.BlocksGroups;
                RenderBoard();

                CheckPossibleMoves();
            }
        }
    }
}
