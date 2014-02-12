/*
 * NUICursorSpaceTransform.cs
 * 
 * NUICursorSpaceTransform transforms coordinates from one "space" to another - i.e., from [X = -320 to 320, Y = 240 to -240] (i.e. Kinect space)
 * to [X = 0 to 1920, Y = 0 to 1080] (i.e. UI space of a 1920x1080 display). This is an affine transformation (preserves relative position/size).
 * 
 * NOTE: Transforming cursor coordinates into a larger space magnifies jitter.
 * It is therefore advisable to apply a jitter transform after a space transform.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NUICursorTools
{
	public class NUICursorSpaceTransform : NUICursorTransform
	{
		public enum NUI_CURSOR_SPACE_TRANSFORM_MODE
		{
			STRETCH,
			FILL,
			FIT
		};

		// CONSTRUCTOR

		public NUICursorSpaceTransform() :
			this(new RectangleF(0, 0, 1, 1), new RectangleF(0, 0, 1, 1), false, false, NUI_CURSOR_SPACE_TRANSFORM_MODE.STRETCH)
		{ }

		public NUICursorSpaceTransform(RectangleF source, RectangleF target, bool mirrorX = false, bool mirrorY = false, NUI_CURSOR_SPACE_TRANSFORM_MODE mode = NUI_CURSOR_SPACE_TRANSFORM_MODE.STRETCH)
		{
			_type = NUI_CURSOR_TRANSFORM_TYPE.SPACE;

			this.source = source;
			this.target = target;
			this.mirrorX = mirrorX;
			this.mirrorY = mirrorY;
			this.mode = mode;
		}

		// METHODS

		public override PointF transform(PointF rawPoint)
		{
			// Variables to hold the horizontal and vertical scaling factors.
			float hScaleFactor;
			float vScaleFactor;

			// Variables to hold the new origin for transformed coordinates
			// (which may NOT be the origin of the target space, depending on mode).
			float newOriginX;
			float newOriginY;
			
			// Flip horizontally and/or vertically, if necessary.
			float x = rawPoint.X * (mirrorX ? -1 : 1);
			float y = rawPoint.Y * (mirrorY ? -1 : 1);

			// Translate from origin of source space to (0, 0) for scaling.
			x -= source.X;
			y -= source.Y;

			// Determine the scale difference between the two spaces, in each dimension.
			float hScale = target.Width / source.Width;
			float vScale = target.Height / source.Height;

			// Set coordinate adjustment variables appropriately for selected mode.
			switch (mode)
			{
				// Non-proportionally scale the source space such that it matches the size and shape of the target space.
				case NUI_CURSOR_SPACE_TRANSFORM_MODE.STRETCH:
					hScaleFactor = hScale;
					vScaleFactor = vScale;

					newOriginX = target.X;
					newOriginY = target.Y;

					break;

				// Proportionally scale the source space such that it circumscribes the target space. 
				// and its center is the center of the target space.
				case NUI_CURSOR_SPACE_TRANSFORM_MODE.FILL:
					hScaleFactor = Math.Max(hScale, vScale);
					vScaleFactor = Math.Max(hScale, vScale);

					if (hScale > vScale)
					{
						newOriginX = target.X;
						newOriginY = target.Y - (Math.Abs(target.Height - (source.Height * vScaleFactor)) / 2);
					}
					else // if (hScale <= vScale)
					{
						newOriginX = target.X - (Math.Abs(target.Width - (source.Width * hScaleFactor)) / 2);
						newOriginY = target.Y;
					}

					break;

				case NUI_CURSOR_SPACE_TRANSFORM_MODE.FIT:
					hScaleFactor = Math.Min(hScale, vScale);
					vScaleFactor = Math.Min(hScale, vScale);

					if (hScale > vScale)
					{
						newOriginX = target.X + (Math.Abs(target.Width - (source.Width * hScaleFactor)) / 2);
						newOriginY = target.Y;
					}
					else // if (hScale <= vScale)
					{
						newOriginX = target.X;
						newOriginY = target.Y + (Math.Abs(target.Height - (source.Height * vScaleFactor)) / 2);
					}

					break;

				default:
					hScaleFactor = 1.0f;
					vScaleFactor = 1.0f;

					newOriginX = source.X;
					newOriginY = source.Y;

					break;
			}

			// Scale.
			x *= hScaleFactor;
			y *= vScaleFactor;

			// Translate to new origin.
			x += newOriginX;
			y += newOriginY;

			// Set the parent shaper's currentSpace property to the target space.
			shaper.currentSpace = target;

			// Return the transformed point.
			return new PointF(x, y);
		}

		public override Point transform(Point rawPoint)
		{
			PointF newPoint_float = transform(new PointF((float) rawPoint.X, (float) rawPoint.Y));
			return new Point((int)newPoint_float.X, (int)newPoint_float.Y);
		}

		// PROPERTIES

		// Source and target spaces.
		public RectangleF source { get; set; }
		public RectangleF target { get; set; }

		// Whether coordinates should be flipped horizontally and/or vertically.
		public bool mirrorX { get; set; }
		public bool mirrorY { get; set; }

		// Aspect ratio adjustment mode. Three modes are available:
		//
		// STRETCH    Stretches source space such that corners of source space are placed in corners of target space.
		// (default)  In the case of non-matching aspect ratios between source and target space, stretch mode results in distortion; 
		//            the dimension which is shortened, relatively speaking, will require more source space movement to produce 
		//            a unit of target space movement than the other dimension.
		//
		// FILL    Fills the target space by proportionally scaling the source space until the target space is fully covered in both dimensions.
		//         In the case of non-matching aspect ratios, does not result in distortion, but effectively "cuts off" the edges of the source space 
		//         in whichever dimension of the source space which is larger, proportionally speaking, than that dimension of the target space
		//         (like fitting a widescreen image onto a non-widescreen display by cutting off the sides of the image).
		//         Input points in the cut-off parts of the source space will have coordinates that are outside the bounds of the target space,
		//         so ensure that the client code is able to handle that (for example, by adding a BoxConstrainTransform after the SpaceTransform).
		//
		// FIT    Scales the source space until it is inscribed within the target space, i.e. such that either the height or width of the source space
		//        matches the corresponding dimension of the target space, but no part of the scaled-up space lies outside the target space.
		//        In the case of non-matching aspect ratios, does not result in distortion, but renders the edges (either vertical or horizontal edges)
		//        of the target space inaccessible; that is, there will be "bands" either along the top and bottom, or left and right, of the target space
		//        such that no point within the bounds of the source space can be mapped to a point within those bands in the target space).
		//        (This is the equivalent of letterboxing, i.e. fitting a widescreen image onto a non-widescreen display by leaving black bars on the top and bottom.)
		public NUI_CURSOR_SPACE_TRANSFORM_MODE mode { get; set; }
	}
}
