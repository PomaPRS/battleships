using System.Collections.Generic;
using System.Linq;

namespace Battleships.AiTester.Entities
{
	public class Test
	{
		private Settings settings;

		public Test(int mapWidth, int mapHeight, List<int> shipSizes, Settings settings, params Map[] maps)
		{
			ShipSizes = shipSizes;
			Maps = maps.ToList();
			MapHeight = mapHeight;
			MapWidth = mapWidth;
			this.settings = settings;
		}

		public int MapWidth { get; private set; }
		public int MapHeight { get; private set; }
		public List<int> ShipSizes { get; private set; }
		public List<Map> Maps { get; private set; }

		public int GamesCount
		{
			get { return Maps.Count; }
		}

		public List<TestResult> Run(params string[] aiPaths)
		{
			var aiTester = new Battleships.AiTester.Entities.AiTester(this, settings);
			return aiPaths.Select(aiTester.Run).ToList();
		}
	}
}
