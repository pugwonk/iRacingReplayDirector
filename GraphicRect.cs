// This file is part of iRacingReplayOverlay.
//
// Copyright 2014 Dean Netherton
// https://github.com/vipoo/iRacingReplayOverlay.net
//
// iRacingReplayOverlay is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// iRacingReplayOverlay is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with iRacingReplayOverlay.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace iRacingReplayOverlay.net
{
	public class GraphicRect
    {
		protected readonly Graphics g;
		protected readonly Rectangle r;
		protected readonly Brush b;
		protected readonly Pen p;
		protected readonly Font f;
		protected readonly StringFormat sf;

        public GraphicRect(Graphics g, Rectangle r)
        {
            this.g = g;
            this.r = r;
        }

		internal GraphicRect(Graphics g, Rectangle r, Brush b, Pen p, Font f, StringFormat sf)
        {
            this.g = g;
            this.r = r;
            this.b = b;
            this.p = p;
            this.f = f;
            this.sf = sf;
        }

		protected virtual GraphicRect New(Graphics g, Rectangle r, Brush b, Pen p, Font f, StringFormat sf)
		{
			return new GraphicRect(g, r, b, p, f, sf);
		}

        internal GraphicRect WithLinearGradientBrush(Color color1, Color color2, LinearGradientMode linearGradientMode)
        {
			return New(g, r, new LinearGradientBrush(r, color1, color2, linearGradientMode), p, f, sf);
        }

        internal GraphicRect With(Func<GraphicRect, GraphicRect> modifiers)
        {
            return modifiers(this);
        }

        internal GraphicRect WithPen(Pen pen)
        {
			return New(g, r, b, pen, f, sf);
        }

        internal virtual GraphicRect DrawRectangleWithBorder()
        {
            g.FillRectangle(b, r);
            g.DrawRectangle(p, r);
            return this;
        }

        const int TEXT_LEFT_OFFSET_MAGIC = -4;
        const int TEXT_RIGHT_PADDING_MAGIC = 10;
		internal virtual GraphicRect DrawText(string text, int leftOffset = 0)
        {
            var rect2 = new Rectangle(r.Left + leftOffset + TEXT_LEFT_OFFSET_MAGIC, r.Top, r.Width + TEXT_RIGHT_PADDING_MAGIC, r.Height);
            g.DrawString(text, f, b, rect2, sf);
            return this;
        }

        public GraphicRect AfterText(string str, int i = 0)
        {
            var size = TextRenderer.MeasureText(g, str, f, new Size(0,0), TextFormatFlags.NoPadding);

            var newRect = new Rectangle(r.Left + (int)size.Width + i, r.Top, r.Width - (int)size.Width - i, r.Height);
            return New(g, newRect, b, p, f, sf);
        }

        public GraphicRect MoveRight(int right)
        {
            return New(g, new Rectangle(r.Left + right, r.Top, r.Width + right, r.Height), b, p, f, sf);
        }

        internal GraphicRect WithBrush(Brush brush)
        {
			return New(g, r, brush, p, f, sf);
        }

        internal GraphicRect WithFont(string prototype, float emSize, FontStyle style)
        {
			return New(g, r, b, p, new Font(prototype, emSize, style), sf);
        }

        internal GraphicRect ToBelow(int? width = null, int? height = null)
        {
            var w = width == null ? r.Width : width.Value;
            var h = height == null ? r.Height : height.Value;

			return New(g, new Rectangle(r.Left, r.Top + r.Height, w, h), b, p, f, sf);
        }

        internal GraphicRect ToRight(int? width = null, int? height = null)
        {
            var w = width == null ? r.Width : width.Value;
            var h = height == null ? r.Height : height.Value;

			return New(g, new Rectangle(r.Left + r.Width, r.Top, w, h), b, p, f, sf);
        }

        internal GraphicRect WithStringFormat(StringAlignment alignment, StringAlignment lineAlignment = StringAlignment.Near)
        {
            var sf = new StringFormat { Alignment = alignment, LineAlignment = lineAlignment };
			return New(g, r, b, p, f, sf);
        }

		public GraphicRect Center(Func<GraphicRect, GraphicRect> operation)
		{
			var newG = new CenterGraphicRect(g, r, b, p, f, sf);

			var calculateCenter = (CenterGraphicRect)operation(newG);

            var width = calculateCenter.Width;
            var currentCenterPoint = r.Left + r.Width / 2;

			var newRect = new Rectangle(currentCenterPoint - width/2, r.Top, width, r.Height);

			var centeredGr = new GraphicRect(g, newRect, b, p, f, sf );

            operation(centeredGr).ToBelow();

			return this;
		}
    }
    
	public class CenterGraphicRect : GraphicRect
	{
		readonly int left;
		readonly int right;

		public CenterGraphicRect(Graphics g, Rectangle r, Brush b, Pen p, Font f, StringFormat sf)
			: base( g, r, b, p, f, sf)
		{
			this.left = int.MaxValue;
			this.right = int.MinValue;
		}

		public CenterGraphicRect(Graphics g, Rectangle r, Brush b, Pen p, Font f, StringFormat sf, int left, int right)
			: base( g, r, b, p, f, sf)
		{
			this.left = left;
			this.right = right;
		}

		protected override GraphicRect New(Graphics g, Rectangle r, Brush b, Pen p, Font f, StringFormat sf)
		{
			return new CenterGraphicRect(g, r, b, p, f, sf, left, right);
		}

        internal int Width { get { return right-left; } }
		
		internal override GraphicRect DrawText(string text, int leftOffset = 0)
		{
            var size = TextRenderer.MeasureText(g, text, f, r.Size, TextFormatFlags.NoPadding);

			var newleft = Math.Min(r.Left + leftOffset, left);
			var newRight = Math.Max(right, r.Left + leftOffset + (int)size.Width);

			return new CenterGraphicRect(g, r, b, p, f, sf, newleft, newRight);
		}

        internal override GraphicRect DrawRectangleWithBorder()
        {
            return this;
        }
	}
}