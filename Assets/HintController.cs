using UnityEngine;
using UnityEngine.UI;

public class HintController : MonoBehaviour
{
    private Text text;

	private void Start()
	{
		text = GetComponent<Text>();
	}

	private void Update()
	{
		text.color += new Color(0, 0, 0, Time.deltaTime);
	}

	public void ShowText(string newText)
	{
		Debug.Log(newText);
		if (text.text != newText)
		{
			text.text = newText;
			text.color = new(1, 1, 1, 0);
		}
    }

	public void HideText()
	{
		text.text = "";
	}
}
