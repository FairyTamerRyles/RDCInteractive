using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private GameBoard gameBoard;
    private AI testAI;
    private List<GameObject> piecesPlacedThisTurn;

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

    public enum GameType
    {
        Local = 0,
        AI = 1,
        Network = 2
    }
    
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
        piecesPlacedThisTurn = new List<GameObject>();
        testAI.AIGameBoard = gameBoard;
        foreach (GameBoard.Tile tile in gameBoard.GameTiles) 
        {
            string tileTag = (int)tile.resourceType + "." + tile.maxLoad;
            GameObject tileObject = GameObject.FindGameObjectWithTag(tile.coord.x + "," + tile.coord.y);
            List<GameObject> tilePrefab = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().Where(g=>g.tag == tileTag).ToList();
            Instantiate(tilePrefab[0], new Vector3(tileObject.transform.position.x, tileObject.transform.position.y, 1), Quaternion.identity);
        }
        updateCurrentPlayer();
        GameObject.Find("UndoButton").GetComponent<Button>().interactable = false;
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

            GameObject newGameObject = new GameObject();
            string pieceType = button.name.Substring(0, 1);
            switch(pieceType)
            {
                case "N": 
                    if(gameBoard.getCurrentPlayer() == GameBoard.Player.Player1)
                    {
                        newGameObject = Instantiate(orangeSlime, new Vector3(button.transform.position.x + .25f, button.transform.position.y+ .25f, 1), Quaternion.identity);
                    }
                    else
                    {
                        newGameObject = Instantiate(purpleSlime, new Vector3(button.transform.position.x+ .25f, button.transform.position.y+ .25f, 1), Quaternion.identity);
                    }
                    break;

                case "B":
                    if(gameBoard.getCurrentPlayer() == GameBoard.Player.Player1)
                    {
                        if(gameBoard.isHorizontalBranch(gamePieceCoord))
                        {
                            newGameObject = Instantiate(orangeVertical, new Vector3(button.transform.position.x, button.transform.position.y, 1), Quaternion.Euler(0, 0, 90));
                        }
                        else
                        {
                            newGameObject = Instantiate(orangeVertical, new Vector3(button.transform.position.x, button.transform.position.y, 1), Quaternion.identity);
                        }
                    }
                    else
                    {
                        if(gameBoard.isHorizontalBranch(gamePieceCoord))
                        {
                            newGameObject = Instantiate(purpleVertical, new Vector3(button.transform.position.x, button.transform.position.y, 1), Quaternion.Euler(0, 0, 90));
                        }
                        else
                        {
                            newGameObject = Instantiate(purpleVertical, new Vector3(button.transform.position.x, button.transform.position.y, 1), Quaternion.identity);
                        }
                    }
                    break;
            }
            piecesPlacedThisTurn.Add(newGameObject);
            GameObject.Find("UndoButton").GetComponent<Button>().interactable = true;
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
        piecesPlacedThisTurn.Clear();
        GameObject.Find("UndoButton").GetComponent<Button>().interactable = false;
        //End of game
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
            updateCurrentPlayer();
            //Not end of game          
            if(gameBoard.getTurnCounter() <= 4)
            {
                GameObject.Find("EndTurnButton").GetComponent<Button>().interactable = false;
            }
            else
            {
                GameObject.Find("Trade-In Button").GetComponent<Button>().interactable = true;
                updateScore();
                updateLargestNetwork();
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
            GameObject.Find("UndoButton").GetComponent<Button>().interactable = true;
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

    public void updateLargestNetwork()
    {
        if(gameBoard.playerWithLargestNetwork() == GameBoard.Player.Player1)
        {
            GameObject.Find("FickleOrange").transform.position = new Vector3(GameObject.Find("FickleOrange").transform.position.x, -4, GameObject.Find("FickleOrange").transform.position.z);
            GameObject.Find("FicklePurple").transform.position = new Vector3(GameObject.Find("FicklePurple").transform.position.x, 10, GameObject.Find("FicklePurple").transform.position.z);
        }
        else if(gameBoard.playerWithLargestNetwork() == GameBoard.Player.Player2)
        {
            GameObject.Find("FickleOrange").transform.position = new Vector3(GameObject.Find("FickleOrange").transform.position.x, -10, GameObject.Find("FickleOrange").transform.position.z);
            GameObject.Find("FicklePurple").transform.position = new Vector3(GameObject.Find("FicklePurple").transform.position.x, 4, GameObject.Find("FicklePurple").transform.position.z);
        }
        else if(gameBoard.playerWithLargestNetwork() == GameBoard.Player.None)
        {
            GameObject.Find("FickleOrange").transform.position = new Vector3(GameObject.Find("FickleOrange").transform.position.x, -10, GameObject.Find("FickleOrange").transform.position.z);
            GameObject.Find("FicklePurple").transform.position = new Vector3(GameObject.Find("FicklePurple").transform.position.x, 10, GameObject.Find("FicklePurple").transform.position.z);
        }
    }

    public void updateCurrentPlayer()
    {
        if(gameBoard.getCurrentPlayer() == GameBoard.Player.Player1)
        {
            GameObject.Find("OrangePlayer").transform.position = new Vector3(GameObject.Find("OrangePlayer").transform.position.x, -4, GameObject.Find("OrangePlayer").transform.position.z);
            GameObject.Find("PurplePlayer").transform.position = new Vector3(GameObject.Find("PurplePlayer").transform.position.x, 10, GameObject.Find("PurplePlayer").transform.position.z);
        }
        else if(gameBoard.getCurrentPlayer() == GameBoard.Player.Player2)
        {
            GameObject.Find("OrangePlayer").transform.position = new Vector3(GameObject.Find("OrangePlayer").transform.position.x, -10, GameObject.Find("OrangePlayer").transform.position.z);
            GameObject.Find("PurplePlayer").transform.position = new Vector3(GameObject.Find("PurplePlayer").transform.position.x, 4, GameObject.Find("PurplePlayer").transform.position.z);
        }
    }

    private void updateScore()
    {
        GameObject.Find("Player1_ScoreText").GetComponent<Text>().text = "Player 1\nScore: " + gameBoard.getScore(GameBoard.Player.Player1).ToString();
        GameObject.Find("Player2_ScoreText").GetComponent<Text>().text = "Player 2\nScore: " + gameBoard.getScore(GameBoard.Player.Player2).ToString();
    }

    public void undo()
    {
        Debug.Log("Undo called");
        if(gameBoard.getTurnCounter() <= 4)
        {
            GameObject.Find("EndTurnButton").GetComponent<Button>().interactable = false;
        }

        if(gameBoard.mostRecentMove().moveType != GameBoard.MoveType.Trade)
        {
            GameObject toBeDestroyed = piecesPlacedThisTurn[piecesPlacedThisTurn.Count - 1];
            piecesPlacedThisTurn.RemoveAt(piecesPlacedThisTurn.Count - 1);
            Destroy(toBeDestroyed);
        }
        else
        {
            GameObject.Find("Trade-In Button").GetComponent<Button>().interactable = true;
        }
        gameBoard.undo();

        if(gameBoard.mostRecentMove() == null)
        {
            GameObject.Find("UndoButton").GetComponent<Button>().interactable = false;
        }

        updateResourceCounters();
    }
}
