using UnityEditor;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTemplatePipeline : ISceneTemplatePipeline
{
	public static int seed = 0;

	public virtual bool IsValidTemplateForInstantiation(SceneTemplateAsset sceneTemplateAsset)
	{
		return true;
	}

	public virtual void BeforeTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, bool isAdditive, string sceneName)
	{
		
	}

	public virtual void AfterTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, Scene scene, bool isAdditive, string sceneName)
	{
		GameObject[] gameObjects = scene.GetRootGameObjects();
		GameObject generatorObject = ArrayUtility.Find(gameObjects, x => x.GetComponent<LevelGenerator>() != null);
		LevelGenerator generator = generatorObject?.GetComponent<LevelGenerator>();
		generator.randomSeed = false;
		generator.seed = (uint)seed;
		if (generator != null)
		{
			generator.GenerateLevelSteps();
#if UNITY_EDITOR
			GameObject.DestroyImmediate(generatorObject);
#else
			GameObject.Destroy(generatorObject);
#endif
		}
		else
		{
			Debug.LogError("No Generator component found in the scene.");
		}
	}
}
