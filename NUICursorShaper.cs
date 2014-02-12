/*
 * NUICursorShaper.cs
 * 
 * NUICursorShaper is the central class of NUICursorTools. All cursor transformations should be applied using a shaper object.
 * 
 * How to use:
 * 
 * 1. Create a shaper object
 * 2. Add transforms to it
 * 3. (Optional) Set order mode
 * 4. Feed raw input device coordinates to it via the shape() method, which will return shaped cursor coordinates.
 * 
 * NUICursorShaper also provides logging functions.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NUICursorTools
{
	public enum NUI_CURSOR_SHAPER_ORDER_MODE
	{
		NORMAL,
		SMART,
		CUSTOM
	}
	
	public class NUICursorShaper
	{
		// CONSTRUCTORS
		
		public NUICursorShaper()
		{
			transforms = new List<NUICursorTransform>();
			mode = NUI_CURSOR_SHAPER_ORDER_MODE.NORMAL;

			currentPoint = new PointF(0, 0);
			previousPoint = new PointF(0, 0);
		}

		// METHODS

		public PointF shape(PointF rawPoint)
		{
			previousPoint = currentPoint;
			currentPoint = rawPoint;
			
			if (mode == NUI_CURSOR_SHAPER_ORDER_MODE.NORMAL)
			{
				foreach (NUICursorTransform t in transforms)
				{
					currentPoint = t.transform(currentPoint);
				}
			}

			return currentPoint;
		}

		public Point shape(Point rawPoint)
		{
			previousPoint = currentPoint;
			currentPoint = rawPoint;

			if (mode == NUI_CURSOR_SHAPER_ORDER_MODE.NORMAL)
			{
				foreach (NUICursorTransform t in transforms)
				{
					currentPoint = t.transform(currentPoint);
				}
			}

			return new Point((int)currentPoint.X, (int)currentPoint.Y);
		}

		public void addTransform(NUICursorTransform t)
		{
			t.shaper = this;
			transforms.Add(t);
		}

		// PROPERTIES

		// Previous point. The previously received raw point plus all the transforms that were applied to it (i.e. all the active transforms).
		// a.k.a. "Where is the cursor right now? (prior to the latest information from the device)"
		public PointF previousPoint { get; private set; }

		// Current point. This is the just-received raw point, with all transforms that have so far been applied to it.
		// a.k.a. "Where has the cursor moved to? (as of this latest information from the device)"
		public PointF currentPoint { get; private set; }

		// Current space. This is the coordinate space for the point passed to the currently-executing transform.
		//
		// NOTE 1: While this property can be set publicly, if the shaper contains a SpaceTransform, the SpaceTransform
		// will automatically set the currentSpace each time it runs. (Which means that the value of currentSpace after
		// each raw point has been fully shaped will be the space of the fully-shaped point, i.e. the target space of the
		// last SpaceTransform in the order.)
		//
		// NOTE 2: If the shaper does NOT contain a SpaceTransform, be sure to manually set the currentSpace property to 
		// the correct value (that is, the space of the raw points)! Otherwise, certain transforms (such as any AccelerationTransforms)
		// will not work properly!
		public RectangleF currentSpace { get; set; }

		// Order of operations mode.
		// NORMAL: Apply transforms in the order they are added. Also known as manual mode. (Default.)
		// SMART: Apply transforms in the order they should be applied for best results in most cases. (That is, use standard priority settings).
		//        Does not consider custom transforms, obviously; these are always applied after all standard transforms, in the order they were added (i.e. priority = 0).
		// CUSTOM: Use the priority setting of each transform to order the transforms. Ties are resolved by order transforms were added. Default priority is 1.
		public NUI_CURSOR_SHAPER_ORDER_MODE mode { get; set; }
		
		// PRIVATE MEMBERS
		
		// The transforms that make up the shaper's functionality.
		private List <NUICursorTransform> transforms;
	}
}
