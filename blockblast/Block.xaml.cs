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
    /// Interaction logic for Block.xaml
    /// </summary>
    public partial class Block : UserControl
    {
        public int Size { get; private set; }
        public SolidColorBrush BlockColor { get; private set; }

        public Block(SolidColorBrush color, int size)
        {
            InitializeComponent();
            
            this.BlockColor = color;

            SetSize(size);
            SetColor(color);
        }

        public void SetSize(int size)
        {
            this.Size = size;
            Width = size;
            Height = size;

            BlockRectangle.Width = size;
            BlockRectangle.Height = size;
        }

        private void SetColor(SolidColorBrush color)
        {
            var darkened = new SolidColorBrush(
                Color.FromArgb(
                    255,
                    (byte)(color.Color.R * 0.8),
                    (byte)(color.Color.G * 0.8),
                    (byte)(color.Color.B * 0.8)
                )
            );
            BlockRectangle.Fill = color;
            BlockRectangle.Stroke = darkened;
        }
    }
}
