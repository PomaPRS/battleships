using System;

namespace Battleships.AiTester.Entities
{
	public class Statistics
	{
		public string AiName { get; set; }
		public double Mean { get; set; }
		public double Sigma { get; set; }
		public int Median { get; set; }
		public int CrashesCount { get; set; }
		public double BadFraction { get; set; }
		public int GamesPlayed { get; set; }
		public double Score { get; set; }
		public TimeSpan TotalProcessorTime { get; set; }
	}
}
