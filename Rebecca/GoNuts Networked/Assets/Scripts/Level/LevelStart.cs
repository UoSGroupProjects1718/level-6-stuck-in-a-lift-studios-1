using GameState;
using UnityEngine;

namespace Level {
	class LevelStart : MonoBehaviour {

		void Start () {
			State.GetInstance().Level(State.LEVEL_READY).Publish();
		}
	}
}
