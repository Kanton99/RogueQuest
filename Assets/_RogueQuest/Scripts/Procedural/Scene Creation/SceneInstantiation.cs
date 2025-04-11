using UnityEngine;
using UnityEditor.SceneTemplate;
using System;
using System.Threading.Tasks;

public class SceneInstantiation : MonoBehaviour
{
	public SceneInstantiation Instance { get; private set; }
	public SceneTemplateAsset sceneTemplate;

	private void Awake(){
		if (Instance != null && Instance != this)
		{
			Destroy(Instance.gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	[ContextMenu("Tools/Generate Level")]
	public async Task GenerateLevel()
	{
		if(sceneTemplate == null)
		{
			Debug.LogError("No scene template assigned.");
			return;
		}
		int seed = UnityEngine.Random.Range(0, int.MaxValue);
		var generation = GenerateScene(seed);
		for(int i = 0; i < 1000000; i++){
	
		}
		await generation;
	}

	private async Awaitable<string> GenerateScene(int seed)
	{
		await Awaitable.MainThreadAsync();
		string levelPath = "Assets/Scenes/Levels/"+seed;

		if (sceneTemplate != null)
		{
			try
			{
				UnityEditor.SceneTemplate.SceneTemplateService.Instantiate(sceneTemplate, true);
				Debug.Log("Scene template instantiated successfully.");
				return levelPath;
			}
			catch(Exception e)
			{
				Debug.LogError("Failed to instantiate scene template: "+e.Message);
				return null;
			}
		}
		else
		{
			Debug.LogError("No scene template assigned.");
			return null;
		}

	}
}