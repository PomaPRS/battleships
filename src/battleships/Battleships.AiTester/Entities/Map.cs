using System;
using System.Collections.Generic;
using System.Linq;
using Battleships.Entities.Enums;

namespace Battleships.AiTester.Entities
{
	public class Map
	{
		private MapCell[,] cells;
		private Ship[,] shipsMap;
		private List<Ship> ships;

		public Map(int width, int height)
		{
			Width = width;
			Height = height;
			cells = new MapCell[width, height];
			shipsMap = new Ship[width, height];
			ships = new List<Ship>();
		}

		public Map(Map map)
			: this(map.Width, map.Height)
		{
			foreach (var ship in map.ships)
			{
				Set(new Ship(ship));
			}
		}

		public int Width { get; private set; }
		public int Height { get; private set; }

		public MapCell this[Vector cell]
		{
			get { return CheckBounds(cell) ? cells[cell.X, cell.Y] : MapCell.Empty; }
			set
			{
				if (!CheckBounds(cell))
					throw new IndexOutOfRangeException(cell + " is not in the map borders");
				cells[cell.X, cell.Y] = value;
			}
		}

		public bool Set(Vector location, int size, Direction direction)
		{
			var ship = new Ship(location, size, direction);
			return Set(ship);
		}

		public bool Set(Ship ship)
		{
			var shipCells = ship.GetShipCells();
			var nearCells = shipCells.SelectMany(GetNearCells);

			var shipWithinMap = shipCells.All(CheckBounds);
			var shipNearCellsAreEmpty = nearCells.All(c => this[c] == MapCell.Empty);
			if (!shipWithinMap || !shipNearCellsAreEmpty)
				return false;

			foreach (var shipCell in shipCells)
			{
				this[shipCell] = MapCell.Ship;
				shipsMap[shipCell.X, shipCell.Y] = ship;
			}
			ships.Add(ship);
			return true;
		}

		public bool CheckBounds(Vector cell)
		{
			return cell.X >= 0 && cell.X < Width && cell.Y >= 0 && cell.Y < Height;
		}

		public IEnumerable<int> GetShipSizes()
		{
			return ships.Select(s => s.Size);
		}

		public Ship GetShipAt(Vector cell)
		{
			return shipsMap[cell.X, cell.Y];
		}

		public bool HasAliveShips()
		{
			return ships.Any(s => s.Alive);
		}

		public IEnumerable<Vector> GetNearCells(Vector cell)
		{
			return
				from x in new[] {-1, 0, 1}
				from y in new[] {-1, 0, 1}
				let c = new Vector(x, y).Add(cell)
				where CheckBounds(c)
				select c;
		}
	}
}
