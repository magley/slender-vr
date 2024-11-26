using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScoreboard : MonoBehaviour
{
    private List<XMLScore> scores = new();
    private Text text;

    void Start()
    {
        text = GetComponent<Text>();
        scores = XMLManager.instance.LoadScores();
        BuildScoreboard();
	}

    private void BuildScoreboard()
    {
        if (scores.Count == 0)
        {
            text.text = "No playthroughs!";
        }
        else
        {
            foreach (XMLScore score in scores)
            {
                string line = $"{score.PlayTime}, {score.PagesCollected}/8 pages\n";
                text.text += line;

		    }
        }
    }
}
