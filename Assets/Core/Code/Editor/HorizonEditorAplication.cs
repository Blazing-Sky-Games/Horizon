using UnityEditor;

[InitializeOnLoad]
public class HorizonEditorAplication
{
	static HorizonEditorAplication()
	{
		ServiceLocator.RegisterService<IReflectionService, ReflectionService>();
	}
}


