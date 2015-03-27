using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Battleships.AiTester.Entities
{
	[Serializable]
	public class Settings
	{
		private const string DefaultCfgFileName = "battleships.cfg.xml";

		public int TestCount { get; set; }
		public int MinWidth { get; set; }
		public int MinHeight { get; set; }
		public int MinGameCount { get; set; }
		public int MinShipSize { get; set; }
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
			using (var sw = new StreamWriter(GetFilePath(cfgFileName)))
			{
				xml.Serialize(sw, this);
			}
		}

		public static Settings Deserialize(string cfgFileName = DefaultCfgFileName)
		{
			var xml = new XmlSerializer(typeof(Settings));
			using (var sr = new StreamReader(GetFilePath(cfgFileName)))
			{
				return (Settings)xml.Deserialize(sr);
			}
		}

		private static string GetFilePath(string fileName)
		{
			string baseDir = AppDomain.CurrentDomain.BaseDirectory;
			// Note RelativeSearchPath can be null even if the doc say something else; don't remove the check
			var searchPath = AppDomain.CurrentDomain.RelativeSearchPath ?? string.Empty;
			string relativeSearchPath = searchPath.Split(';').First();
			string binPath = Path.Combine(baseDir, relativeSearchPath);
			return Path.Combine(binPath, fileName);
		}
	}
}
