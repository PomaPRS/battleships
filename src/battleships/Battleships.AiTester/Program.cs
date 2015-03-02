using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleships.Entities;

namespace Battleships.AiTester
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var settings = new Settings()
				{
					CrashLimit = 1,
					MemoryLimit = 200000000,
					TimeLimitSeconds = 10,
					MinGameCount = 100,
					MaxGameCount = 100,
					MinWidth = 1,
					MaxWidth = 20,
					MinHeight = 1,
					MaxHeight = 20
				};

				var randomSeed = settings.RandomSeed;
				var random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();
				var maps = GenerateMaps(random, settings);

				Console.WriteLine("{0} {1}", maps[0].Width, maps[0].Height);
				foreach (var shipSize in maps[0].GetShipSizes())
				{
					Console.Write("{0} ", shipSize);
				}
				Console.WriteLine();

				var aiTester = new Entities.AiTester(maps, settings);

				var results = args.Select(aiTester.Run).ToList();
				WriteTotal(results, settings);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		static List<Map> GenerateMaps(Random random, Settings settings)
		{
			int mapCount;
			List<Map> maps;
			do
			{
				var width = random.Next(settings.MinWidth, settings.MaxWidth);
				var height = random.Next(settings.MinHeight, settings.MaxHeight);
				var maxShipSize = Math.Min(width, height);
				var maxShipCount = Math.Max(1, width * height / 4);
				var shipSizes = GenerateShipSizes(maxShipCount, maxShipSize, random);
				mapCount = random.Next(settings.MinGameCount, settings.MaxGameCount);
				maps = GenerateMaps(width, height, shipSizes, mapCount, random);
			} while (mapCount > maps.Count);
			return maps;
		}

		static List<Map> GenerateMaps(int width, int height, List<int> shipSizes, int mapCount, Random random)
		{
			var maps = new List<Map>();
			for (int i = 0; i < mapCount; i++)
			{
				var mapGenerator = new MapGenerator(width, height, shipSizes, random);
				var map = mapGenerator.GenerateMap();

				if (map != null)
					maps.Add(map);
			}
			return maps;
		}

		static List<int> GenerateShipSizes(int maxShipCount, int maxShipSize, Random random)
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

		private static void WriteTotal(List<TestResult> results, Settings settings)
		{
			var headers = FormatTableRow(new object[] { "AiName", "Mean", "Sigma", "Median", "Crashes", "Bad%", "Games", "Score" });
			Console.WriteLine("Score statistics");
			Console.WriteLine("================");
			Console.WriteLine(headers);
			foreach (var result in results)
			{
				WriteTotal(result, settings);
			}
			Console.WriteLine();
		}

		private static void WriteTotal(TestResult result, Settings settings)
		{
			var aiName = result.AiName;
			var width = result.MapWidth;
			var height = result.MapHeight;
			var shots = result.Shots;
			var badShots = result.BadShots.Sum();
			var crashes = result.CrashesCount;
			var gamesPlayed = result.PlayedGamesCount;

			if (shots.Count == 0) shots.Add(1000 * 1000);
			shots.Sort();
			var median = shots.Count % 2 == 1 ? shots[shots.Count / 2] : (shots[shots.Count / 2] + shots[(shots.Count + 1) / 2]) / 2;
			var mean = shots.Average();
			var sigma = Math.Sqrt(shots.Average(s => (s - mean) * (s - mean)));
			var badFraction = (100.0 * badShots) / shots.Sum();
			var crashPenalty = 100.0 * crashes / settings.CrashLimit;
			var efficiencyScore = 100.0 * (width * height - mean) / (width * height);
			var score = efficiencyScore - crashPenalty - badFraction;
			var message = FormatTableRow(new object[] { aiName, mean, sigma, median, crashes, badFraction, gamesPlayed, score });
			Console.WriteLine(message);
		}

		private static string FormatTableRow(object[] values)
		{
			return FormatValue(values[0], 15)
				+ string.Join(" ", values.Skip(1).Select(v => FormatValue(v, 7)));
		}

		private static string FormatValue(object v, int width)
		{
			return v.ToString().Replace("\t", " ").PadRight(width).Substring(0, width);
		}
	}
}
