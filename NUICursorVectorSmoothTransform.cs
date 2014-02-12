using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NUICursorTools
{
	public class NUICursorVectorSmoothTransform : NUICursorTransform
	{
		// CONSTRUCTORS

		public NUICursorVectorSmoothTransform()
		{
			_type = NUI_CURSOR_TRANSFORM_TYPE.SMOOTH;
			lastSeveralDirections = new Queue<PointF>();
			smoothingActive = false;

			// Default smooth depth.
			_smoothDepth = 3;
		}

		// METHODS

		// Transform functions. The transform function calculates the angle from the last-passed point, 
		// adds the angle point to the list of recent angles, averages the last (up to) smoothDepth angles, 
		// and returns a new point which is the endpoint of a vector that starts at the last-provided point 
		// and is rotated to the averaged angle.

		public override PointF transform(PointF rawPoint)
		{
			// If smoothingActive is false (i.e. smoothing has not yet started, or was reset), 
			// then we assume that the last point is invalid, and simply return the passed point.
			// We store the passed point as the new last point.
			if (smoothingActive == false)
			{
				lastPoint = rawPoint;
				smoothingActive = true;

				return rawPoint;
			}

			// If we've got the max number of directions in the list (this will often be the case), remove the least recent one.
			if (lastSeveralDirections.Count == smoothDepth)
				lastSeveralDirections.Dequeue();

			// Add the latest direction (from the previously passed point to the passed point) to the list.
			PointF vector = NUICursorUtility.vector(lastPoint, rawPoint);
			PointF direction = NUICursorUtility.normalize(vector);
			lastSeveralDirections.Enqueue(direction);

			// Store the passed point.
			lastPoint = rawPoint;

			// If we've got more than one vector stored at this point, calculate their averages...
			if (lastSeveralDirections.Count > 1)
			{
				PointF averageDirection = NUICursorUtility.normalize(NUICursorUtility.averagePoint(lastSeveralDirections));

				// ... scale the average direction by the magnitude of the original vector...
				float magnitude = (float) NUICursorUtility.magnitude(vector);
				PointF newVector = new PointF(averageDirection.X *= magnitude, averageDirection.Y *= magnitude);

				// ... and add the resulting vector to the previous shaped point.
				PointF newPoint = shaper.previousPoint;
				newPoint.X += newVector.X;
				newPoint.Y += newVector.Y;

				// Return the new point.
				return newPoint;
			}

			// Otherwise, return the passed point.
			else
			{
				return rawPoint;
			}
		}

		public override Point transform(Point rawPoint)
		{
			throw new NotImplementedException();
		}

		public void reset()
		{
			lastSeveralDirections.Clear();
			smoothingActive = false;
		}

		// PROPERTIES

		// How long a list of recent directions to retain.
		public int smoothDepth
		{
			get
			{
				return _smoothDepth;
			}
			set
			{
				_smoothDepth = value < 2 ? 2 : value;

				// If we've already stored more directions than this, remove the excess ones.
				while (lastSeveralDirections.Count > _smoothDepth)
					lastSeveralDirections.Dequeue();
			}
		}

		// PRIVATE MEMBERS

		// Is smoothing active? i.e. is the transform in use?
		private bool smoothingActive;

		// The last point passed to the transform.
		private PointF lastPoint;

		// The recent directions list (direction expressed as angle in radians; theta = 0 at positive-direction horizontal axis).
		private Queue<PointF> lastSeveralDirections;

		// How long a list of recent points to retain.
		private int _smoothDepth;
	}
}
