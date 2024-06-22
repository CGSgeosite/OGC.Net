namespace Geosite
{
    public class ToolStripSpringTextBox : ToolStripTextBox
    {
        public override Size GetPreferredSize(Size constrainingSize)
        {
            if (IsOnOverflow || Owner!.Orientation == Orientation.Vertical)
                return DefaultSize;

            var width = Owner.DisplayRectangle.Width;

            if (Owner.OverflowButton.Visible)
                width = width - Owner.OverflowButton.Width - Owner.OverflowButton.Margin.Horizontal;

            var springBoxCount = 0;

            foreach (ToolStripItem item in Owner.Items)
            {
                if (item.IsOnOverflow)
                    continue;
                if (item is ToolStripSpringTextBox)
                {
                    springBoxCount++;
                    width -= item.Margin.Horizontal;
                }
                else
                    width = width - item.Width - item.Margin.Horizontal;
            }

            if (springBoxCount > 1)
                width /= springBoxCount;

            if (width < DefaultSize.Width)
                width = DefaultSize.Width;

            var size = base.GetPreferredSize(constrainingSize);
            size.Width = width;
            return size;
        }
    }
}
