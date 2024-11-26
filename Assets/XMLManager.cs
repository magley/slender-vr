using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class XMLScore
{
	public string PlayTime { get; set; }
	public int PagesCollected { get; set; }
}

[System.Serializable]
public class Leaderboard
{
	public List<XMLScore> list = new List<XMLScore>();
}

public class XMLManager : MonoBehaviour
{
	public static XMLManager instance;
	public Leaderboard leaderboard;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}

		if (!Directory.Exists(Application.persistentDataPath + "/HighScores/"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/HighScores/");
		}
	}

	public void AddScore(string time, int pages)
	{
		leaderboard.list.Add(new XMLScore { PlayTime = time, PagesCollected = pages });
	}

	public void SaveScores(List<XMLScore> scoresToSave)
	{
		leaderboard.list = scoresToSave;
		XmlSerializer serializer = new XmlSerializer(typeof(Leaderboard));
		FileStream stream = new FileStream(Application.persistentDataPath + "/HighScores/highscores.xml", FileMode.Create);
		serializer.Serialize(stream, leaderboard);
		stream.Close();
	}

	public List<XMLScore> LoadScores()
	{
		if (File.Exists(Application.persistentDataPath + "/HighScores/highscores.xml"))
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Leaderboard));
			FileStream stream = new FileStream(Application.persistentDataPath + "/HighScores/highscores.xml", FileMode.Open);
			leaderboard = serializer.Deserialize(stream) as Leaderboard;
			stream.Close();
		}

		return leaderboard.list;
	}
}
