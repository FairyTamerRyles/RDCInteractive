using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorFunctions : MonoBehaviour
{
	[SerializeField] MenuButtonController menuButtonController;
	public bool disableOnce;

	public AudioClip clickConversationProgress;

	void PlaySound(AudioClip whichSound){
		if(!disableOnce){
			if (whichSound == clickConversationProgress) {
				menuButtonController.soundManager.PlaySFX("ClickConversationProgress");
			} else {
				menuButtonController.soundManager.PlaySFX("ClickConversationEnd");
			}
		}else{
			disableOnce = false;
		}
	}
}	
