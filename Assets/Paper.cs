using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Paper : MonoBehaviour
{
    private bool isGazing = false;
    private float collectProgress = 0f; // [0.0, 1.0]
    private Outline outline;
    [SerializeField][Range(0, 1)] private float CollectSpeed = 0.1f;
    [SerializeField] AudioClip sndPaper;

    [SerializeField] private GameObject PrefabEnemy;
	private HintController hintController;

	private void Start()
    {
        outline = GetComponent<Outline>();
		if (outline)
		{
			outline.enabled = false;
		}

		hintController = GameObject.FindFirstObjectByType<HintController>();
	}

    private void Update()
    {
        if (isGazing)
        {
            collectProgress += CollectSpeed;

            if (collectProgress >= 1)
            {
                Collect();
            }
        }
        else
        {
            collectProgress -= CollectSpeed;

            if (collectProgress < 0)
            {
                collectProgress = 0;
            }
        }
    }
    public void OnPointerEnter()
    {
        isGazing = true;
        if (outline)
        {
            outline.enabled = true;
		}
	}

    public void OnPointerExit()
    {
        isGazing = false;
		if (outline)
		{
			outline.enabled = false;
		}
	}

    private void Collect()
    {
        GameData.PagesCollected++;
        gameObject.layer = 0;
        gameObject.SetActive(false);
        AudioSource.PlayClipAtPoint(sndPaper, transform.position, 2.5f);
		hintController.HideText();

		int pagesCollected = GetPagesCollected();

		GameObject.Find("UI_PageCounter").GetComponent<Text>().text = $"{pagesCollected}/8 Pages";

        if (GameObject.FindAnyObjectByType<Enemy>() is Enemy enemy)
        {
            enemy.MaybeTeleportBehindPlayer();
		}

		if (pagesCollected == 2)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                Instantiate(PrefabEnemy, new Vector3(0, 1.5f, 0), Quaternion.identity);
            }
        }
    }

    private int GetPagesCollected()
    {
		int pagesLeft = GameObject.FindGameObjectsWithTag("Paper").Length;
		int pagesCollected = 8 - pagesLeft;

        return pagesCollected;
	}
}
