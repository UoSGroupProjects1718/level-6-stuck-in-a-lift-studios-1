using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutControlScript : MonoBehaviour {
	float maxSize = 10f;
	float startSize = 1f;
	float minSize = 0.1f;
	float currentScale;
	bool upScale;
	int frames = 100;
	float scaling;
	float timeDelay = 0.05f;
	// Use this for initialization
	void Start () {
		int rand = Random.Range (1, 3);
		if (rand == 1) {
			upScale = true;
		}
		if (rand == 2) {
			upScale = false;
		}
		startSize = Random.Range(0.1f,1.75f);
		currentScale = startSize;
		scaling = (maxSize - startSize)/frames;
		StartCoroutine (Scale());	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator Scale(){
		while (true)
		{
			while (upScale)
			{
				currentScale += scaling;
				if (currentScale > maxSize)
				{
					upScale = false;
					currentScale = maxSize;
				}
				gameObject.transform.localScale = new Vector3(1, 1, 0) * currentScale;
				yield return new WaitForSeconds(timeDelay);
			}

			while (!upScale)
			{
				currentScale -= scaling;
				if (currentScale < minSize)
				{
					upScale = true;
					currentScale = minSize;
				}
				gameObject.transform.localScale = new Vector3(1, 1, 0) * currentScale;
				yield return new WaitForSeconds(timeDelay);
			}
		}
	}
}

