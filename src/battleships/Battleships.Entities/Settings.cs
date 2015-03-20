using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Battleships.Entities
{
	[Serializable]
	public class Settings
	{
		private const string DefaultCfgFileName = "battleships.cfg.xml";

		public int TestCount { get; set; }
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

		public void Serialize(string cfgFileName = DefaultCfgFileName)
		{
			var xml = new XmlSerializer(typeof(Settings));
			using (var sw = new StreamWriter(cfgFileName))
			{
				xml.Serialize(sw, this);
			}
		}

		public static Settings Deserialize(string cfgFileName = DefaultCfgFileName)
		{
			var xml = new XmlSerializer(typeof(Settings));
			using (var sr = new StreamReader(cfgFileName))
			{
				return (Settings)xml.Deserialize(sr);
			}
		}
	}
}
