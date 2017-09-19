using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCollection : MonoBehaviour {

	void OnTriggerEnter(Collider col){
		Debug.Log(name + " picked up!");
	}
}
