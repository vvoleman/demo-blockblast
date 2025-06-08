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
        public const int InventorySize = 3;

        public BlockGroup?[] BlocksGroups { get; private set; } = new BlockGroup?[InventorySize];

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

            for (int i = 0; i < InventorySize; i++)
            {
                var color = colors[Random.Shared.Next(colors.Count)];
                var mask = masks[Random.Shared.Next(masks.Count)];

                BlocksGroups[i] = new BlockGroup(color, mask);
            }
        }

        public void Render()
        {
            // Remove content of the Container
            Container.Children.Clear();

            for (int i = 0; i < BlocksGroups.Length; i++)
            {
                if (BlocksGroups[i] != null)
                {
                    var group = BlocksGroups[i]!;

                    group.RenderGroup(true);
                    Container.Children.Add(BlocksGroups[i]);
                }
            }
        }

        public void Remove(BlockGroup blockGroup)
        {
            Container.Children.Remove(blockGroup);
            for (int i = 0; i < BlocksGroups.Length; i++)
            {
                if (BlocksGroups[i] == blockGroup)
                {
                    BlocksGroups[i] = null;
                    break;
                }
            }
            Render();
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
                },
                new int[,]
                {
                    { 1 },
                },
                new int[,]
                {
                    { 1,1 },
                },
                new int[,]
                {
                    { 1,1 },
                    { 1,1 },
                    { 1,1 },
                    { 1,1 },
                    { 1,1 },
                    { 1,1 },
                }
            ];
        }
    }
}
