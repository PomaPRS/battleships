using System;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Battleships.AiTester.Entities
{
	public static class StrategyLogManager
	{
		private static object locker = new Object();

		public static Logger GetLogger(string aiName)
		{
			var targetName = "strategy-" + aiName;
			var logInfos = new[]
			{
				new LogInfo
				{
					LoggerNamePattern = "strategy-" + aiName,
					TargetName = targetName,
					FileName = "${basedir}/logs/strategies/" + aiName + ".log",
					Layout = "${message}",
					LogLevel = LogLevel.Info
				},
				new LogInfo
				{
					LoggerNamePattern = "strategy-" + aiName + "*",
					TargetName = "strategy-" + aiName + "-errors",
					FileName = "${basedir}/logs/strategies/" + aiName + "-errors.log",
					Layout = "${time} ${uppercase:${level}} ${logger} ${message}",
					LogLevel = LogLevel.Error
				}
			};

			SetLogConfiguration(logInfos);
			return LogManager.GetLogger(targetName);
		}

		private static void SetLogConfiguration(params LogInfo[] logInfos)
		{
			foreach (var logInfo in logInfos)
			{
				var target = new FileTarget {Name = logInfo.TargetName, FileName = logInfo.FileName, Layout = logInfo.Layout};
				var loggingRule = new LoggingRule(logInfo.LoggerNamePattern, logInfo.LogLevel, target);
				SetTarget(target);
				SetLoggingRule(loggingRule);
			}
			lock (locker)
			{
				LogManager.Configuration.Reload();
			}
		}

		private static void SetTarget(Target target)
		{
			lock (locker)
			{
				var contains = LogManager.Configuration.AllTargets.Any(x => x.Name == target.Name);
				if (contains) return;
				LogManager.Configuration.AddTarget(target.Name, target);
			}
		}

		private static void SetLoggingRule(LoggingRule loggingRule)
		{
			lock (locker)
			{
				var contains = LogManager.Configuration.LoggingRules.Any(
					x =>
						x.LoggerNamePattern == loggingRule.LoggerNamePattern &&
						x.Levels.SequenceEqual(loggingRule.Levels)
					);
				if (contains) return;
				LogManager.Configuration.LoggingRules.Add(loggingRule);
			}
		}

		class LogInfo
		{
			public string LoggerNamePattern { get; set; }
			public string TargetName { get; set; }
			public string FileName { get; set; }
			public string Layout { get; set; }
			public LogLevel LogLevel { get; set; }
		}
	}
}
