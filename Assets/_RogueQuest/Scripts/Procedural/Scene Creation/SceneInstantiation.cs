using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneTemplate;
using System;
using UnityEditor;

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
	public string GenerateLevel()
	{
		int seed = UnityEngine.Random.Range(0, int.MaxValue);
		string levelPath = "Assets/Scenes/Levels/"+seed;

		if (sceneTemplate != null)
		{
			try
			{
				InstantiationResult result = UnityEditor.SceneTemplate.SceneTemplateService.Instantiate(sceneTemplate, true);
				return levelPath;
			}
			catch
			{
				Debug.LogError("Failed to instantiate scene template.");
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