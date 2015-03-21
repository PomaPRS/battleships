using System;
using System.Collections.Generic;

namespace Battleships.AiTester.Entities
{
	public class TestGenerator
	{
		private Random random;
		private Settings settings;
		private int testGenerated;

		public TestGenerator(Random random, Settings settings)
		{
			this.random = random;
			this.settings = settings;
			testGenerated = 0;
		}

		public Test GenerateTest()
		{
			Test test;
			int mapCount;
			do
			{
				var width = random.Next(settings.MinWidth, settings.MaxWidth);
				var height = random.Next(settings.MinHeight, settings.MaxHeight);
				var maxShipSize = Math.Min(width, height);
				var maxShipCount = Math.Max(1, width * height / 4);
				var shipSizes = GenerateShipSizes(maxShipCount, maxShipSize, random);
				mapCount = random.Next(settings.MinGameCount, settings.MaxGameCount);
				test = GenerateTest(width, height, shipSizes, mapCount);
			} while (mapCount > test.Maps.Count);
			return test;
		}

		private Test GenerateTest(int width, int height, List<int> shipSizes, int mapCount)
		{
			var maps = new List<Map>();
			for (int i = 0; i < mapCount; i++)
			{
				var mapGenerator = new MapGenerator(width, height, shipSizes, random);
				var map = mapGenerator.GenerateMap();

				if (map != null)
					maps.Add(map);
			}
			return new Test(testGenerated++, width, height, shipSizes, settings, maps.ToArray());
		}

		private List<int> GenerateShipSizes(int maxShipCount, int maxShipSize, Random random)
		{
			var shipCount = random.Next(1, maxShipCount);
			var shipSizes = new List<int>();
			for (int i = 0; i < shipCount; i++)
			{
				var size = random.Next(1, maxShipSize);
				shipSizes.Add(size);
			}
			return shipSizes;
		}
	}
}
