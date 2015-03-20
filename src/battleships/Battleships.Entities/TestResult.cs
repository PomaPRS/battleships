using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleships.Entities
{
	public class TestResult
	{
		private Settings settings;
		
		public TestResult(int mapWidth, int mapHeight, string aiName, int gamesPlayed, Settings settings, params GameResult[] gameResults)
		{
			MapWidth = mapWidth;
			MapHeight = mapHeight;
			AiName = aiName;
			GamesPlayed = gamesPlayed;
			GameResults = gameResults.ToList();
			this.settings = settings;
		}

		public int MapWidth { get; private set; }
		public int MapHeight { get; private set; }
		public string AiName { get; private set; }
		public int GamesPlayed { get; private set; }
		public List<GameResult> GameResults { get; private set; }

		public Statistics GetStatistics()
		{
			return new Statistics
			{
				AiName = AiName,
				Mean = GetShotsMean(),
				Sigma = GetShotsSigma(),
				Median = GetShotsMedian(),
				CrashesCount = GetCrashesCount(),
				BadFraction = GetBadFraction(),
				GamesPlayed = GamesPlayed,
				Score = GetScore()
			};
		}

		public double GetScore()
		{
			return GetEfficiencyScore() - GetCrashPenalty() - GetBadFraction();
		}

		public double GetEfficiencyScore()
		{
			return 100.0*(MapWidth*MapHeight- GetShotsMean())/(MapWidth*MapHeight);
		}

		public double GetCrashPenalty()
		{
			return 100.0*GetCrashesCount()/settings.CrashLimit;
		}

		public double GetBadFraction()
		{
			var shots = GameResults.Select(x => x.ShotsCount);
			var badShots = GameResults.Sum(x => x.BadShotsCount);
			return (100.0*badShots)/shots.Sum();
		}

		public int GetCrashesCount()
		{
			return GameResults.Count(x => x.Crashed);
		}

		public int GetShotsMedian()
		{
			var shots = GetShots();
			var count = shots.Count;
			return count%2 == 1
				? shots[count/2]
				: (shots[count/2] + shots[(count + 1)/2])/2;
		}

		public double GetShotsSigma()
		{
			var mean = GetShotsMean();
			return Math.Sqrt(GameResults.Average(x => (x.ShotsCount - mean)*(x.ShotsCount - mean)));
		}

		public double GetShotsMean()
		{
			return GameResults.Average(x => x.ShotsCount);
		}

		public List<int> GetShots()
		{
			var shots = GameResults.Select(x => x.ShotsCount).ToList();
			if (shots.Count == 0)
				shots.Add(1000 * 1000);
			return shots;
		}

		public int GetBadShotsCount()
		{
			return GameResults.Sum(x => x.BadShotsCount);
		}

		public int GetTurnsCount()
		{
			return GameResults.Where(x => !x.Crashed).Sum(x => x.TurnsCount);
		}
	}
}
