using System;
using UnityEngine;
using System.Collections;
using System.IO;

[Flags]
//TODO HACK i wish the log manager (a core class) didnt have to know about somthing combat related
public enum LogDestination
{
	Console = 1,
	Combat = 2
}

public class LogManager
{
	//TODO HACK i wish the log manager (a core class) didnt have to know about somthing combat related
	//TODO hmm....should this even be a message
	public static readonly Message<string, string, LogType> CombatLog = new Message<string, string, LogType>();

	private static string combatLogFilePath;

	//TODO hmm...should this be syncronous?
	public static Coroutine Log(string message, LogDestination destination = (LogDestination.Console | LogDestination.Console))
	{
		return CoroutineManager.Main.StartCoroutine(LogRoutine(message, destination));
	}

	public static void NewCombatLog()
	{
		//TODO helper function for dealing with non-existant directories
		string combatLogDirectoryPath = Path.Combine(Application.dataPath, "Combat/Logs");
		combatLogFilePath = Path.Combine(combatLogDirectoryPath, "CombatLog.txt");
		if(!Directory.Exists(combatLogDirectoryPath))
		{
			Directory.CreateDirectory(combatLogDirectoryPath);
		}

		using(var combatLogFileWriter = File.CreateText(combatLogFilePath))
		{
		}
	}

	static void WriteToCombatLog(string message)
	{
		using(var combatLogFileWriter = File.OpenWrite(combatLogFilePath))
		{
			combatLogFileWriter.Seek(0, SeekOrigin.End);
			using(var writer = new StreamWriter(combatLogFileWriter))
			{
				writer.WriteLine(message);
			}
		}
	}

	private static IEnumerator LogRoutine(string message, LogDestination destination)
	{
		if((destination & LogDestination.Console) == LogDestination.Console)
		{
			Debug.Log(message);
		}

		if((destination & LogDestination.Combat) == LogDestination.Combat)
		{
			WriteToCombatLog(message);

			yield return new Routine(CombatLog.WaitSend(message, new System.Diagnostics.StackTrace().ToString(), LogType.Log));
		}

		yield break;
	}
}


