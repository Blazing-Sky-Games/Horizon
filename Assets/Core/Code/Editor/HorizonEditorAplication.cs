using UnityEditor;

[InitializeOnLoad]
public class HorizonEditorAplication
{
	static HorizonEditorAplication()
	{
		ServiceLocator.RegisterService<IReflectionService, ReflectionService>();

		EditorApplication.playmodeStateChanged += PlayModeChaged;  
	}

	static void PlayModeChaged()
	{
		if(!EditorApplication.isPlaying)
			ServiceLocator.RegisterService<IReflectionService, ReflectionService>();
	}
}


