using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NUICursorTools
{
	public class NUICursorBoxConstrainTransform : NUICursorTransform
	{
		// CONSTRUCTORS

		public NUICursorBoxConstrainTransform(RectangleF box)
		{
			_type = NUI_CURSOR_TRANSFORM_TYPE.CONSTRAIN;
			
			this.box = box;
		}

		// METHODS

		public override PointF transform(PointF rawPoint)
		{
			if (box.Contains(rawPoint))
			{
				return rawPoint;
			}
			else
			{
				float x = rawPoint.X;
				float y = rawPoint.Y;

				// Constrain x coordinate.
				if (rawPoint.X < box.X)
					x = box.X;
				else if (rawPoint.X > box.X + box.Width)
					x = box.X + box.Width;

				// Constrain y coordinate.
				if (rawPoint.Y < box.Y)
					y = box.Y;
				else if (rawPoint.Y > box.Y + box.Height)
					y = box.Y + box.Height;

				return new PointF(x, y);
			}
		}

		public override Point transform(Point rawPoint)
		{
			throw new NotImplementedException();
		}

		// PROPERTIES

		public RectangleF box { get; set; }
	}
}
