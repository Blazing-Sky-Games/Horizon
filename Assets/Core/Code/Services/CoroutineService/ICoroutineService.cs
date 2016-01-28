using System.Collections;

public interface ICoroutineService : IService
{
	Coroutine StartCoroutine (IEnumerator routine);

	void UpdateCoroutines ();

	void UpdateCoroutines<YeildInstructionType> ()
			where YeildInstructionType : RoutineYieldInstruction, new();
}