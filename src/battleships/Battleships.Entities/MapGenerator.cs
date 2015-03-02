using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Entities
{
	public class MapGenerator
	{
		private readonly int height;
		private readonly Random random;
		private readonly List<int> shipSizes;
		private readonly int width;

		public MapGenerator(int width, int height, List<int> shipSizes, Random random)
		{
			this.width = width;
			this.height = height;
			this.shipSizes = shipSizes.OrderByDescending(s => s).ToList();
			this.random = random;
		}

		public Map GenerateMap()
		{
			var map = new Map(width, height);
			foreach (var size in shipSizes)
			{
				if (!PlaceShip(map, size))
					return null;
			}
			return map;
		}

		private bool PlaceShip(Map map, int size)
		{
			var cells = Vector.Rect(0, 0, width, height).OrderBy(v => random.Next());
			foreach (var loc in cells)
			{
				var direction = random.Next(2) == 0 ? Direction.Horizontal : Direction.Vertical;
				if (map.Set(loc, size, direction))
					return true;
				direction = SwitchDirection(direction);
				if (map.Set(loc, size, direction))
					return true;
			}
			return false;
		}

		private Direction SwitchDirection(Direction direction)
		{
			return direction == Direction.Horizontal ? Direction.Vertical : Direction.Horizontal;
		}
	}
}
