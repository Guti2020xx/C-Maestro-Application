using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Database_Control
{
    class ItemBtn : GroupBox
    {
        private EventHandler? ClickItem;
        public event EventHandler OnItemClick
        {
            add
            {
                ClickItem += value;
                Click += value;
            }
            remove
            {
                ClickItem -= value;
                Click -= value;
            }
        }

        public ItemBtn()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        public void ActivateClickItem()
        {
            ClickItem?.Invoke(this, EventArgs.Empty);
        }

        private Color borderColor = Color.Black;
        [DefaultValue(typeof(Color), "Black")]
        public Color BorderColor
        {
            get { return borderColor; }
            set { borderColor = value; this.Invalidate(); }
        }
        private Color textColor = Color.Black;
        [DefaultValue(typeof(Color), "Black")]
        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; this.Invalidate(); }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            GroupBoxState state = base.Enabled ? GroupBoxState.Normal :
                GroupBoxState.Disabled;
            TextFormatFlags flags = TextFormatFlags.PreserveGraphicsTranslateTransform |
                TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.TextBoxControl |
                TextFormatFlags.WordBreak;
            Color titleColor = this.TextColor;
            if (!this.ShowKeyboardCues)
                flags |= TextFormatFlags.HidePrefix;
            if (this.RightToLeft == RightToLeft.Yes)
                flags |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
            if (!this.Enabled)
                titleColor = SystemColors.GrayText;
            DrawUnthemedGroupBoxWithText(e.Graphics, new Rectangle(0, 0, base.Width,
                base.Height), this.Text, this.Font, titleColor, flags, state);
            RaisePaintEvent(this, e);
        }
        private void DrawUnthemedGroupBoxWithText(Graphics g, Rectangle bounds,
            string groupBoxText, Font font, Color titleColor,
            TextFormatFlags flags, GroupBoxState state)
        {
            Rectangle rectangle = bounds;
            rectangle.Width -= 8;
            Size size = TextRenderer.MeasureText(g, groupBoxText, font,
                new Size(rectangle.Width, rectangle.Height), flags);
            rectangle.Width = size.Width;
            rectangle.Height = size.Height;
            if ((flags & TextFormatFlags.Right) == TextFormatFlags.Right)
                rectangle.X = (bounds.Right - rectangle.Width) - 8;
            else
                rectangle.X += 8;
            TextRenderer.DrawText(g, groupBoxText, font, rectangle, titleColor, flags);
            if (rectangle.Width > 0)
                rectangle.Inflate(2, 0);
            using (var pen = new Pen(this.BorderColor))
            {
                int num = bounds.Top + (font.Height / 2);
                g.DrawLine(pen, bounds.Left, num - 1, bounds.Left, bounds.Height - 2);
                g.DrawLine(pen, bounds.Left, bounds.Height - 2, bounds.Width - 1,
                    bounds.Height - 2);
                g.DrawLine(pen, bounds.Left, num - 1, rectangle.X - 3, num - 1);
                g.DrawLine(pen, rectangle.X + rectangle.Width + 2, num - 1,
                    bounds.Width - 2, num - 1);
                g.DrawLine(pen, bounds.Width - 2, num - 1, bounds.Width - 2,
                   bounds.Height - 2);
            }
        }
    }

    public class InputField : TextBox
    {
        public delegate void InputEnd(string Text);
        public InputEnd OnEnterKey;
        public Action OnEnd;

        public InputField()
        {
            this.KeyDown += Text_KeyDown;
        }

        private void Text_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (OnEnterKey != null && OnEnd != null)
                {
                    OnEnterKey(Text);
                    OnEnd();
                }
            }
        }
    }
}
