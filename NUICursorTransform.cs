/*
 * NUICursorTransform.cs
 * 
 * NUICursorTransform is the base class for cursor transforms. Abstract, can't be instantiated.
 * Has a type (mostly for internal use by NUICursorShaper) and a priority, which is used for CUSTOM order mode.
 * Provides two versions of the transform() method, one for points with floating-point coordinates (PointF)
 * and one for points with integer coordinates (Point).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NUICursorTools
{
	public enum NUI_CURSOR_TRANSFORM_TYPE
	{
		SMOOTH, 
		JITTER, 
		SCALE, 
		ACCELERATION, 
		SPACE,
		CONSTRAIN,
		OTHER
	};

	public abstract class NUICursorTransform
	{
		// METHODS

		public abstract PointF transform(PointF rawPoint);

		public abstract Point transform(Point rawPoint);

		// PROPERTIES

		// The shaper to which the transform has been added.
		// Set by a shaper when a transform is added to it.
		public virtual NUICursorShaper shaper { get; set; }

		public NUI_CURSOR_TRANSFORM_TYPE type
		{
			get
			{
				return _type;
			}
		}

		public int priority
		{
			get
			{
				return _priority;
			}
			set
			{
				_priority = value;
				// MORE CODE GOES HERE
			}
		}

		// PRIVATE MEMBERS

		protected NUI_CURSOR_TRANSFORM_TYPE _type;

		// Used by NUICursorShaper for CUSTOM  order mode.
		protected int _priority;
	}
}
