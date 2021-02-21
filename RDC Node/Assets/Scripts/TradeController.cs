using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TradeController : MonoBehaviour
{
    public Button RedResource;
    public Button BlueResource;
    public Button GreenResource;
    public Button YellowResource;
    public GameObject RedCounter;
    public GameObject BlueCounter;
    public GameObject GreenCounter;
    public GameObject YellowCounter;
    public GameObject GameController;

    private int selectedResource;
    private int[] resourceTradeCount;
    private int[] currentPlayerResources;
    private Button disabledIncrement;
    private Button disabledDecrement;
    public void ResourceSelected(Button clickedButton)
    {       
        string resourceChosen = clickedButton.name;
        RedResource.interactable = false;
        BlueResource.interactable = false;
        GreenResource.interactable = false;
        YellowResource.interactable = false;
        switch(resourceChosen)
        {
            case "RedResource":
                if (selectedResource != 0)
                {
                    if (selectedResource != -1)
                    {
                        disabledDecrement.interactable = true;
                        disabledIncrement.interactable = true;
                    }
                    resourceTradeCount[0]--;
                    disabledIncrement = GameObject.Find("RedIncrement").GetComponent<Button>();
                    disabledDecrement = GameObject.Find("RedDecrement").GetComponent<Button>();
                    selectedResource = 0;
                    disabledIncrement.interactable = false;
                    disabledDecrement.interactable = false;
                    BlueCounter.GetComponent<Text>().enabled = true;
                    GreenCounter.GetComponent<Text>().enabled = true;
                    YellowCounter.GetComponent<Text>().enabled = true;
                }
                else
                {
                    resourceTradeCount[0]++;
                    disabledDecrement.interactable = true;
                    disabledIncrement.interactable = true;
                    selectedResource = -1;
                    disabledDecrement = null;
                    disabledIncrement = null;
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                    clearResources();
                    BlueCounter.GetComponent<Text>().enabled = false;
                    GreenCounter.GetComponent<Text>().enabled = false;
                    YellowCounter.GetComponent<Text>().enabled = false;
                }
                break;
            case "BlueResource":
                if (selectedResource != 1)
                {
                    if (selectedResource != -1)
                    {
                        disabledDecrement.interactable = true;
                        disabledIncrement.interactable = true;
                    }
                    resourceTradeCount[1]--;
                    selectedResource = 1;
                    disabledIncrement = GameObject.Find("BlueIncrement").GetComponent<Button>();
                    disabledDecrement = GameObject.Find("BlueDecrement").GetComponent<Button>();
                    disabledIncrement.interactable = false;
                    disabledDecrement.interactable = false;
                    RedCounter.GetComponent<Text>().enabled = true;
                    GreenCounter.GetComponent<Text>().enabled = true;
                    YellowCounter.GetComponent<Text>().enabled = true;
                }
                else
                {
                    resourceTradeCount[1]++;
                    disabledDecrement.interactable = true;
                    disabledIncrement.interactable = true;
                    selectedResource = -1;
                    disabledDecrement = null;
                    disabledIncrement = null;
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                    clearResources();
                    RedCounter.GetComponent<Text>().enabled = false;
                    GreenCounter.GetComponent<Text>().enabled = false;
                    YellowCounter.GetComponent<Text>().enabled = false;
                }
                break;
            case "GreenResource":
                if (selectedResource != 2)
                {
                    if (selectedResource != -1)
                    {
                        disabledDecrement.interactable = true;
                        disabledIncrement.interactable = true;
                    }
                    resourceTradeCount[2]--;
                    selectedResource = 2;
                    disabledIncrement = GameObject.Find("GreenIncrement").GetComponent<Button>();
                    disabledDecrement = GameObject.Find("GreenDecrement").GetComponent<Button>();
                    disabledIncrement.interactable = false;
                    disabledDecrement.interactable = false;
                    RedCounter.GetComponent<Text>().enabled = true;
                    BlueCounter.GetComponent<Text>().enabled = true;
                    YellowCounter.GetComponent<Text>().enabled = true;
                }
                else
                {
                    resourceTradeCount[2]++;
                    disabledDecrement.interactable = true;
                    disabledIncrement.interactable = true;
                    selectedResource = -1;
                    disabledDecrement = null;
                    disabledIncrement = null;
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                    clearResources();
                    RedCounter.GetComponent<Text>().enabled = false;
                    BlueCounter.GetComponent<Text>().enabled = false;
                    YellowCounter.GetComponent<Text>().enabled = false;
                }
                break;
            case "YellowResource":
                if (selectedResource != 3)
                {
                    if (selectedResource != -1)
                    {
                        disabledDecrement.interactable = true;
                        disabledIncrement.interactable = true;
                    }
                    resourceTradeCount[3]--;
                    selectedResource = 3;
                    disabledIncrement = GameObject.Find("YellowIncrement").GetComponent<Button>();
                    disabledDecrement = GameObject.Find("YellowDecrement").GetComponent<Button>();
                    disabledIncrement.interactable = false;
                    disabledDecrement.interactable = false;
                    RedCounter.GetComponent<Text>().enabled = true;
                    BlueCounter.GetComponent<Text>().enabled = true;
                    GreenCounter.GetComponent<Text>().enabled = true;
                }
                else
                {
                    resourceTradeCount[3]++;
                    disabledDecrement.interactable = true;
                    disabledIncrement.interactable = true;
                    selectedResource = -1;
                    disabledDecrement = null;
                    disabledIncrement = null;
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                    clearResources();
                    RedCounter.GetComponent<Text>().enabled = false;
                    BlueCounter.GetComponent<Text>().enabled = false;
                    GreenCounter.GetComponent<Text>().enabled = false;
                }
                break;
        }
        RedResource.interactable = true;
        BlueResource.interactable = true;
        GreenResource.interactable = true;
        YellowResource.interactable = true;
    }
    // Start is called before the first frame update
    public void changeTradeCount(Button ResourceChangeButton)
    {
        string typeOfChange = ResourceChangeButton.tag;
        switch(typeOfChange)
        {
            case "+R":
                if(addedResources(resourceTradeCount) < 3 && currentPlayerResources[0] > resourceTradeCount[0])
                {
                    Debug.Log(addedResources(resourceTradeCount));
                    resourceTradeCount[0]++;
                    RedCounter.GetComponent<Text>().text = resourceTradeCount[0].ToString();
                    if(addedResources(resourceTradeCount) == 3)
                    {
                        Debug.Log("Button Enabled");
                        GameObject.Find("AcceptTrade").GetComponent<Button>().interactable = true;
                    }
                }
                break;
            case "-R":
                if(resourceTradeCount[0] > 0)
                {
                    resourceTradeCount[0]--;
                    RedCounter.GetComponent<Text>().text = resourceTradeCount[0].ToString();
                    if(GameObject.Find("AcceptTrade").GetComponent<Button>().interactable == true)
                    {
                        Debug.Log("Button Disabled");
                        GameObject.Find("AcceptTrade").GetComponent<Button>().interactable = false;
                    }
                }
                break;
            case "+B":
                if(addedResources(resourceTradeCount) < 3 && currentPlayerResources[1] > resourceTradeCount[1])
                {
                    resourceTradeCount[1]++;
                    Debug.Log(addedResources(resourceTradeCount));
                    BlueCounter.GetComponent<Text>().text = resourceTradeCount[1].ToString();
                    if(addedResources(resourceTradeCount) == 3)
                    {
                        GameObject.Find("AcceptTrade").GetComponent<Button>().interactable = true;
                    }
                }
                break;
            case "-B":
                if(resourceTradeCount[1] > 0)
                {
                    resourceTradeCount[1]--;
                    BlueCounter.GetComponent<Text>().text = resourceTradeCount[1].ToString();
                    if(GameObject.Find("AcceptTrade").GetComponent<Button>().interactable == true)
                    {
                        GameObject.Find("AcceptTrade").GetComponent<Button>().interactable = false;
                    }
                }
                break;
            case "+G":
                if(addedResources(resourceTradeCount) < 3 && currentPlayerResources[2] > resourceTradeCount[2])
                {
                    resourceTradeCount[2]++;
                    Debug.Log(addedResources(resourceTradeCount));
                    GreenCounter.GetComponent<Text>().text = resourceTradeCount[2].ToString();
                    if(addedResources(resourceTradeCount) == 3)
                    {
                        GameObject.Find("AcceptTrade").GetComponent<Button>().interactable = true;
                    }
                }
                break;
            case "-G":
                if(resourceTradeCount[2] > 0)
                {
                    resourceTradeCount[2]--;
                    GreenCounter.GetComponent<Text>().text = resourceTradeCount[2].ToString();
                    if(GameObject.Find("AcceptTrade").GetComponent<Button>().interactable == true)
                    {
                        GameObject.Find("AcceptTrade").GetComponent<Button>().interactable = false;
                    }
                }
                break;
            case "+Y":
                if(addedResources(resourceTradeCount) < 3 && currentPlayerResources[3] > resourceTradeCount[3])
                {
                    resourceTradeCount[3]++;
                    Debug.Log(addedResources(resourceTradeCount));
                    YellowCounter.GetComponent<Text>().text = resourceTradeCount[3].ToString();
                    if(addedResources(resourceTradeCount) == 3)
                    {
                        GameObject.Find("AcceptTrade").GetComponent<Button>().interactable = true;
                    }
                }
                break;
            case "-Y":
            if(resourceTradeCount[3] > 0)
            {
                resourceTradeCount[3]--;
                YellowCounter.GetComponent<Text>().text = resourceTradeCount[3].ToString();
                if(GameObject.Find("AcceptTrade").GetComponent<Button>().interactable == true)
                {
                    GameObject.Find("AcceptTrade").GetComponent<Button>().interactable = false;
                }
            }
                break;
        }
    }
    public int addedResources(int[] tradeCount)
    {
        if(tradeCount[0] == -1)
        {
            return tradeCount[1] + tradeCount[2] + tradeCount[3];
        }
        else if(tradeCount[1] == -1)
        {
            return tradeCount[0] + tradeCount[2] + tradeCount[3];
        }
        else if(tradeCount[2] == -1)
        {
            return tradeCount[1] + tradeCount[0] + tradeCount[3];
        }
        else
        {
            return tradeCount[1] + tradeCount[2] + tradeCount[0];
        }
    }
    public void clearResources()
    {
        RedCounter.GetComponent<Text>().text = "0";
        BlueCounter.GetComponent<Text>().text = "0";
        GreenCounter.GetComponent<Text>().text = "0";
        YellowCounter.GetComponent<Text>().text = "0";
        resourceTradeCount[0] = 0;
        resourceTradeCount[1] = 0;
        resourceTradeCount[2] = 0;
        resourceTradeCount[3] = 0;
        GameObject.Find("AcceptTrade").GetComponent<Button>().interactable = false;
    }
    public void resetMenu()
    {
        RedCounter.GetComponent<Text>().text = "0";
        BlueCounter.GetComponent<Text>().text = "0";
        GreenCounter.GetComponent<Text>().text = "0";
        YellowCounter.GetComponent<Text>().text = "0";
        resourceTradeCount[0] = 0;
        resourceTradeCount[1] = 0;
        resourceTradeCount[2] = 0;
        resourceTradeCount[3] = 0;
        GameObject.Find("AcceptTrade").GetComponent<Button>().interactable = false;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null); 
        RedCounter.GetComponent<Text>().enabled = false;
        BlueCounter.GetComponent<Text>().enabled = false;
        GreenCounter.GetComponent<Text>().enabled = false;
        YellowCounter.GetComponent<Text>().enabled = false;
        disabledIncrement.interactable = true;
        disabledDecrement.interactable = true;
        disabledDecrement = null;
        disabledIncrement = null;
        selectedResource = -1;
    }
    void Start()
    {
        selectedResource = -1;
        disabledDecrement = null;
        disabledIncrement = null;
        resourceTradeCount = new int[] {0, 0, 0, 0};
    }

    public void onAcceptClick()
    {
        for(int i = 0; i < resourceTradeCount.Length; ++i)
        {
            resourceTradeCount[i] *= -1;
        }

        GameController.GetComponent<GameController>().makeTrade(resourceTradeCount);
        resetMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updatePlayerResources()
    {
        currentPlayerResources =  GameController.GetComponent<GameController>().getPlayerResources();
    }
}
