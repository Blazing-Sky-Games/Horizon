using System;
using Slash.Unity.DataBind.Core.Data;
using System.Collections;

namespace Core.Scripts.Contexts
{
	public interface ICoroutineService : IService
	{
		Coroutine StartCoroutine (IEnumerator routine);

		void UpdateCoroutines ();

		void UpdateCoroutines<YeildInstructionType> ()
			where YeildInstructionType : RoutineYieldInstruction, new();
	}

}

