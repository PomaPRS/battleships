using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Entities
{
	public class Settings
	{
		public int MinWidth { get; set; }
		public int MinHeight { get; set; }
		public int MinGameCount { get; set; }
		public int MaxWidth { get; set; }
		public int MaxHeight { get; set; }
		public int MaxGameCount { get; set; }

		public int CrashLimit { get; set; }
		public int MemoryLimit { get; set; }
		public int? RandomSeed { get; set; }
		public int TimeLimitSeconds { get; set; }
	}
}
