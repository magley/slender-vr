using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Death : MonoBehaviour
{
    [SerializeField] private GameObject objUI;
    [SerializeField] private GameObject objSounds;
	[SerializeField] private GameObject objStatsText;
	[SerializeField] private Material whiteNoiseMaterial;

	private int toggleSlenderTimerIndex = 0;
    private List<float> toggleSlenderTimers = new List<float>();


	void Start()
    {
		whiteNoiseMaterial.SetFloat("_Alpha", 0.75f);

		int k = 25 * 2;
        while (k-- > 0)
        {
            if (k % 2 == 0)
            {
				toggleSlenderTimers.Add(Random.Range(0.015f, 0.1f));
			}
            else
            {
				toggleSlenderTimers.Add(Random.Range(0.025f, 0.1f));
			}
        }

        ToggleTimer();
	}

    private void ToggleTimer()
    {
        toggleSlenderTimerIndex++;
        objUI.SetActive(!objUI.activeSelf);
		objSounds.SetActive(!objSounds.activeSelf);

        Debug.Log($"{toggleSlenderTimerIndex} {toggleSlenderTimers.Count}");

		if (toggleSlenderTimerIndex >= toggleSlenderTimers.Count)
		{
			objUI.SetActive(false);
			objSounds.SetActive(false);
			Invoke("FinishDeathScene", 1);
		}
        else
        {
			Invoke("ToggleTimer", toggleSlenderTimers[toggleSlenderTimerIndex]);
		}
	}

	private void FinishDeathScene()
    {
		string timeString = $"{(int)(GameData.TimeEnd - GameData.TimeStart).Minutes}m {(int)(GameData.TimeEnd - GameData.TimeStart).Seconds}s";

		string text = "";
        text += $"Total time: {timeString}\n";
        text += $"Pages collected: {GameData.PagesCollected} / 8";

        objStatsText.GetComponent<Text>().text = text;

        objStatsText.SetActive(true);
		objUI.SetActive(false);
		objSounds.SetActive(false);

		XMLManager.instance.AddScore(timeString, GameData.PagesCollected);
		XMLManager.instance.SaveScores(XMLManager.instance.leaderboard.list);

		Invoke("BackToMenu", 3);
	}

	private void BackToMenu()
	{
		SceneManager.LoadScene("menu");
	}
}
