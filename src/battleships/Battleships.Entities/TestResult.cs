using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Entities
{
	public class TestResult
	{
		public int MapWidth { get; set; }
		public int MapHeight { get; set; }
		public string AiName { get; set; }
		public List<int> Shots { get; set; }
		public List<int> BadShots { get; set; }
		public List<int> Turns { get; set; }
		public int CrashesCount { get; set; }
		public int PlayedGamesCount { get; set; }
	}
}
