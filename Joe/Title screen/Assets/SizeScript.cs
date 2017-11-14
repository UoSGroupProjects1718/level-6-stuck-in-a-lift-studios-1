using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SizeScript : MonoBehaviour {
	public RotateScript Focus;
	public GameObject one;
	public GameObject two;
	public GameObject three;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Focus.position == 0) {
			one.transform.localScale = new Vector3 (2, 2, 0);
			two.transform.localScale = new Vector3 (1, 1, 0);
			three.transform.localScale = new Vector3 (0.5f, 0.5f, 0);
			one.GetComponent<Text> ().color = new Color (0,0,0,1f);
			two.GetComponent<Text> ().color = new Color (0,0,0,0.5f);
			three.GetComponent<Text> ().color = new Color (0,0,0,0.25f); 
		}

		if (Focus.position == 1) {
			one.transform.localScale = new Vector3 (1, 1, 0);
			two.transform.localScale = new Vector3 (2, 2, 0);
			three.transform.localScale = new Vector3 (1, 1, 0);
			one.GetComponent<Text> ().color = new Color (0,0,0,0.5f);
			two.GetComponent<Text> ().color = new Color (0,0,0,1f);
			three.GetComponent<Text> ().color = new Color (0,0,0,0.5f); 
		}

		if (Focus.position == 2) {
			one.transform.localScale = new Vector3 (0.5f, 0.5f, 0);
			two.transform.localScale = new Vector3 (1, 1, 0);
			three.transform.localScale = new Vector3 (2, 2, 0);
			one.GetComponent<Text> ().color = new Color (0,0,0,0.25f);
			two.GetComponent<Text> ().color = new Color (0,0,0,0.5f);
			three.GetComponent<Text> ().color = new Color (0,0,0,1f); 
		}
	}
}
