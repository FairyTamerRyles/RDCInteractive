using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject orangeSlime;
    public void SpawnSlime()
    {
        Instantiate(orangeSlime, new Vector3(gameObject.transform.position.x + .19f, gameObject.transform.position.y + .2f, -1f* .1f), Quaternion.identity);
        gameObject.GetComponent<Button>().interactable = false;
    }
    void Start()
    {

    }

    // Update is called once per frame
}
