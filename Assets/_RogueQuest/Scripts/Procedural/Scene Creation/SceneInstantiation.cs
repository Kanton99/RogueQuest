using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneTemplate;
using System;

public class SceneInstantiation : MonoBehaviour
{
	public SceneInstantiation Instance { get; private set; }
	public SceneTemplateAsset sceneTemplate;

	private Awaitable<string> newLevel;

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

	[ContextMenu("Generate Next Level")]
	public void GenerateLevel()
	{
		if(sceneTemplate == null)
		{
			Debug.LogError("No scene template assigned.");
			return;
		}
		int seed = UnityEngine.Random.Range(0, int.MaxValue);
		newLevel = GenerateScene(seed);
	}

	[ContextMenu("Load Next Level")]
	public async void LoadLevel(){
		await newLevel;
	}

	private async Awaitable<string> GenerateScene(int seed)
	{
		await Awaitable.BackgroundThreadAsync();
		string levelPath = "Assets/Scenes/Levels/"+seed;

		if (sceneTemplate != null)
		{
			try
			{
				LevelTemplatePipeline.seed = seed;
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