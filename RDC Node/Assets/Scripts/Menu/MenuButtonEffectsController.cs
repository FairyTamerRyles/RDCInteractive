using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonEffectsController : MonoBehaviour
{

    public void enlarge()
    {
        Debug.Log("Mouse is Over");
        GetComponent<RectTransform>().localScale = new Vector3(GetComponent<RectTransform>().localScale.x + .02f, GetComponent<RectTransform>().localScale.x + .02f, 1);
    }
    public void shrink()
    {
        GetComponent<RectTransform>().localScale = new Vector3(GetComponent<RectTransform>().localScale.x - .02f, GetComponent<RectTransform>().localScale.x - .02f, 1);   
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
