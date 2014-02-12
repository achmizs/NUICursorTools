using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NUICursorTools
{
	public class NUICursorSmoothTransform : NUICursorTransform
	{
		// CONSTRUCTORS

		public NUICursorSmoothTransform()
		{
			_type = NUI_CURSOR_TRANSFORM_TYPE.SMOOTH;
			lastSeveralPoints_float = new Queue<PointF>();
			lastSeveralPoints_int = new Queue<Point>();

			// Default smooth depth.
			_smoothDepth = 50;
		}

		// METHODS
	
		// Transform functions. The transform function adds the provided point to the list of recent points,
		// averages the last (up to) smoothDepth points, and returns a new point with the averaged coordinates.

		public override PointF transform(PointF rawPoint)
		{
			float x = 0;
			float y = 0;

			// If we've got the max number of points in the list (this will often be the case), remove the least recent one.
			if (lastSeveralPoints_float.Count == smoothDepth)
				lastSeveralPoints_float.Dequeue();

			// Add the provided point to the recent points list.
			lastSeveralPoints_float.Enqueue(rawPoint);

			// Smoothed point is average of new point and the last (n-1) points, where n is smoothDepth.
			foreach (PointF p in lastSeveralPoints_float)
			{
				x += p.X;
				y += p.Y;
			}

			// Get the average values for x and y.
			x /= lastSeveralPoints_float.Count;
			y /= lastSeveralPoints_float.Count;

			// Return smoothed point.
			return new PointF(x, y);
		}

		public override Point transform(Point rawPoint)
		{
			float x = 0;
			float y = 0;

			// If we've got the max number of points in the list (this will often be the case), remove the least recent one.
			if (lastSeveralPoints_float.Count == smoothDepth)
				lastSeveralPoints_float.Dequeue();

			// Add the provided point to the recent points list.
			lastSeveralPoints_float.Enqueue(rawPoint);

			// Smoothed point is average of new point and the last (n-1) points, where n is smoothDepth.
			foreach (PointF p in lastSeveralPoints_float)
			{
				x += p.X;
				y += p.Y;
			}

			// Get the average values for x and y.
			x /= lastSeveralPoints_float.Count;
			y /= lastSeveralPoints_float.Count;

			// Round x and y to nearest integers.
			x = (float) Math.Floor(x + 0.5);
			y = (float) Math.Floor(y + 0.5);

			// Return smoothed point.
			return new Point((int) x, (int) y);
		}

		// Clears the recent points list.
		public void reset()
		{
			lastSeveralPoints_float.Clear();
			lastSeveralPoints_int.Clear();
		}

		// PROPERTIES

		// How long a list of recent points to retain.
		public int smoothDepth
		{
			get
			{
				return _smoothDepth;
			}
			set
			{
				_smoothDepth = value < 2 ? 2 : value;

				// If we've already stored more points than this, remove the excess ones.
				while (lastSeveralPoints_int.Count > _smoothDepth)
					lastSeveralPoints_int.Dequeue();
				while (lastSeveralPoints_float.Count > _smoothDepth)
					lastSeveralPoints_float.Dequeue();
			}
		}

		// PRIVATE MEMBERS

		// The recent points list.
		private Queue<PointF> lastSeveralPoints_float;
		private Queue<Point> lastSeveralPoints_int;

		// How long a list of recent points to retain.
		private int _smoothDepth;
	}
}
