using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageBlinking : MonoBehaviour
{
	public float minOpacity;
	public float r;

	private float angle;
	private RawImage image;

	void Start()
	{
		image = GetComponent<RawImage>();
		angle = 0;
	}

	void FixedUpdate()
	{
		float newAlpha = Mathf.Clamp(Mathf.Cos(angle) * r + minOpacity + r, minOpacity, 1.0f);
		Color c = new Vector4(image.color.r, image.color.g, image.color.b, newAlpha);
		image.color = c;
		angle += 0.05f;
	}
}
