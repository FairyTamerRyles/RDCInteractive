using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // when using Event data.
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
	 , IPointerClickHandler 
	 , IPointerEnterHandler
	 , IPointerExitHandler
{
	//[SerializeField] MenuButtonController menuButtonController;
	[SerializeField] Animator animator;
	[SerializeField] AnimatorFunctions animatorFunctions;
	[SerializeField] int thisIndex;

	private AudioSource source;

	void Start()
	{
		source = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
    {
		//if(menuButtonController.index == thisIndex)
		//{
		//	print(thisIndex);
		//	animator.SetBool ("selected", true);
  //          if (Input.GetAxis("Submit") == 1)
  //          {
  //              animator.SetBool("pressed", true);
  //          }
  //          else if (animator.GetBool("pressed"))
  //          {
  //              animator.SetBool("pressed", false);
  //              animatorFunctions.disableOnce = true;
  //          }
  //      }
  //      else
  //      {
  //          animator.SetBool("selected", false);
  //      }
    }

	public void OnPointerEnter(PointerEventData eventData)
	{
		animator.SetBool("selected", true);
	}

	public void OnPointerClick(PointerEventData eventData) // 3
	{	
		animator.SetBool("pressed", true);
		//if (name == "New Game")
  //      {
		//	yield return new WaitForSeconds(5);
		//	SceneManager.LoadScene("Game");
		//}
	}


	public void OnPointerExit(PointerEventData eventData)
	{
		animator.SetBool("pressed", false);
		animatorFunctions.disableOnce = false ;
		animator.SetBool("selected", false);
	}

}
