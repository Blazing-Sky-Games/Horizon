using System;
using UnityEngine;
using System.Collections;

[Flags]
public enum LogDestination
{
	Console = 1,
	Combat = 2
}

public class LogManager
{
	public static readonly Message<string, string, LogType> CombatLog = new Message<string, string, LogType>();

	public static Coroutine Log(string message, LogDestination destination = (LogDestination.Console | LogDestination.Console))
	{
		return CoroutineManager.Main.StartCoroutine(LogRoutine(message, destination));
	}

	private static IEnumerator LogRoutine(string message, LogDestination destination)
	{
		if((destination & LogDestination.Console) == LogDestination.Console)
		{
			Debug.Log(message);
		}

		if((destination & LogDestination.Combat) == LogDestination.Combat)
		{
			yield return new Routine(CombatLog.WaitSend(message, new System.Diagnostics.StackTrace().ToString(), LogType.Log));
		}

		yield break;
	}
}


