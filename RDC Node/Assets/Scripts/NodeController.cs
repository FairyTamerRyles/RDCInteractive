using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject orangeSlime;
    public GameObject purpleSlime;
    public int random;
    private GameObject gameController;

    public void SpawnSlime()
    {
        if(random == 1)
        {
            Instantiate(orangeSlime, new Vector3(gameObject.transform.position.x + .25f, gameObject.transform.position.y + .25f, -1f* .1f), Quaternion.identity);
            gameObject.GetComponent<Button>().interactable = false;
        }
        else
        {
            Instantiate(purpleSlime, new Vector3(gameObject.transform.position.x + .25f, gameObject.transform.position.y + .25f, -1f* .1f), Quaternion.identity);
            gameObject.GetComponent<Button>().interactable = false;
        }
    }
    void Start()
    {
        gameController = GameObject.Find("GameControllerObject");
        random = Random.Range(1, 3);
    }

    // Update is called once per frame
}
