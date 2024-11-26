using System.Collections.Generic;
using UnityEngine;

public class PaperHolder : MonoBehaviour
{
    void Start()
    {
		DecideWhichPaperToSpawn();
	}

    private void DecideWhichPaperToSpawn()
    {
		List<Transform> list = new List<Transform>();
		foreach (Transform child in transform)
		{
			list.Add(child);
		}
		int selectedChildIndex = Random.Range(0, list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			if (i != selectedChildIndex)
			{
				Destroy(list[i].gameObject);
			}
		}
	}
}
