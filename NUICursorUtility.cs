/*
 * NUICursorTools.cs
 * 
 * Provides various utility functions that are used by the shaper and transform classes.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NUICursorTools
{
	public class NUICursorUtility
	{
		// Calculates (floating-point) distance between two points (floating-point coordinates)
		public static double distance(PointF a, PointF b)
		{
			float deltaX = a.X - b.X;
			float deltaY = a.Y - b.Y;
			// Plain old Pythagorean formula, nothing to see here.
			return Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
		}

		// Calculates (integer) distance between two points (integer coordinates)
		public static int distance(Point a, Point b)
		{
			int deltaX = a.X - b.X;
			int deltaY = a.Y - b.Y;
			// Plain old Pythagorean formula, nothing to see here.
			// (Rounds the distance to nearest integer, then casts to an int for proper return value type.)
			return (int) Math.Floor(Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2)) + 0.5);
		}

		// Calculates magnitude of vector (distance from (0,0) to vector endpoint
		public static double magnitude(PointF p)
		{
			return distance(new PointF(0, 0), p);
		}

		// Calculates slope of line between two points (floating-point coordinates)
		// Vertical lines return a slope of double.PositiveInfinity.
		public static double slope(PointF a, PointF b)
		{
			if (a.X == b.X)
			{
				return double.PositiveInfinity;
			}
			else
			{
				float deltaY = a.Y - b.Y;
				float deltaX = a.X - b.X;

				return deltaY / deltaX;
			}
		}

		// Returns vector between two points (from first to second) (floating-point coordinates)
		public static PointF vector(PointF a, PointF b)
		{
			PointF v = new PointF(0, 0);

			v.X = b.X - a.X;
			v.Y = b.Y - a.Y;

			return v;
		}

		// Calculates angle of vector between two points (from first to second) in radians
		// Returned values always in the interval (-pi, pi]
		public static double angle(PointF a, PointF b)
		{
			float deltaY = b.Y - a.Y;
			float deltaX = b.X - a.X;

			return Math.Atan2(deltaY, deltaX);
		}

		// Calculates angle of given vector in radians
		// Returned values always in the interval (-pi, pi]
		public static double angle(PointF vector)
		{
			return angle(new PointF(0, 0), vector);
		}

		// Normalizes a vector (converts to unit vector in same direction)
		public static PointF normalize(PointF vector)
		{
			float m = (float) magnitude(vector);

			vector.X /= m;
			vector.Y /= m;

			return vector;
		}

		// Calculates the average point from a Queue<Pointf> of points.
		public static PointF averagePoint(Queue<PointF> points)
		{
			PointF avgPoint = new PointF(0, 0);

			foreach (PointF p in points)
			{
				avgPoint.X += p.X;
				avgPoint.Y += p.Y;
			}

			avgPoint.X /= points.Count;
			avgPoint.Y /= points.Count;

			return avgPoint;
		}
	}
}
