using System;
using System.Collections.Generic;
using System.IO;

namespace Battleships.AiTester.Entities
{
	public class AiTester
	{
		private readonly Settings settings;
		private readonly ProcessMonitor monitor;
		private readonly Test test;

		public AiTester(Test test, Settings settings)
		{
			this.test = test;
			this.settings = settings;
			var timeLimit = TimeSpan.FromSeconds(settings.TimeLimitSeconds*test.GamesCount);
			monitor = new ProcessMonitor(timeLimit, settings.MemoryLimit);
		}

		public TestResult Run(string exeFilePath)
		{
			var ai = new Ai(exeFilePath, monitor);
			var logger = StrategyLogManager.GetLogger(ai.Name);
			var gameResults = new List<GameResult>();
			var crashesCount = 0;

			logger.Info("");
			logger.Info("Test #{0}", test.TestNumber);
			for (int i = 0; i < test.Maps.Count; i++)
			{
				logger.Info("");
				logger.Info("Map #{0}", i + 1);

				var map = test.Maps[i];
				var game = new Game(new Map(map), ai);
				gameResults.Add(game.RunToEnd());

				if (game.AiCrashed)
				{
					crashesCount++;
					if (crashesCount > settings.CrashLimit)
						break;
					ai = new Ai(exeFilePath, monitor);
				}
				logger.Info("--------------------");
			}
			ai.Dispose();
			logger.Info("========================================");

			var aiName = Path.GetFileNameWithoutExtension(exeFilePath);
			return new TestResult(aiName, test, settings, gameResults.ToArray());
		}

		
	}
}