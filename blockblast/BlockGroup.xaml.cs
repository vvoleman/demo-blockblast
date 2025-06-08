using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for BlockGroup.xaml
    /// </summary>
    public partial class BlockGroup : UserControl
    {
        public Color Color { get; private set; }
        public int[,] Mask { get; private set; }

        private Point _dragStart;
        private DragAdorner? _adorner;
        private AdornerLayer? _layer;
        private Point _lastMousePosition;

        public Block?[,] Blocks { get; private set; }

        public BlockGroup(Color color, int[,] mask)
        {
            InitializeComponent();

            this.Color = color;
            this.Mask = mask;
            this.Blocks = GenerateBlocks();
            
        }

        private Block?[,] GenerateBlocks()
        {
            int rows = Mask.GetLength(0);
            int cols = Mask.GetLength(1);
            var blocks = new Block?[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (Mask[i, j] == 1)
                    {
                        blocks[i, j] = new Block(new SolidColorBrush(Color), GameBoardControl.BlockSize);
                    }
                    else
                    {
                        blocks[i, j] = null;
                    }
                }
            }

            return blocks;
        }

        public void RemoveBlock(Block block)
        {
            GroupCanvas.Children.Remove(block);
            for (int i = 0; i < Blocks.GetLength(0); i++)
            {
                for (int j = 0; j < Blocks.GetLength(1); j++)
                {
                    if (Blocks[i, j] == block)
                    {
                        Blocks[i, j] = null;
                        return;
                    }
                }
            }
        }

        public int GetBlockCount()
        {
            int count = 0;

            for (int i = 0; i < Blocks.GetLength(0); i++)
            {
                for (int j = 0; j < Blocks.GetLength(1); j++)
                {
                    if (Blocks[i, j] != null)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public void RenderGroup(bool isInInventory)
        {
            GroupCanvas.Children.Clear();
            int rows = Mask.GetLength(0);
            int cols = Mask.GetLength(1);
            int blockSize = !isInInventory ? 20 : 20;
            double width = cols * blockSize;

            Margin = new Thickness(10, 0, 0, 0);


            double height = rows * blockSize;
            GroupCanvas.Width = width;
            GroupCanvas.Height = height;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (Blocks[row, col] != null)
                    {
                        Block block = Blocks[row, col]!;
                        block.SetSize(blockSize);
                        if (!GroupCanvas.Children.Contains(block))
                        {
                            GroupCanvas.Children.Add(block);
                        }
                        Canvas.SetLeft(block, col * blockSize);
                        Canvas.SetTop(block, row * blockSize);
                    }
                }
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _dragStart = e.GetPosition(this);
            CaptureMouse();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            Point cur = e.GetPosition(this);
            Vector diff = cur - _dragStart;
            if (Math.Abs(diff.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(diff.Y) < SystemParameters.MinimumVerticalDragDistance)
                return;

            ReleaseMouseCapture();

            if (Window.GetWindow(this) == null)
            {
                return;
            }

            // 1️⃣  create adorner
            //_layer = AdornerLayer.GetAdornerLayer(Window.GetWindow(this)?.Content as Visual);
            //_adorner = new DragAdorner(this, this, _dragStart);
            //_layer?.Add(_adorner);

            // 2️⃣  handle feedback so we can move the adorner
            GiveFeedback += BlockGroup_GiveFeedback;
            RenderGroup(false);
            var data = new DataObject(typeof(BlockGroup), this);

            DragDrop.DoDragDrop(this, data, DragDropEffects.Move);

            // 3️⃣  cleanup
            //GiveFeedback -= BlockGroup_GiveFeedback;
            //_layer?.Remove(_adorner);
            //_adorner = null;
            //_layer = null;
        }

        private void BlockGroup_GiveFeedback(object? sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;

            if (_adorner != null && _layer != null)
            {
                //Point pos = GetMousePosition();
                // Get the position of this control relative to main window
                //Point blockPos = TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));

                //Point diff = new Point(new Random().Next(0,50), new Random().Next(0, 50));

                // TODO: Finish later
                // _adorner.UpdatePosition(diff);
            }

            e.Handled = true;
        }

        private Point GetMousePosition()
        {
            var window = Application.Current.MainWindow;
            return Mouse.GetPosition(window);
        }

    }
}
