using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NUICursorTools
{
	public class NUICursorScaleTransform : NUICursorTransform
	{
		// CONSTRUCTORS

		public NUICursorScaleTransform() :
			this(1.0f)
		{ }

		public NUICursorScaleTransform(float multiplier)
		{
			_type = NUI_CURSOR_TRANSFORM_TYPE.SCALE;

			lastRawPoint_float = new PointF(0, 0);
			lastRawPoint_int = new Point(0, 0);

			this.multiplier = multiplier;
		}

		// METHODS

		public override PointF transform(PointF rawPoint)
		{
			// Get the movement vector by subtracting the last raw point from the current raw point.
			PointF vector = new PointF(rawPoint.X - lastRawPoint_float.X, rawPoint.Y - lastRawPoint_float.Y);

			// Scale the vector by the multiplier.
			vector.X *= multiplier;
			vector.Y *= multiplier;

			// Get the new point by adding the scaled vector to the previous shaped point.
			float x = shaper.previousPoint.X + vector.X;
			float y = shaper.previousPoint.Y + vector.Y;

			// Construct the new point.
			PointF newPoint = new PointF(x, y);

			// Update the "last raw point" variable.
			lastRawPoint_float = rawPoint;

			return newPoint;
		}

		public override Point transform(Point rawPoint)
		{
			throw new NotImplementedException();
		}

		// PROPERTIES

		// The velocity scale multiplier.
		public float multiplier { get; set; }

		// PRIVATE MEMBERS

		// The previously passed point.
		private PointF lastRawPoint_float;
		private Point lastRawPoint_int;
	}
}
