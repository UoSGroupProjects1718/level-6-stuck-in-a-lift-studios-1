using UnityEngine;
using System.Collections;

namespace Nut {
	public class NutSpin : MonoBehaviour {

		void Update (){
			transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
		}

	}
}
