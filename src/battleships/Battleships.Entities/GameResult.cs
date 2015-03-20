using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Entities
{
	public class GameResult
	{
		public GameResult(int shotsCount, int badShotsCount, int turnsCount, bool crashed)
		{
			Crashed = crashed;
			TurnsCount = turnsCount;
			BadShotsCount = badShotsCount;
			ShotsCount = shotsCount;
		}

		public int ShotsCount { get; private set; }
		public int BadShotsCount { get; private set; }
		public int TurnsCount { get; private set; }
		public bool Crashed { get; private set; }
	}
}
