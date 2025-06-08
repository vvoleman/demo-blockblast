using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            Blocks = new Block?[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (Mask[i, j] == 1)
                    {
                        Blocks[i, j] = new Block(new SolidColorBrush(Color), GameBoardControl.BlockSize);
                    }
                    else
                    {
                        Blocks[i, j] = null;
                    }
                }
            }

            return Blocks;
        }

        public void RenderGroup(bool isInInventory)
        {
            GroupCanvas.Children.Clear();
            int rows = Mask.GetLength(0);
            int cols = Mask.GetLength(1);
            int blockSize = !isInInventory ? 50 : 20;
            double width = cols * blockSize;
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
                        GroupCanvas.Children.Add(block);
                        Canvas.SetLeft(block, col * blockSize);
                        Canvas.SetTop(block, row * blockSize);
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Package the data.
                DataObject data = new DataObject();
                data.SetData(DataFormats.StringFormat, Blocks.ToString());
                data.SetData("Double", GroupCanvas.Height);
                data.SetData("Object", this);

                // Initiate the drag-and-drop operation.
                DragDrop.DoDragDrop(this, this, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);
            // These Effects values are set in the drop target's
            // DragOver event handler.
            if (e.Effects.HasFlag(DragDropEffects.Copy))
            {
                Mouse.SetCursor(Cursors.Cross);
            }
            else if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                Mouse.SetCursor(Cursors.Pen);
            }
            else
            {
                Mouse.SetCursor(Cursors.No);
            }
            e.Handled = true;
        }
    }
}
