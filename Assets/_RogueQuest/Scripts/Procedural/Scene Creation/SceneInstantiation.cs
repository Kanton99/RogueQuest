using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneTemplate;
using System;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneInstantiation : MonoBehaviour
{
	public SceneInstantiation Instance { get; private set; }
	public LevelGenerator levelGenerator;

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

	public void NewLevelSteps()
	{
		//Fade to black
		//Place player at the start of the level
		GenerateLevel();	
	}

	[ContextMenu("Generate Next Level")]
	public void GenerateLevel()
	{
		if(levelGenerator == null)
		{
			Debug.LogError("No level generator assigned.");
			return;
		}
		int seed = UnityEngine.Random.Range(0, int.MaxValue);

		levelGenerator.seed = (uint)seed;
		levelGenerator.GenerateLevelSteps();
	}
}