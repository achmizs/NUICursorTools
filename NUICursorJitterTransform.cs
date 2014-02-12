using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NUICursorTools
{
	public class NUICursorJitterTransform : NUICursorTransform
	{
		// CONSTRUCTORS

		public NUICursorJitterTransform()
		{
			_type = NUI_CURSOR_TRANSFORM_TYPE.JITTER;

			lastPoint_float = new PointF(0, 0);
			lastPoint_int = new Point(0, 0);

			smoothTransform = new NUICursorSmoothTransform();
			vectorSmoothTransform = new NUICursorVectorSmoothTransform();

			// Default jitter threshold.
			jitterThreshold = 20;
			//jitterThreshold = (float) Math.Sqrt(shaper.currentSpace.Width * shaper.currentSpace.Width + shaper.currentSpace.Height * shaper.currentSpace.Height);

			_sampleDepth = 50;
			vectorSmoothTransform.smoothDepth = sampleDepth;
			lastSeveralDirections = new Queue<PointF>();
			curvatureThreshold = (float) (Math.PI * 0.0);
		}

		// METHODS

		public override PointF transform(PointF rawPoint)
		{
			// If the last cursor movement was smaller than the jitter threshold, start smoothing.
			// Otherwise, stop smoothing.
			if (NUICursorUtility.distance(rawPoint, lastPoint_float) < jitterThreshold)
			{
				// If we've got the max number of directions in the list (this will often be the case), remove the least recent one.
				if (lastSeveralDirections.Count == sampleDepth)
					lastSeveralDirections.Dequeue();

				// Add the latest direction (from the previously passed point to the passed point) to the list.
				PointF direction = NUICursorUtility.normalize(NUICursorUtility.vector(lastPoint_float, rawPoint));
				lastSeveralDirections.Enqueue(direction);

				// Decide between path smoothing or point smoothing by checking to see whether the variance in the 
				// last sampleDepth directions ever gets above the path curvature threshhold; in other words, 
				// whether (as far as we can tell) the movement represented by the directions in the queue is intentional
				// slow movement (curvature is below threshold) or is jitter (curvature is above or equal to threshold).
				bool isIntentionalMovement = true;
				float angle = Math.Abs((float) NUICursorUtility.angle(lastSeveralDirections.First()));
				float nextAngle;
				foreach (PointF dir in lastSeveralDirections)
				{
					nextAngle = Math.Abs((float) NUICursorUtility.angle(dir));
					if (Math.Abs(angle - nextAngle) >= curvatureThreshold)
						isIntentionalMovement = false;
					angle = nextAngle;
				}

				// Update the "last provided point" variable.
				lastPoint_float = rawPoint;

				// If the movement is intentional slow movement, apply the path (vector) smooth transform;
				// otherwise, it's just jitter, so apply the point smooth transform.
				if (isIntentionalMovement)
				{
					smoothTransform.reset();
					return vectorSmoothTransform.transform(rawPoint);
				}
				else
				{
					vectorSmoothTransform.reset();
					return smoothTransform.transform(rawPoint);
				}
			}
			else
			{
				// Update the "last provided point" variable.
				lastPoint_float = rawPoint;

				smoothTransform.reset();
				vectorSmoothTransform.reset();
				lastSeveralDirections.Clear();

				return rawPoint;
			}
		}

		public override Point transform(Point rawPoint)
		{
			// If the last cursor movement was smaller than the jitter threshold, start smoothing.
			// Otherwise, stop smoothing.
			if (NUICursorUtility.distance(rawPoint, lastPoint_int) < jitterThreshold)
			{
				// Update the "last provided point" variable.
				lastPoint_int = rawPoint;

				return smoothTransform.transform(rawPoint);
			}
			else
			{
				// Update the "last provided point" variable.
				lastPoint_int = rawPoint;

				smoothTransform.reset();
				return rawPoint;
			}
		}

		// PROPERTIES

		// The jitter threshold; movements smaller than this trigger smoothing start, movements at least this large trigger smoothing stop.
		// NOTE: Jitter threshold value is in the space inhabited by the cursor coordinates passed to the JitterTransform!
		// This means that the actual, real-world threshold of input jitter that triggers the smoothing, will be affected by whether
		// the cursor coordinates have been transformed from "input device space" into "UI space".
		// We recommend that you transform first, then apply jitter reduction.
		public float jitterThreshold { get; set; }

		// The curvature threshold; movement direction changes larger than this trigger path smoothing end, movement direction changes 
		// at most this large, with each movement being below the jitter threshold, trigger path smoothing start.
		public float curvatureThreshold { get; set; }

		public override NUICursorShaper shaper
		{
			get
			{
				return base.shaper;
			}
			set
			{
				base.shaper = value;
				smoothTransform.shaper = value;
				vectorSmoothTransform.shaper = value;
			}
		}

		// How long a list of recent directions to retain.
		public int sampleDepth
		{
			get
			{
				return _sampleDepth;
			}
			set
			{
				_sampleDepth = value < 2 ? 2 : value;

				// If we've already stored more directions than this, remove the excess ones.
				while (lastSeveralDirections.Count > _sampleDepth)
					lastSeveralDirections.Dequeue();
			}
		}
		// PRIVATE MEMBERS

		// The previously passed point.
		private PointF lastPoint_float;
		private Point lastPoint_int;
		
		// The last several directions.
		private Queue<PointF> lastSeveralDirections;
		private int _sampleDepth;

		private NUICursorSmoothTransform smoothTransform;
		private NUICursorVectorSmoothTransform vectorSmoothTransform;
	}
}
