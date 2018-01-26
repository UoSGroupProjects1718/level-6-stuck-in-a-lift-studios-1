﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI.Menu {
	public class SettingsMenu : MonoBehaviour {

		public AudioMixer audioMixer;
		public Dropdown resolutionDropdown;

		private Resolution[] resolutions;

		void Start () {
			resolutions = Screen.resolutions;
			resolutionDropdown.ClearOptions();

			int currentResolutionIndex = 0;
			List<string> options = new List<string>();
			for (int i=0; i<resolutions.Length; i++){
				string option = resolutions[i].width + " x " + resolutions[i].height;
				options.Add(option);

				if (resolutions[i].width == Screen.currentResolution.width &&
					resolutions[i].height == Screen.currentResolution.height){
					currentResolutionIndex = i;
				}
			}

			resolutionDropdown.AddOptions(options);
			resolutionDropdown.value = currentResolutionIndex;
			resolutionDropdown.RefreshShownValue();
		}

		public void SetVolume (float volume){
			audioMixer.SetFloat("volume", volume);
		}

		public void SetResolution(int resolutionIndex){
			Resolution resolution = resolutions[resolutionIndex];
			Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
		}

		public void SetQuality (int qualityIndex){
			QualitySettings.SetQualityLevel(qualityIndex);
		}

		public void SetFullscreen (bool isFullscreen){
			Screen.fullScreen = isFullscreen;
		}
	}
}
