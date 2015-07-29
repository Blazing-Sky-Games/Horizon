using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Horizon.Core.WeakSubscription;
using System.Threading;

[ExecuteInEditMode]
public class Dispatcher : MonoBehaviour
{
	public static void CallOnMainThread(Action action)
	{
		if(mainThreadStarted) 
		{
			//TODO add logic to check if the calling thread is the main thread.
			if(Thread.CurrentThread == mainThread)
			{
				action();
			}
			else
			{
				lock(lockObject)
				{
					actions.Add(action);
				}
			}
		}
		else
		{
			lock(lockObject)
			{
				actions.Add(action);
			}
		}
	}

	static void runActions()
	{
		for (int i = 0; i < actions.Count; i++) 
		{
			try 
			{
				actions [i] ();
			}
			catch (Exception e) 
			{
				Debug.LogError (e);
			}
		}
		actions.Clear ();
	}

	public void OnEnable()
	{
		runActions();
		
		mainThread = Thread.CurrentThread;
		mainThreadStarted = true;
	}

	void Update()
	{
		runActions();
	}

	void OnDisable() 
	{
		mainThreadStarted = false;
		mainThread = null;
	}

	public void OnDestroy()
	{
		mainThreadStarted = false;
		mainThread = null;
	}
	
	private static List<Action> actions = new List<Action>();
	private static bool mainThreadStarted = false;
	private static object lockObject = new object();
	private static Thread mainThread;
}

