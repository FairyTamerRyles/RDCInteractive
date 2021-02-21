using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameBoard gameBoard;
    private AI testAI;

    public Button[] nodeButtons;
    public Button[] branchButtons;
    public Button[] tileButtons;
    public Text[] p1ResourceCounters;
    public Text[] p2ResourceCounters;
    public Text p1Score;
    public Text p2Score;
    public GameObject red1;
    public GameObject red2;
    public GameObject red3;
    public GameObject blue1;
    public GameObject blue2;
    public GameObject blue3;
    public GameObject yellow1;
    public GameObject yellow2;
    public GameObject yellow3;
    public GameObject green1;
    public GameObject green2;
    public GameObject green3;
    public GameObject voidTile;
    public GameObject purpleSlime;
    public GameObject orangeSlime;
    public GameObject purpleVertical;
    public GameObject orangeVertical;
    public GameObject gameOver;
    
    //make setGameControllerReference
    // Start is called before the first frame update
    public void onHover(Button nodeGameObject)
    {
        string tag = nodeGameObject.tag;
        string[] coordinates = tag.Split(',');
        GameBoard.Coordinate buttonCoord = new GameBoard.Coordinate{x = int.Parse(coordinates[0]), y = int.Parse(coordinates[1])};
        if(gameBoard.isValidMove(buttonCoord))
        {
            Debug.Log("It Works!");
        }
        else
        {
            Debug.Log("I want death");
        }
    }
    void Start()
    {
        gameBoard = new GameBoard();
        testAI = new AI();
        testAI.AIGameBoard = gameBoard;
        foreach (GameBoard.Tile tile in gameBoard.GameTiles) 
        {
            string tileTag = (int)tile.resourceType + "." + tile.maxLoad;
            GameObject tileObject = GameObject.FindGameObjectWithTag(tile.coord.x + "," + tile.coord.y);
            List<GameObject> tilePrefab = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().Where(g=>g.tag == tileTag).ToList();
            Instantiate(tilePrefab[0], new Vector3(tileObject.transform.position.x, tileObject.transform.position.y, 1), Quaternion.identity);
        }
    }

    public void onBranchNodeClick(Button button)
    {
        GameBoard.Coordinate gamePieceCoord = parseTag(button);
        if(gameBoard.isValidMove(gamePieceCoord))
        {
            Debug.Log("valid move");
            gameBoard.placePiece(gamePieceCoord);
            if(gameBoard.numMovesMadeThisTurn() == 2)
            {
                GameObject.Find("EndTurnButton").GetComponent<Button>().interactable = true;
            }
            else if (gameBoard.getTurnCounter() <= 4)
            {
                 GameObject.Find("EndTurnButton").GetComponent<Button>().interactable = false;
            }

            string pieceType = button.name.Substring(0, 1);
            switch(pieceType)
            {
                case "N": 
                    if(gameBoard.getCurrentPlayer() == GameBoard.Player.Player1)
                    {
                        Instantiate(orangeSlime, new Vector3(button.transform.position.x + .25f, button.transform.position.y+ .25f, 1), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(purpleSlime, new Vector3(button.transform.position.x+ .25f, button.transform.position.y+ .25f, 1), Quaternion.identity);
                    }
                    break;

                case "B":
                    if(gameBoard.getCurrentPlayer() == GameBoard.Player.Player1)
                    {
                        if(gameBoard.isHorizontalBranch(gamePieceCoord))
                        {
                            Instantiate(orangeVertical, new Vector3(button.transform.position.x, button.transform.position.y, 1), Quaternion.Euler(0, 0, 90));
                        }
                        else
                        {
                            Instantiate(orangeVertical, new Vector3(button.transform.position.x, button.transform.position.y, 1), Quaternion.identity);
                        }
                    }
                    else
                    {
                        if(gameBoard.isHorizontalBranch(gamePieceCoord))
                        {
                            Instantiate(purpleVertical, new Vector3(button.transform.position.x, button.transform.position.y, 1), Quaternion.Euler(0, 0, 90));
                        }
                        else
                        {
                            Instantiate(purpleVertical, new Vector3(button.transform.position.x, button.transform.position.y, 1), Quaternion.identity);
                        }
                    }
                    break;
            }
            updateResourceCounters();
            gameBoard.logGameBoard();

        }
        else
        {
            Debug.Log("invalid move");
        }
    }

    public void endTurn()
    {
        gameBoard.endTurn();
        if(gameBoard.checkForWin() != GameBoard.Player.None)
        {
            updateScore();
            Instantiate(gameOver, new Vector3(0, 0, 1), Quaternion.identity);
            GameObject.Find("Canvas").GetComponent<GraphicRaycaster>().enabled = false;
            //TODO: Give them the option to leave
        }
        else
        {
            updateResourceCounters();
            
            if(gameBoard.getTurnCounter() <= 4)
            {
                GameObject.Find("EndTurnButton").GetComponent<Button>().interactable = false;
            }
            else
            {
                GameObject.Find("Trade-In Button").GetComponent<Button>().interactable = true;
                updateScore();
            }
        }
    }

    public void makeTrade(int[] rChange)
    {
        if(gameBoard.isValidTrade(rChange))
        {
            gameBoard.makeTrade(rChange);
            updateResourceCounters();
            GameObject.Find("Trade-In Button").GetComponent<Button>().interactable = false;
        }
    }

    public int[] getPlayerResources()
    {
        if(gameBoard.getCurrentPlayer() == GameBoard.Player.Player1)
        {
            return gameBoard.getResources(GameBoard.Player.Player1);
        }
        else
        {
            return gameBoard.getResources(GameBoard.Player.Player2);
        }
    }

    private GameBoard.Coordinate parseTag(Button button)
    {
        string tag = button.tag;
        string[] coordinates = tag.Split(',');
        GameBoard.Coordinate buttonCoord = new GameBoard.Coordinate{x = int.Parse(coordinates[0]), y = int.Parse(coordinates[1])};
        return buttonCoord;
    }

    private void updateResourceCounters()
    {
        GameObject currentResourceCounter;
        string counterTag;
        for(int i = 0; i < GameBoard.numResources; ++i)
        {
            counterTag = "P1_" + i;
            currentResourceCounter = GameObject.FindGameObjectWithTag(counterTag);
            currentResourceCounter.GetComponent<Text>().text = gameBoard.getResources(GameBoard.Player.Player1)[i].ToString();

            counterTag = "P2_" + i;
            currentResourceCounter = GameObject.FindGameObjectWithTag(counterTag);
            currentResourceCounter.GetComponent<Text>().text = gameBoard.getResources(GameBoard.Player.Player2)[i].ToString();
        }
    }

    private void updateScore()
    {
        GameObject.Find("Player1_ScoreText").GetComponent<Text>().text = "Player 1\nScore: " + gameBoard.getScore(GameBoard.Player.Player1).ToString();
        GameObject.Find("Player2_ScoreText").GetComponent<Text>().text = "Player 2\nScore: " + gameBoard.getScore(GameBoard.Player.Player2).ToString();
    }
}
