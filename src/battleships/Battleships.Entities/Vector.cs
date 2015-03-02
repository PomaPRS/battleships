using System.Collections.Generic;
using System.Linq;

namespace Battleships.Entities
{
	public class Vector
	{
		public Vector(int x, int y)
		{
			X = x;
			Y = y;
		}

		public int X { get; private set; }
		public int Y { get; private set; }

		public Vector Add(Vector other)
		{
			return new Vector(X + other.X, Y + other.Y);
		}

		public Vector Mult(int k)
		{
			return new Vector(k*X, k*Y);
		}

		public Vector Sub(Vector other)
		{
			return new Vector(X - other.X, Y - other.Y);
		}
		public static IEnumerable<Vector> Rect(int minX, int minY, int width, int height)
		{
			return
				from x in Enumerable.Range(minX, width)
				from y in Enumerable.Range(minY, height)
				select new Vector(x, y);
		}

		protected bool Equals(Vector other)
		{
			return Y == other.Y && X == other.X;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Vector)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Y * 397) ^ X;
			}
		}
	}
}
