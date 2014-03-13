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
            scaleFactor = 1.0f;
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
                    scaleTransform.multiplier = pair.Value * scaleFactor;
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
            // Template
            
            //accelCurve[0.0f] =
            //accelCurve[10] =
            //accelCurve[20] =
            //accelCurve[30] =
            //accelCurve[40] =
            //accelCurve[50] =
            //accelCurve[60] =
            //accelCurve[70] =
            //accelCurve[80] =
            //accelCurve[90] =
            //accelCurve[100] =

            // Windows

            //accelCurve[0.0f] = 0.55f;
            //accelCurve[10] = 0.95f;
            //accelCurve[20] = 1.6f;
            //accelCurve[30] = 1.8f;
            //accelCurve[40] = 1.9f;
            //accelCurve[50] = 2.0f;
            //accelCurve[60] = 2.05f;
            //accelCurve[70] = 2.1f;
            //accelCurve[80] = 2.12f;
            //accelCurve[90] = 2.15f;
            //accelCurve[100] = 2.15f;

            //accelCurve[0.0f] = 0.55f;
            //accelCurve[20] = 0.95f;
            //accelCurve[40] = 1.6f;
            //accelCurve[60] = 1.8f;
            //accelCurve[80] = 1.9f;
            //accelCurve[100] = 2.0f;
            //accelCurve[120] = 2.05f;
            //accelCurve[140] = 2.1f;
            //accelCurve[160] = 2.12f;
            //accelCurve[180] = 2.15f;
            //accelCurve[200] = 2.15f;

            //accelCurve[0.0f] = 0.55f;
            //accelCurve[40] = 0.95f;
            //accelCurve[80] = 1.6f;
            //accelCurve[120] = 1.8f;
            //accelCurve[160] = 1.9f;
            //accelCurve[200] = 2.0f;
            //accelCurve[240] = 2.05f;
            //accelCurve[280] = 2.1f;
            //accelCurve[320] = 2.12f;
            //accelCurve[360] = 2.15f;
            //accelCurve[400] = 2.15f;

            // Mac OS X

            //accelCurve[0.0f] = 0.1f;
            //accelCurve[10] = 0.4f;
            //accelCurve[20] = 0.5f;
            //accelCurve[30] = 1.0f;
            //accelCurve[40] = 1.9f;
            //accelCurve[50] = 2.25f;
            //accelCurve[60] = 2.2f;
            //accelCurve[70] = 2.0f;
            //accelCurve[80] = 1.85f;
            //accelCurve[90] = 1.7f;
            //accelCurve[100] = 1.5f;

            //accelCurve[0.0f] = 0.1f;
            //accelCurve[20] = 0.4f;
            //accelCurve[40] = 0.5f;
            //accelCurve[60] = 1.0f;
            //accelCurve[80] = 1.9f;
            //accelCurve[100] = 2.25f;
            //accelCurve[120] = 2.2f;
            //accelCurve[140] = 2.0f;
            //accelCurve[160] = 1.85f;
            //accelCurve[180] = 1.7f;
            //accelCurve[200] = 1.5f;

            accelCurve[0.0f] = 0.1f;
            accelCurve[40] = 0.4f;
            accelCurve[80] = 0.5f;
            accelCurve[120] = 1.0f;
            accelCurve[160] = 1.9f;
            accelCurve[200] = 2.25f;
            accelCurve[240] = 2.2f;
            accelCurve[280] = 2.0f;
            accelCurve[320] = 1.85f;
            accelCurve[360] = 1.7f;
            accelCurve[400] = 1.5f;

            // Custom

            //accelCurve[0.0f] = 0.05f;
            //accelCurve[10] = 0.15f;
            //accelCurve[20] = 0.25f;
            //accelCurve[30] = 0.5f;
            //accelCurve[40] = 1.9f;
            //accelCurve[50] = 2.25f;
            //accelCurve[60] = 2.3f;
            //accelCurve[70] = 2.35f;
            //accelCurve[80] = 2.38f;
            //accelCurve[90] = 2.4f;
            //accelCurve[100] = 2.41f;

            //accelCurve[0.0f] = 0.05f;
            //accelCurve[20] = 0.15f;
            //accelCurve[40] = 0.25f;
            //accelCurve[60] = 0.5f;
            //accelCurve[80] = 1.9f;
            //accelCurve[100] = 2.25f;
            //accelCurve[120] = 2.3f;
            //accelCurve[140] = 2.35f;
            //accelCurve[160] = 2.38f;
            //accelCurve[190] = 2.4f;
            //accelCurve[200] = 2.41f;

            //accelCurve[0.0f] = 0.05f;
            //accelCurve[40] = 0.1f;
            //accelCurve[80] = 0.2f;
            //accelCurve[120] = 0.4f;
            //accelCurve[160] = 0.8f;
            //accelCurve[200] = 1.6f;
            //accelCurve[240] = 2.0f;
            //accelCurve[280] = 2.2f;
            //accelCurve[320] = 2.3f;
            //accelCurve[360] = 2.35f;
            //accelCurve[400] = 2.38f;

            accelCurve[0.0f] = 0.05f;
            accelCurve[40] = 0.1f;
            accelCurve[80] = 0.1f;
            accelCurve[160] = 0.2f;
            accelCurve[240] = 0.4f;
            accelCurve[320] = 0.8f;
            accelCurve[400] = 1.6f;
            accelCurve[480] = 2.0f;
            accelCurve[560] = 2.2f;
            accelCurve[640] = 2.3f;
            accelCurve[720] = 2.35f;
            accelCurve[800] = 2.38f;

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

        // Y-axis accel curve scale factor.
        private float scaleFactor;

		// Scale transform(s).
		private NUICursorScaleTransform scaleTransform;
	}
}
