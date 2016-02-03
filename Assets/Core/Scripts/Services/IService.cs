using System.Collections;

public interface IService
{
	IEnumerator WaitLoadService();
	void UnloadService();
}

