using System.Collections.Generic;
using System.Linq;

namespace Battleships.AiTester.Entities
{
	public class Test
	{
		private Settings settings;

		public Test(int testNumber, int mapWidth, int mapHeight, List<int> shipSizes, Settings settings, params Map[] maps)
		{
			TestNumber = testNumber;
			ShipSizes = shipSizes;
			Maps = maps.ToList();
			MapHeight = mapHeight;
			MapWidth = mapWidth;
			this.settings = settings;
		}

		public int TestNumber { get; private set; }
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
			var aiTester = new AiTester(this, settings);
			return aiPaths.AsParallel().Select(aiTester.Run).AsSequential().ToList();
		}
	}
}
