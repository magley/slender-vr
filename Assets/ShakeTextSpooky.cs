using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShakeTextSpooky : MonoBehaviour
{
    private Text text;
    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        float rotZ = Random.Range(-5, 5);
        text.rectTransform.rotation = Quaternion.Euler(new(0, 0, rotZ));
    }
}
