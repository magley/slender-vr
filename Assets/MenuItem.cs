using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum MenuItemType
{
	StartGame,
	ExitGame,
}

public class MenuItem : MonoBehaviour
{
	private bool isGazing = false;
	private Outline outline;
	private float gazingCounter = 0;
	[SerializeField] private MenuItemType type;

	private bool opacityShouldGoDown = false;
	public static float UIopacity = 1.0f;

	private Text text;

	public void OnPointerEnter()
	{
		isGazing = true;
		text.color = new(0.9f, 0.075f, 0.1f, 1);
		if (outline)
		{
			outline.enabled = true;
		}
	}

	public void OnPointerExit()
	{
		isGazing = false;
		text.color = new(1, 1, 1, 1);
		if (outline)
		{
			outline.enabled = false;
		}
	}

	private void Start()
	{
		text = GetComponentInChildren<Text>();
		outline = GetComponent<Outline>();
		if (outline)
		{
			outline.enabled = false;
		}
	}

	void Update()
    {
		if (opacityShouldGoDown)
		{
			UIopacity -= 0.15f * Time.deltaTime;
			if (UIopacity < 0)
			{
				UIopacity = 0;
			}

			isGazing = false;
			gazingCounter = 0;

			foreach (var text in FindObjectsOfType<Text>())
			{
				text.color = new(text.color.r, text.color.g, text.color.b, UIopacity);
			}
		}
		else
		{
			if (isGazing)
			{
				gazingCounter += 0.65f * Time.deltaTime;
			}
			else
			{
				gazingCounter -= 0.65f * Time.deltaTime;
			}
			gazingCounter = Mathf.Clamp(gazingCounter, 0, 1);

			if (gazingCounter >= 1.0f)
			{
				OnSelectMenuItem();
			}
		}
    }

	private void OnSelectMenuItem()
	{
		switch (type)
		{
			case MenuItemType.StartGame:
				StartCoroutine(StartGame());
				isGazing = false;
				gazingCounter = 0;
				opacityShouldGoDown = true;
				return;
			case MenuItemType.ExitGame:
				#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
				#else
					Application.Quit();
				#endif
				return;
		}
	}

	IEnumerator StartGame()
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("scn01");

		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}
}
