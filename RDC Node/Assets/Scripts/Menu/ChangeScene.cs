using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public Animator transition;
    public float transtionTime = 1f;
    
    // Start is called before the first frame update 
    public void loadlevel(string level)
    {
        if (level == "Quit")
        {
            Application.Quit();
        }
        else
        {
            StartCoroutine(LoadLevel(level));
        }
        
    }
     
    IEnumerator LoadLevel(string level)
    {
        //transition.setTrigger("Start");
        yield return new WaitForSeconds(transtionTime);
        SceneManager.LoadScene(level);
    }
}
