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
    /// Interaction logic for Inventory.xaml
    /// </summary>
    public partial class Inventory : UserControl
    {
        public BlockGroup?[] blocksGroups { get; private set; } = new BlockGroup?[3];

        public Inventory()
        {
            InitializeComponent();
        }

        public void GenerateGroups()
        {
            var masks = getMasks();
            var colors = new List<Color>
            {
                Colors.Red,
                Colors.Green,
                Colors.Blue,
                Colors.Yellow,
                Colors.Purple,
                Colors.Orange
            };

            for (int i = 0; i < masks.Count; i++)
            {
                var color = colors[Random.Shared.Next(colors.Count - 1)];
                var mask = masks[Random.Shared.Next(masks.Count - 1)];

                blocksGroups[i] = new BlockGroup(color, mask);
            }
        }

        public void Render()
        {
            // Remove content of the Container
            Container.Children.Clear();

            for (int i = 0; i < blocksGroups.Length; i++)
            {
                if (blocksGroups[i] != null)
                {
                    var group = blocksGroups[i]!;

                    if (i != 0)
                    {
                        group.Margin = new Thickness(10, 0, 0, 0);
                    }

                    group.Margin = new Thickness(10, 0, 0, 0);
                    group.RenderGroup(true);
                    Container.Children.Add(blocksGroups[i]);
                }
            }
        }

        private List<int[,]> getMasks()
        {
            return
            [
                new int[,]
                {
                    { 1, 1 },
                    { 1, 1 },
                },
                new int[,]
                {
                    { 1, 0, 0 },
                    { 1, 1, 1 },
                },
                new int[,]
                {
                    { 0, 1, 0 },
                    { 1, 1, 1 },
                }
            ];
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            Point p = e.GetPosition(Container);

            // Get element from event
            if (e.Data.GetDataPresent("Object"))
            {
                var group = e.Data.GetData("Object") as BlockGroup;
                if (group != null)
                {
                    Canvas.SetLeft(group, p.X - group.Width / 2);
                    Canvas.SetTop(group, p.Y - group.Height / 2);
                }

            }
        }
    }
}
