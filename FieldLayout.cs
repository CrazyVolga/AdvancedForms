using System.Drawing;

namespace AdvancedForms
{
    public class FieldLayout
    {
        public Rectangle LabelRect;
        public Rectangle ValueRect;

        public Rectangle FullRect => Rectangle.Union(LabelRect, ValueRect);

        public FieldLayout(Rectangle fieldLable, Rectangle fieldValue)
        {
            this.LabelRect = fieldLable;
            this.ValueRect = fieldValue;
        }
    }
}
