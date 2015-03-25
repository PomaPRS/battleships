using System;

namespace Battleships.AiTester.Entities
{
	public class GameResult
	{
		public GameResult(int shotsCount, int badShotsCount, int turnsCount, bool crashed, TimeSpan totalProcessorTime)
		{
			TotalProcessorTime = totalProcessorTime;
			Crashed = crashed;
			TurnsCount = turnsCount;
			BadShotsCount = badShotsCount;
			ShotsCount = shotsCount;
		}

		public int ShotsCount { get; private set; }
		public int BadShotsCount { get; private set; }
		public int TurnsCount { get; private set; }
		public bool Crashed { get; private set; }
		public TimeSpan TotalProcessorTime { get; private set; }
	}
}
