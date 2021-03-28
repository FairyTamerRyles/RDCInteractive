using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ChangeScene : MonoBehaviour
{
    // Start is called before the first frame update 
    public void loadlevel(string level)
    {
        //yield return new WaitForSeconds(5);
        StartCoroutine(Action());
        //Invoke("Action", 2.0f);
        SceneManager.LoadScene(level,LoadSceneMode.Single);

    }

    public IEnumerator Action()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Game");
    }
}
