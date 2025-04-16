using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneTemplate;
using System;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

public class SceneInstantiation : MonoBehaviour
{
	public SceneInstantiation Instance { get; private set; }
	public LevelGenerator levelGenerator;
	public Image fadeoutCanvas;
	public float fadeTime = 1f;

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
		FadeOut();
		//Place player at the start of the level
		GenerateLevel();
		FadeIn();
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

	private void FadeOut(){
		float time = 0;
		Color color = fadeoutCanvas.color;
		while (time < fadeTime)
		{
			time += Time.deltaTime;
			color.a = Mathf.Lerp(1, 0, time / fadeTime);
			fadeoutCanvas.color = color;
		}
		color.a = 0;
	}
	private void FadeIn(){
		float time = 0;
		Color color = fadeoutCanvas.color;
		while (time < fadeTime)
		{
			time += Time.deltaTime;
			color.a = Mathf.Lerp(0, 1, time / fadeTime);
			fadeoutCanvas.color = color;
		}
		color.a = 1;
	}
}