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
                    clearResources();
                    GameObject.Find("RedIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("BlueIncrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("GreenIncrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("YellowIncrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("RedDecrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("BlueDecrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("GreenDecrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("YellowDecrement").GetComponent<Image>().enabled = true;
                    resourceTradeCount[0]--;
                    selectedResource = 0;
                    RedCounter.GetComponent<Text>().enabled = false;
                    BlueCounter.GetComponent<Text>().enabled = true;
                    GreenCounter.GetComponent<Text>().enabled = true;
                    YellowCounter.GetComponent<Text>().enabled = true;
                }
                else
                {
                    GameObject.Find("RedIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("BlueIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("GreenIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("YellowIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("RedDecrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("BlueDecrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("GreenDecrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("YellowDecrement").GetComponent<Image>().enabled = false;
                    resourceTradeCount[0]++;
                    selectedResource = -1;
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
                    clearResources();
                    GameObject.Find("RedIncrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("BlueIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("GreenIncrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("YellowIncrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("RedDecrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("BlueDecrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("GreenDecrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("YellowDecrement").GetComponent<Image>().enabled = true;
                    resourceTradeCount[1]--;
                    selectedResource = 1;;
                    RedCounter.GetComponent<Text>().enabled = true;
                    BlueCounter.GetComponent<Text>().enabled = false;
                    GreenCounter.GetComponent<Text>().enabled = true;
                    YellowCounter.GetComponent<Text>().enabled = true;
                }
                else
                {
                    clearResources();
                    GameObject.Find("RedIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("BlueIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("GreenIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("YellowIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("RedDecrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("BlueDecrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("GreenDecrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("YellowDecrement").GetComponent<Image>().enabled = false;
                    resourceTradeCount[1]++;
                    selectedResource = -1;
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
                    clearResources();
                    GameObject.Find("RedIncrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("BlueIncrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("GreenIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("YellowIncrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("RedDecrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("BlueDecrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("GreenDecrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("YellowDecrement").GetComponent<Image>().enabled = true;
                    resourceTradeCount[2]--;
                    selectedResource = 2;
                    RedCounter.GetComponent<Text>().enabled = true;
                    BlueCounter.GetComponent<Text>().enabled = true;
                    GreenCounter.GetComponent<Text>().enabled = false;
                    YellowCounter.GetComponent<Text>().enabled = true;
                }
                else
                {
                    clearResources();
                    GameObject.Find("RedIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("BlueIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("GreenIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("YellowIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("RedDecrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("BlueDecrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("GreenDecrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("YellowDecrement").GetComponent<Image>().enabled = false;
                    resourceTradeCount[2]++;
                    selectedResource = -1;
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
                    clearResources();
                    GameObject.Find("RedIncrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("BlueIncrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("GreenIncrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("YellowIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("RedDecrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("BlueDecrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("GreenDecrement").GetComponent<Image>().enabled = true;
                    GameObject.Find("YellowDecrement").GetComponent<Image>().enabled = false;
                    resourceTradeCount[3]--;
                    selectedResource = 3;
                    RedCounter.GetComponent<Text>().enabled = true;
                    BlueCounter.GetComponent<Text>().enabled = true;
                    GreenCounter.GetComponent<Text>().enabled = true;
                    YellowCounter.GetComponent<Text>().enabled = false;
                }
                else
                {
                    GameObject.Find("RedIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("BlueIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("GreenIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("YellowIncrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("RedDecrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("BlueDecrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("GreenDecrement").GetComponent<Image>().enabled = false;
                    GameObject.Find("YellowDecrement").GetComponent<Image>().enabled = false;
                    resourceTradeCount[3]++;
                    selectedResource = -1;
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
        GameObject.Find("RedIncrement").GetComponent<Image>().enabled = false;
        GameObject.Find("BlueIncrement").GetComponent<Image>().enabled = false;
        GameObject.Find("GreenIncrement").GetComponent<Image>().enabled = false;
        GameObject.Find("YellowIncrement").GetComponent<Image>().enabled = false;
        GameObject.Find("RedDecrement").GetComponent<Image>().enabled = false;
        GameObject.Find("BlueDecrement").GetComponent<Image>().enabled = false;
        GameObject.Find("GreenDecrement").GetComponent<Image>().enabled = false;
        GameObject.Find("YellowDecrement").GetComponent<Image>().enabled = false;
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
        selectedResource = -1;
    }
    void Start()
    {
        selectedResource = -1;
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
