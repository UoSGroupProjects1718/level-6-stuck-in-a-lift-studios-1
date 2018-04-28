/**
 * This is based on the tutorial series found here: https://ditt.to/game-dev/multiplayer-1
 **/
using UnityEngine;
using UnityEngine.UI;

public class ModalUI : MonoBehaviour {

	public GameObject simpleModal;
	public GameObject okModal;
	public GameObject cancelModal;

	public Text simpleTitle;
	public Text okModalTitle;
	public Text okButtonTitle;
	public Text cancelModalTitle;
	public Text cancelOkButtonTitle;
	public Text cancelCancelButtonTitle;

	public delegate void ButtonResponse();

	private ButtonResponse okButtonResponse;
	private ButtonResponse cancelButtonResponse;

	public ModalUI SetTitleText (string titleText){
		simpleTitle.text = titleText;
		okModalTitle.text = titleText;
		cancelModalTitle.text = titleText;
		return this;
	}

	public ModalUI SetOkButtonText (string okButtonText){
		okButtonTitle.text = okButtonText;
		cancelOkButtonTitle.text = okButtonText;
		return this;
	}

	public ModalUI SetOkButtonResponse(ButtonResponse okResponse){
		okButtonResponse = okResponse;
		return this;
	}

	public ModalUI SetCancelButtonText(string cancelButtonText){
		cancelCancelButtonTitle.text = cancelButtonText;
		return this;
	}

	public ModalUI SetCancelButtonResponse(ButtonResponse cancelResponse){
		cancelButtonResponse = cancelResponse;
		return this;
	}

	public void Show(){
		if (cancelCancelButtonTitle.text != ""){
			cancelModal.SetActive(true);
		} else if (okButtonTitle.text != ""){
			okModal.SetActive(true);
		} else {
			simpleModal.SetActive(true);
		}
		gameObject.SetActive(true);
	}

	public void OkButtonClick(){
		okButtonResponse();
	}

	public void CancelButtonClick(){
		cancelButtonResponse();
	}
}
