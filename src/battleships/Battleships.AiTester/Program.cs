using System;
using System.Collections.Generic;
using System.Linq;
using Battleships.AiTester.Entities;
using NLog;

namespace Battleships.AiTester
{
	class Program
	{
		static void Main(string[] args)
		{
			var testsLogger = new TestLogger("tester-tests");
			var resultsLogger = LogManager.GetLogger("tester-results");

			try
			{
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
					resultsLogger.Info("");
					resultsLogger.Info("Test #{0}", i + 1);

					var test = testGenerator.GenerateTest();
					testsLogger.Log(test);
					WriteTestInfo(test, resultsLogger);

					var results = test.Run(aiPaths);
					testResults.AddRange(results);

					var statistics = results.Select(x => x.GetStatistics()).ToList();
					WriteTestResult(statistics, resultsLogger);
				}
				WriteTotal(testResults, settings, resultsLogger);
			}
			catch (Exception e)
			{
				resultsLogger.Error(e);
				Console.WriteLine(e);
			}
		}

		private static void WriteTestInfo(Test test, Logger resultsLogger)
		{
			var shipSizes = test.ShipSizes.Aggregate("Ships: ", (a, b) => a + " " + b);
			Console.WriteLine("Game Count: {0}", test.GamesCount);
			Console.WriteLine("Width: {0}", test.MapWidth);
			Console.WriteLine("Height: {0}", test.MapHeight);
			Console.Write(shipSizes);
			Console.WriteLine();
			resultsLogger.Info("Game Count: {0}", test.GamesCount);
			resultsLogger.Info("Width: {0}", test.MapWidth);
			resultsLogger.Info("Height: {0}", test.MapHeight);
			resultsLogger.Info(shipSizes);
			resultsLogger.Info("");
		}

		private static void WriteTotal(List<TestResult> results, Settings settings, Logger resultsLogger)
		{
			var headers = FormatTableRow(new object[] { "AiName", "TotalScore", "MeanScores", "Wins" }, 10);
			Console.WriteLine();
			Console.WriteLine("Total results:");
			Console.WriteLine("==============");
			Console.WriteLine(headers);
			resultsLogger.Info("");
			resultsLogger.Info("Total results:");
			resultsLogger.Info("==============");
			resultsLogger.Info(headers);

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
				resultsLogger.Info(message);
			}
			resultsLogger.Info("************************************************************");
		}

		private static void WriteTestResult(List<Statistics> results, Logger resultsLogger)
		{
			var headers = FormatTableRow(new object[] {"AiName", "Mean", "Sigma", "Median", "Crashes", "Bad%", "Score"}, 7);
			Console.WriteLine();
			Console.WriteLine("Score statistics");
			Console.WriteLine("================");
			Console.WriteLine(headers);
			resultsLogger.Info("");
			resultsLogger.Info("Score statistics");
			resultsLogger.Info("================");
			resultsLogger.Info(headers);

			foreach (var result in results.OrderByDescending(x => x.Score))
			{
				WriteTestResult(result, resultsLogger);
			}
			Console.WriteLine();
			Console.WriteLine("---------------------------------------------------------------");
			resultsLogger.Info("");
			resultsLogger.Info("---------------------------------------------------------------");

			if (results.Any(x => x.BadFraction > 0 || x.CrashesCount != 0))
			{
				Console.WriteLine("Check");
				Console.ReadLine();
			}
		}

		private static void WriteTestResult(Statistics result, Logger resultsLogger)
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
			resultsLogger.Info(message);
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
