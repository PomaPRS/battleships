using System;
using System.Linq;
using Battleships.Entities.Enums;

namespace Battleships.AiTester.Entities
{
	public class ShotInfo
	{
		public ShotResult Hit;
		public Vector Target;
	}

	public class Game
	{
		private readonly Ai ai;

		public Game(Map map, Ai ai)
		{
			Map = map;
			this.ai = ai;
			TurnsCount = 0;
			BadShotsCount = 0;
			ShotsCount = 0;
		}

		public Vector LastTarget { get; private set; }
		public int TurnsCount { get; private set; }
		//<summary>Количество сделанных "глупых" ходов</summary>
		public int BadShotsCount { get; private set; }
		public int ShotsCount { get; private set; }
		public Map Map { get; private set; }
		public ShotInfo LastShotInfo { get; private set; }
		public bool AiCrashed { get; private set; }
		public Exception LastError { get; private set; }

		public bool IsOver()
		{
			return !Map.HasAliveShips() || AiCrashed;
		}

		public GameResult RunToEnd()
		{
			while (!IsOver())
			{
				MakeStep();
			}
			return new GameResult(ShotsCount, BadShotsCount, TurnsCount, AiCrashed);
		}

		public void MakeStep()
		{
			if (IsOver()) throw new InvalidOperationException("Game is Over");
			if (!UpdateLastTarget()) return;
			if (IsBadShot(LastTarget)) BadShotsCount++;
			var hit = Hit(LastTarget);
			LastShotInfo = new ShotInfo { Target = LastTarget, Hit = hit };
			if (hit == ShotResult.Miss)
				TurnsCount++;
			ShotsCount++;
		}

		public ShotResult Hit(Vector target)
		{
			var hit = Map.CheckBounds(target) && Map[target] == MapCell.Ship;
			if (hit)
			{
				var ship = Map.GetShipAt(target);
				ship.Hit(target);
				Map[target] = MapCell.DeadOrWoundedShip;
				return ship.Alive ? ShotResult.Wound : ShotResult.Kill;
			}

			if (Map[target] == MapCell.Empty) 
				Map[target] = MapCell.Miss;
			return ShotResult.Miss;
		}

		private bool UpdateLastTarget()
		{
			try
			{
				LastTarget = LastTarget == null
					? ai.Init(Map.Width, Map.Height, Map.GetShipSizes().ToArray())
					: ai.GetNextShot(LastShotInfo.Target, LastShotInfo.Hit);
				return true;
			}
			catch (Exception e)
			{
				AiCrashed = true;
				LastError = e;
				return false;
			}
		}

		private bool IsBadShot(Vector target)
		{
			var cellWasHitAlready = Map[target] != MapCell.Empty && Map[target] != MapCell.Ship;
			var cellIsNearDestroyedShip = Map.GetNearCells(target).Any(c => Map.GetShipAt(c) != null && !Map.GetShipAt(c).Alive);
			var diagonals = new[] { new Vector(-1, -1), new Vector(-1, 1), new Vector(1, -1), new Vector(1, 1) };
			var cellHaveWoundedDiagonalNeighbour = diagonals.Any(d => Map[target.Add(d)] == MapCell.DeadOrWoundedShip);
			return cellWasHitAlready || cellIsNearDestroyedShip || cellHaveWoundedDiagonalNeighbour;
		}
	}
}
