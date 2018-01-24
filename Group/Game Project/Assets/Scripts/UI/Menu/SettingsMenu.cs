using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace UI.Menu {
	public class SettingsMenu : MonoBehaviour {

		public AudioMixer audioMixer;

		void Start () {
			
		}

		public void SetVolume (float volume){
			audioMixer.SetFloat("volume", volume);
		}

		public void SetQuality (int qualityIndex){
			QualitySettings.SetQualityLevel(qualityIndex);
		}

		public void SetFullscreen (bool isFullscreen){
			Screen.fullScreen = isFullscreen;
		}
	}
}
