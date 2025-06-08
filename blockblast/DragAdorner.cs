using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace blockblast
{
    public sealed class DragAdorner : Adorner
    {
        private readonly VisualBrush _brush;
        private Point _offset;    // offset from top-left of control to cursor
        private double _left;     // position on the adorner layer
        private double _top;

        public DragAdorner(UIElement adorned, UIElement visual, Point cursorOffset)
            : base(adorned)
        {
            _brush = new VisualBrush(visual)
            {
                Opacity = 0.8,
                Stretch = Stretch.None,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top
            };

            _offset = cursorOffset;
            IsHitTestVisible = false;
        }

        public void UpdatePosition(Point pos)
        {
            _left = pos.X - _offset.X;
            _top = pos.Y - _offset.Y;
            InvalidateVisual(); // request re-render
        }

        protected override void OnRender(DrawingContext dc)
        {
            Size size = AdornedElement.RenderSize;

            dc.DrawRectangle(
                _brush,
                null,
                new Rect(new Point(_left, _top), size)
            );
        }
    }
}
