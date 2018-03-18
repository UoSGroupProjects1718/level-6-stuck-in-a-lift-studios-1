using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {

	public Sprite[] sprites;
	public int timeMin;
	public int timeMax;

	private Image image;
	private int swapTimer;
	private Sprite lastSprite;

	void Start () {
		image = GetComponent<Image>();
		StartCoroutine(TutorialImageTimer(Random.Range(timeMin, timeMax)));
	}

	private int GetRandom(){
		int result = Random.Range(0, sprites.Length);
		if (sprites[result] == lastSprite){
			return GetRandom();
		}
		lastSprite = sprites[result];
		return result;
	}

	private IEnumerator TutorialImageTimer(int time){
		image.sprite = sprites[GetRandom()];
		swapTimer = 0;
		while (swapTimer < time){
			swapTimer ++;
			yield return new WaitForSeconds(1);
		}
		StartCoroutine(TutorialImageTimer(Random.Range(timeMin, timeMax)));
	}
}
