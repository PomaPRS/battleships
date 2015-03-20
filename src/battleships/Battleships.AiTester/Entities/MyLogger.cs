using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleships.Entities.Enums;
using NLog;

namespace Battleships.AiTester.Entities
{
	public class MyLogger
	{
		private Logger logger;

		public MyLogger(string name)
		{
			logger = LogManager.GetLogger(name);
		}

		public void Log(Test test, int testNumber)
		{
			var shipSizes = test.ShipSizes.Aggregate("Ships: ", (a, b) => a + " " + b);
			logger.Info("Test #{0}", testNumber);
			logger.Info("Game Count: {0}", test.GamesCount);
			logger.Info("Width: {0}", test.MapWidth);
			logger.Info("Height: {0}", test.MapWidth);
			logger.Info(shipSizes);

			for (int i = 0; i < test.Maps.Count; i++)
			{
				Log(test.Maps[i], i + 1);
			}
			logger.Info("============================================");
		}

		public void Log(Map map, int mapNumber)
		{
			logger.Info("Map #{0}\n\n{1}", mapNumber, MapToString(map));
		}

		private static string MapToString(Map map)
		{
			var sb = new StringBuilder();
			for (var y = 0; y < map.Height; y++)
			{
				for (var x = 0; x < map.Width; x++)
					sb.Append(GetSymbol(map[new Vector(x, y)]));
				sb.AppendLine();
			}
			return sb.ToString();
		}

		private static string GetSymbol(MapCell cell)
		{
			switch (cell)
			{
				case MapCell.Empty:
					return ".";
				case MapCell.Miss:
					return "*";
				case MapCell.Ship:
					return "O";
				case MapCell.DeadOrWoundedShip:
					return "X";
				default:
					throw new Exception(cell.ToString());
			}
		}
	}
}
