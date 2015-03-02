using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Battleships.Entities
{
	public enum Direction
	{
		Horizontal,
		Vertical
	}

	public class Ship
	{
		private HashSet<Vector> aliveCells;

		public Ship(Vector location, int size, Direction direction)
		{
			Location = location;
			Size = size;
			Direction = direction;
			aliveCells = new HashSet<Vector>(GetShipCells());
		}

		public Ship(Ship ship)
			: this(ship.Location, ship.Size, ship.Direction)
		{
		}

		public Vector Location { get; private set; }
		public int Size { get; private set; }
		public Direction Direction { get; private set; }

		public bool Alive { get { return aliveCells.Any(); } }

		public List<Vector> GetShipCells()
		{
			var direciton = Direction == Direction.Horizontal ? new Vector(1, 0) : new Vector(0, 1);
			var shipCells = new List<Vector>();
			for (int i = 0; i < Size; i++)
			{
				var shipCell = direciton.Mult(i).Add(Location);
				shipCells.Add(shipCell);
			}
			return shipCells;
		}

		public bool Hit(Vector target)
		{
			return aliveCells.Remove(target);
		}
	}
}
