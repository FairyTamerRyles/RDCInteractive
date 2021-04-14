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
    public GameObject TradeSelectText;
    public GameObject Red1;
    public GameObject Red2;
    public GameObject Red3;
    public GameObject Red4;
    public GameObject Blue1;
    public GameObject Blue2;
    public GameObject Blue3;
    public GameObject Blue4;
    public GameObject Green1;
    public GameObject Green2;
    public GameObject Green3;
    public GameObject Green4;
    public GameObject Yellow1;
    public GameObject Yellow2;
    public GameObject Yellow3;
    public GameObject Yellow4;
    public GameObject Equal;
    public GameObject Plus1;
    public GameObject Plus2;
    public GameObject RedIncrement;
    public GameObject RedDecrement;
    public GameObject BlueIncrement;
    public GameObject BlueDecrement;
    public GameObject GreenIncrement;
    public GameObject GreenDecrement;
    public GameObject YellowIncrement;
    public GameObject YellowDecrement;


    private List<GameObject> RedResourceGraphics = new List<GameObject>();
    private List<GameObject> BlueResourceGraphics = new List<GameObject>();
    private List<GameObject> GreenResourceGraphics = new List<GameObject>();
    private List<GameObject> YellowResourceGraphics = new List<GameObject>();

    private List<GameObject> IncrementCounters = new List<GameObject>();
    private List<GameObject> DecrementCounters = new List<GameObject>();

    private List<GameObject> Counters = new List<GameObject>();

    private List<List<GameObject>> ResourceGraphics = new List<List<GameObject>>();

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

        clearResources();
        int resourceClicked = -1;
        switch(resourceChosen)
        {
            case "RedResource":
                resourceClicked = 0;
                break;
            case "BlueResource":
                resourceClicked = 1;
                break;
            case "GreenResource":
                resourceClicked = 2;
                break;
            case "YellowResource":
                resourceClicked = 3;
                break;
        }

        //New Resource selected
        if(selectedResource != resourceClicked)
        {
            for(int i = 0; i < IncrementCounters.Count; ++i)
            {
                if(i == resourceClicked)
                {
                    IncrementCounters[i].GetComponent<Image>().enabled = false;
                    DecrementCounters[i].GetComponent<Image>().enabled = false;
                    Counters[i].GetComponent<Text>().enabled = false;
                }
                else
                {
                    IncrementCounters[i].GetComponent<Image>().enabled = true;
                    DecrementCounters[i].GetComponent<Image>().enabled = true;
                    Counters[i].GetComponent<Text>().enabled = true;
                }
            }

            resourceTradeCount[resourceClicked]--;
            selectedResource = resourceClicked;
            TradeSelectText.GetComponent<Text>().text = "Select resources to trade in";
        }
        else //Same resource clicked
        {
            for(int i = 0; i < IncrementCounters.Count; ++i)
            {
                IncrementCounters[i].GetComponent<Image>().enabled = false;
                DecrementCounters[i].GetComponent<Image>().enabled = false;
                Counters[i].GetComponent<Text>().enabled = false;
            }
            
            selectedResource = -1;
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            TradeSelectText.GetComponent<Text>().text = "Select the resource you want";
        }

        RedResource.interactable = true;
        BlueResource.interactable = true;
        GreenResource.interactable = true;
        YellowResource.interactable = true;
        updatePlannedTradeGraphics();
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
        updatePlannedTradeGraphics();
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
        updatePlannedTradeGraphics();
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
        TradeSelectText.GetComponent<Text>().text = "Select the resource you want";
        updatePlannedTradeGraphics();
    }
    void Start()
    {
        selectedResource = -1;
        resourceTradeCount = new int[] {0, 0, 0, 0};

        RedResourceGraphics.Add(Red1);
        RedResourceGraphics.Add(Red2);
        RedResourceGraphics.Add(Red3);
        RedResourceGraphics.Add(Red4);
        BlueResourceGraphics.Add(Blue1);
        BlueResourceGraphics.Add(Blue2);
        BlueResourceGraphics.Add(Blue3);
        BlueResourceGraphics.Add(Blue4);
        GreenResourceGraphics.Add(Green1);
        GreenResourceGraphics.Add(Green2);
        GreenResourceGraphics.Add(Green3);
        GreenResourceGraphics.Add(Green4);
        YellowResourceGraphics.Add(Yellow1);
        YellowResourceGraphics.Add(Yellow2);
        YellowResourceGraphics.Add(Yellow3);
        YellowResourceGraphics.Add(Yellow4);
        ResourceGraphics.Add(RedResourceGraphics);
        ResourceGraphics.Add(BlueResourceGraphics);
        ResourceGraphics.Add(GreenResourceGraphics);
        ResourceGraphics.Add(YellowResourceGraphics);

        IncrementCounters.Add(RedIncrement);
        IncrementCounters.Add(BlueIncrement);
        IncrementCounters.Add(GreenIncrement);
        IncrementCounters.Add(YellowIncrement);

        DecrementCounters.Add(RedDecrement);
        DecrementCounters.Add(BlueDecrement);
        DecrementCounters.Add(GreenDecrement);
        DecrementCounters.Add(YellowDecrement);

        Counters.Add(RedCounter);
        Counters.Add(BlueCounter);
        Counters.Add(GreenCounter);
        Counters.Add(YellowCounter);
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

    public void updatePlannedTradeGraphics()
    {
        bool resourceSelected = false;
        int currentResourceDisplay = 0;
        for(int i = 0; i < 4; ++i)
        {
            if(resourceTradeCount[i] == -1)
            {
                for(int h = 0; h < 3; ++h)
                {
                    setResourceGraphic(h, i, false);
                }
                setResourceGraphic(3, i, true);
                resourceSelected = true;
            }
            else
            {
                setResourceGraphic(3, i, false);
                for(int j = 0; j < 3; ++j)
                {
                    if((j < currentResourceDisplay) || j >= (currentResourceDisplay + resourceTradeCount[i]))
                    {
                        setResourceGraphic(j, i, false);
                    }
                    else
                    {
                        setResourceGraphic(j, i, true);
                    }
                }
                currentResourceDisplay += resourceTradeCount[i];
            }
        }

        Equal.SetActive(resourceSelected);
        Plus1.SetActive(resourceSelected);
        Plus2.SetActive(resourceSelected);
    }

    public void setResourceGraphic(int position, int resource, bool b)
    {
        ResourceGraphics[resource][position].SetActive(b);
    }
}


