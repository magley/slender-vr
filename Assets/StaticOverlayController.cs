using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StaticOverlayController : MonoBehaviour
{
	[SerializeField] private Material whiteNoiseMaterial;
	[SerializeField] private LookToWalk player;

	void Update()
	{
		if (player != null)
		{
			whiteNoiseMaterial.SetFloat("_Alpha", player.SawSlender);
		}
	}
}
