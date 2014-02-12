using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NUICursorTools
{
	public class NUICursorAccelerationTransform : NUICursorTransform
	{
		// CONSTRUCTORS

		//public NUICursorAccelerationTransform() :
		//    this(50, 2.0f)
		//{ }

		public NUICursorAccelerationTransform()
		{
			_type = NUI_CURSOR_TRANSFORM_TYPE.ACCELERATION;

			lastPoint = new PointF(0, 0);

			scaleTransform = new NUICursorScaleTransform();

			accelCurve = new SortedDictionary<float, float>();
			constructAccelCurve();
		}

		// METHODS

		public override PointF transform(PointF rawPoint)
		{
			// Determine raw distance moved since last raw point.
			double distance = NUICursorUtility.distance(rawPoint, lastPoint);

			scaleTransform.multiplier = accelCurve[0.0f];
			foreach (KeyValuePair<float, float> pair in accelCurve)
			{
				if (distance > pair.Key)
					scaleTransform.multiplier = pair.Value;
			}

			//if (distance >= threshold)
			//{
			//    scaleTransform.multiplier = multiplier;
			//}
			//else
			//{
			//    scaleTransform.multiplier = 1.0f;
			//}

			// Apply the scale transform to get the new point.
			PointF newPoint = scaleTransform.transform(rawPoint);

			// Update the "last raw point" variable.
			lastPoint = rawPoint;

			return newPoint;
		}

		public override Point transform(Point rawPoint)
		{
			throw new NotImplementedException();
		}

		private void constructAccelCurve()
		{
			accelCurve[0.0f] = 0.5f;
			//accelCurve[10] = 1.0f;
			accelCurve[20] = 1.5f;
			accelCurve[50] = 2.0f;
			accelCurve[80] = 3.0f;
		}

		// PROPERTIES

		// The minimum speed at which acceleration is applied (measured in pixels per frame).
		//public float threshold { get; set; }

		// The acceleration multiplier, applied if the speed is at least equal to the threshold.
		// Can't be less than 1.0.
		//public float multiplier
		//{
		//    get
		//    {
		//        return _multiplier;
		//    }
		//    set
		//    {
		//        _multiplier = (float)Math.Max(value, 1.0);
		//    }
		//}

		public override NUICursorShaper shaper
		{
			get
			{
				return base.shaper;
			}
			set
			{
				base.shaper = value;
				scaleTransform.shaper = value;
			}
		}

		// PRIVATE MEMBERS

		// The previously passed point.
		private PointF lastPoint;

		// Lookup table that defines the acceleration curve.
		private SortedDictionary <float, float> accelCurve;

		// Scale transform(s).
		private NUICursorScaleTransform scaleTransform;
	}
}
