using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Horizon.Core.WeakSubscription;

[ExecuteInEditMode]
public class Initilizer : MonoBehaviour
{
	public static void CallOnInit(Action action)
	{
		if(initilized) return;

		actions.Add(action);
	}

	static Initilizer()
	{
		actions = new List<Action>();
	}

	public void OnEnable()
	{
		for(int i = 0; i  < actions.Count; i++)
		{
			actions[i]();
		}
		
		actions.Clear();
		
		initilized = true;
	}

	public void OnDestroy()
	{
		actions.Clear();
		initilized = false;
	}

	private static List<Action> actions;
	private static bool initilized = false;
}

