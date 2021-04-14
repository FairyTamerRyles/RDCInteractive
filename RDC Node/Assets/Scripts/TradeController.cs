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
            updateIncrementAndDecrementCounters();
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

    
    public void changeTradeCount(Button ResourceChangeButton)
    {
        int resourceClicked = -1;
        bool incrementing = true;
        string typeOfChange = ResourceChangeButton.tag;
        switch(typeOfChange)
        {
            case "+R":
                resourceClicked = 0;
                break;
            case "-R":
                resourceClicked = 0;
                incrementing = false;
                break;
            case "+B":
                resourceClicked = 1;
                break;
            case "-B":
                resourceClicked = 1;
                incrementing = false;
                break;
            case "+G":
                resourceClicked = 2;
                break;
            case "-G":
                resourceClicked = 2;
                incrementing = false;
                break;
            case "+Y":
                resourceClicked = 3;
                break;
            case "-Y":
                resourceClicked = 3;
                incrementing = false;
                break;
        }

        if(incrementing)
        {
            if(addedResources(resourceTradeCount) < 3 && currentPlayerResources[resourceClicked] > resourceTradeCount[resourceClicked])
            {
                resourceTradeCount[resourceClicked]++;
                if(addedResources(resourceTradeCount) == 3)
                {
                    GameObject.Find("AcceptTrade").GetComponent<Button>().interactable = true;
                }
            }
        }
        else
        {
            if(resourceTradeCount[resourceClicked] > 0)
            {
                resourceTradeCount[resourceClicked]--;
                if(GameObject.Find("AcceptTrade").GetComponent<Button>().interactable == true)
                {
                    GameObject.Find("AcceptTrade").GetComponent<Button>().interactable = false;
                }
            }
        }

        Counters[resourceClicked].GetComponent<Text>().text = resourceTradeCount[resourceClicked].ToString();
        updatePlannedTradeGraphics();
        updateIncrementAndDecrementCounters();
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
        for(int i = 0; i < Counters.Count; ++i)
        {
            Counters[i].GetComponent<Text>().text = "0";
            resourceTradeCount[i] = 0;
        }
        GameObject.Find("AcceptTrade").GetComponent<Button>().interactable = false;
        updatePlannedTradeGraphics();
    }

    public void resetMenu()
    {
        for(int i = 0; i < Counters.Count; ++i)
        {
            IncrementCounters[i].GetComponent<Image>().enabled = false;
            DecrementCounters[i].GetComponent<Image>().enabled = false;
            Counters[i].GetComponent<Text>().enabled = false;
            Counters[i].GetComponent<Text>().text = "0";
            resourceTradeCount[i] = 0;
        }
        GameObject.Find("AcceptTrade").GetComponent<Button>().interactable = false;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null); 
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

    public void updateIncrementAndDecrementCounters()
    {
        for(int i = 0; i < IncrementCounters.Count; ++i)
        {
            IncrementCounters[i].GetComponent<Image>().enabled = ((addedResources(resourceTradeCount) < 3 && currentPlayerResources[i] > resourceTradeCount[i]) && selectedResource != i);
            DecrementCounters[i].GetComponent<Image>().enabled = (resourceTradeCount[i] > 0 && selectedResource != i);
        }
    }

    public void setResourceGraphic(int position, int resource, bool b)
    {
        ResourceGraphics[resource][position].SetActive(b);
    }
}


