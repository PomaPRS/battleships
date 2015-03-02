using System;
using System.Collections.Generic;
using System.IO;

namespace Battleships.Entities
{
	public class AiTester
	{
		private readonly Settings settings;
		private readonly ProcessMonitor monitor;
		private readonly List<Map> maps;

		public AiTester(List<Map> maps, Settings settings)
		{
			this.maps = maps;
			this.settings = settings;
			monitor = new ProcessMonitor(TimeSpan.FromSeconds(settings.TimeLimitSeconds*maps.Count), settings.MemoryLimit);
		}

		public TestResult Run(string exeFilePath)
		{
			var ai = new Ai(exeFilePath, monitor);
			var crashesCount = 0;
			var playedGamesCount = 0;
			var shots = new List<int>();
			var badShots = new List<int>();
			var turns = new List<int>();
			
			foreach (var map in maps)
			{
				var game = new Game(new Map(map), ai);
				game.RunToEnd();
				playedGamesCount++;
				badShots.Add(game.BadShotsCount);
				shots.Add(game.ShotsCount);

				if (game.AiCrashed)
				{
					crashesCount++;
					if (crashesCount > settings.CrashLimit)
						break;
					ai = new Ai(exeFilePath, monitor);
				}
				else
					turns.Add(game.TurnsCount);
			}
			ai.Dispose();

			return new TestResult
			{
				AiName = Path.GetFileNameWithoutExtension(exeFilePath),
				MapWidth = maps[0].Width,
				MapHeight = maps[0].Height,
				BadShots = badShots,
				CrashesCount = crashesCount,
				PlayedGamesCount = playedGamesCount,
				Shots = shots,
				Turns = turns
			};
		}
	}
}
