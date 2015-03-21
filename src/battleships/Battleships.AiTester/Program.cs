using System;
using System.Collections.Generic;
using System.Linq;
using Battleships.AiTester.Entities;

namespace Battleships.AiTester
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var testsLogger = new MyLogger("tests");
				var aiPaths = args;
				var settings = Settings.Deserialize();
				var randomSeed = settings.RandomSeed;
				var random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();
				var testResults = new List<TestResult>();
				var testGenerator = new TestGenerator(random, settings);

				for (int i = 0; i < settings.TestCount; i++)
				{
					Console.WriteLine();
					Console.WriteLine("Test #{0}", i + 1);

					var test = testGenerator.GenerateTest();
					testsLogger.Log(test, i + 1);
					WriteTestInfo(test);
					var results = test.Run(aiPaths);
					testResults.AddRange(results);

					var statistics = results.Select(x => x.GetStatistics()).ToList();
					WriteTestResult(statistics);
				}
				WriteTotal(testResults, settings);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		private static void WriteTestInfo(Test test)
		{
			Console.WriteLine("Game Count: {0}", test.GamesCount);
			Console.WriteLine("Width: {0}", test.MapWidth);
			Console.WriteLine("Height: {0}", test.MapWidth);
			Console.Write("Ships:");

			foreach (var shipSize in test.ShipSizes.OrderBy(x => x))
			{
				Console.Write(" {0}", shipSize);
			}
			Console.WriteLine();
		}

		private static void WriteTotal(List<TestResult> results, Settings settings)
		{
			var headers = FormatTableRow(new object[] { "AiName", "TotalScore", "MeanScores", "Wins" }, 10);
			Console.WriteLine();
			Console.WriteLine("Total results:");
			Console.WriteLine("==============");
			Console.WriteLine(headers);
			
			var winCounts = results
				.GroupBy(x => x.Test.TestNumber)
				.SelectMany(x =>
				{
					var scores = x.Select(v => new { v.AiName, Score = v.GetScore() }).ToList();
					var scoreMax = scores.Max(v => v.Score);
					return scores
						.Where(v => Math.Abs(v.Score - scoreMax) < 1e-5)
						.Select(v => new { TestNumber = x.Key, v.AiName });
				})
				.GroupBy(x => x.AiName)
				.ToDictionary(x => x.Key, x => x.Count());

			var aiResults = results.GroupBy(x => x.AiName)
				.Select(x => new
				{
					AiName = x.Key,
					WinCount = winCounts.ContainsKey(x.Key) ? winCounts[x.Key] : 0,
					TotalScore = x.Sum(v => v.GetScore())
				})
				.OrderByDescending(x => x.TotalScore);

			foreach (var aiResult in aiResults)
			{
				var meanScores = aiResult.TotalScore / settings.TestCount;
				var message = FormatTableRow(new object[]
				{
					aiResult.AiName,
					aiResult.TotalScore,
					meanScores,
					aiResult.WinCount
				}, 10);
				Console.WriteLine(message);
			}
		}

		private static void WriteTestResult(List<Statistics> results)
		{
			var headers = FormatTableRow(new object[] {"AiName", "Mean", "Sigma", "Median", "Crashes", "Bad%", "Score"}, 7);
			Console.WriteLine();
			Console.WriteLine("Score statistics");
			Console.WriteLine("================");
			Console.WriteLine(headers);
			foreach (var result in results.OrderByDescending(x => x.Score))
			{
				WriteTestResult(result);
			}
			Console.WriteLine();
			Console.WriteLine("---------------------------------------------------------------");

			if (results.Any(x => x.BadFraction > 0 || x.CrashesCount != 0))
			{
				Console.WriteLine("Check");
				Console.ReadLine();
			}
		}

		private static void WriteTestResult(Statistics result)
		{
			var message = FormatTableRow(new object[]
			{
				result.AiName,
				result.Mean,
				result.Sigma,
				result.Median,
				result.CrashesCount,
				result.BadFraction,
				result.Score
			}, 7);
			Console.WriteLine(message);
		}

		private static string FormatTableRow(object[] values, int cellSize)
		{
			return FormatValue(values[0], 15) + string.Join(" ", values.Skip(1).Select(v => FormatValue(v, cellSize)));
		}

		private static string FormatValue(object v, int width)
		{
			return v.ToString().Replace("\t", " ").PadRight(width).Substring(0, width);
		}
	}
}
