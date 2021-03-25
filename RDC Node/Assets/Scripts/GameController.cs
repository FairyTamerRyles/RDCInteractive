using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class GameController : MonoBehaviour
{
    private GameBoard gameBoard;
    private AI testAI;
    private List<GameObject> piecesPlacedThisTurn;
    private AdamRandomAI randomAI;
    public GameBoard.Player humanPlayer;
    public GameBoard.Player AIPlayer;
    public GameType gameType;

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

    void Start()
    {
        gameType = GameType.AI;
        humanPlayer = GameBoard.Player.Player1;
        AIPlayer = GameBoard.Player.Player2;

        gameBoard = new GameBoard();
        testAI = new AI(humanPlayer, gameBoard);
        randomAI = new AdamRandomAI(gameBoard);
        piecesPlacedThisTurn = new List<GameObject>();
        testAI.AIGameBoard = gameBoard;
        foreach (GameBoard.Tile tile in gameBoard.GameTiles)
        {
            string tileTag = (int)tile.resourceType + "." + tile.maxLoad;
            GameObject tileObject = GameObject.FindGameObjectWithTag(tile.coord.x + "," + tile.coord.y);
            List<GameObject> tilePrefab = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().Where(g=>g.tag == tileTag).ToList();
            GameObject startTile = new GameObject();
            foreach (GameObject o in tilePrefab)
            {
                if(o.name.IndexOf('S') != -1)
                {
                    startTile = o;
                }
            }
            Instantiate(startTile, new Vector3(tileObject.transform.position.x, tileObject.transform.position.y + .1f, 1), Quaternion.identity);
        }
        updateCurrentPlayer();
        GameObject.Find("UndoButton").GetComponent<Button>().interactable = false;
        if(gameBoard.getCurrentPlayer() != humanPlayer)
        {
            blockPlayerFromPlaying();
            StartCoroutine(makeAIMove());
        }
    }

    public void onBranchNodeClick(Button button)
    {
        if(gameBoard.getCurrentPlayer() == humanPlayer || gameType == GameType.Local)
        {
            GameBoard.Coordinate gamePieceCoord = parseTag(button);
            if(gameBoard.isValidMove(gamePieceCoord))
            {
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
                            newGameObject = GameObject.Find(button.tag);
                            newGameObject.GetComponent<Animator>().SetBool("piecePlaced", true);
                            //newGameObject = Instantiate(orangeSlime, new Vector3(button.transform.position.x + .25f, button.transform.position.y+ .25f, 1), Quaternion.identity);
                        }
                        else
                        {
                            newGameObject = GameObject.Find(button.tag);
                            newGameObject.GetComponent<Animator>().SetBool("piecePlaced", true);
                            //newGameObject = Instantiate(purpleSlime, new Vector3(button.transform.position.x+ .25f, button.transform.position.y+ .25f, 1), Quaternion.identity);
                        }
                        break;

                    case "B":
                        if(gameBoard.getCurrentPlayer() == GameBoard.Player.Player1)
                        {
                            if(gameBoard.isHorizontalBranch(gamePieceCoord))
                            {
                                newGameObject = GameObject.Find(button.tag);
                                newGameObject.GetComponent<Animator>().SetBool("piecePlaced", true);
                                newGameObject.GetComponent<Animator>().SetBool("topOrRight", true);
                                //newGameObject = Instantiate(orangeVertical, new Vector3(button.transform.position.x, button.transform.position.y, 1), Quaternion.Euler(0, 0, 90));
                            }
                            else
                            {
                                newGameObject = GameObject.Find(button.tag);
                                newGameObject.GetComponent<Animator>().SetBool("piecePlaced", true);
                                newGameObject.GetComponent<Animator>().SetBool("topOrRight", true);
                                //newGameObject = Instantiate(orangeVertical, new Vector3(button.transform.position.x, button.transform.position.y, 1), Quaternion.identity);
                            }
                        }
                        else
                        {
                            if(gameBoard.isHorizontalBranch(gamePieceCoord))
                            {
                                //newGameObject = Instantiate(purpleVertical, new Vector3(button.transform.position.x, button.transform.position.y, 1), Quaternion.Euler(0, 0, 90));
                                newGameObject = GameObject.Find(button.tag);
                                newGameObject.GetComponent<Animator>().SetBool("piecePlaced", true);
                                newGameObject.GetComponent<Animator>().SetBool("topOrRight", true);
                            }
                            else
                            {
                                //newGameObject = Instantiate(purpleVertical, new Vector3(button.transform.position.x, button.transform.position.y, 1), Quaternion.identity);
                                newGameObject = GameObject.Find(button.tag);
                                newGameObject.GetComponent<Animator>().SetBool("piecePlaced", true);
                                newGameObject.GetComponent<Animator>().SetBool("topOrRight", true);
                            }
                        }
                        break;
                }
                piecesPlacedThisTurn.Add(newGameObject);
                GameObject.Find("UndoButton").GetComponent<Button>().interactable = true;
                updateResourceCounters();
            }
            else
            {
                Debug.Log("invalid move");
            }
        }
    }

    private IEnumerator makeAIMove()
    {
        yield return new WaitForSeconds(3);
        GameBoard boardAfterAIMove = randomAI.makeRandomAIMove(new GameBoard(gameBoard));
        updateBoardGraphic(boardAfterAIMove);
        gameBoard = new GameBoard(boardAfterAIMove);
        updateResourceCounters();
        endTurn();
        enablePlayerPlaying();
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
            updateExhaustedTiles();
            updateCapturedTiles();
            //Not end of game
            if(gameBoard.getTurnCounter() <= 4)
            {
                GameObject.Find("EndTurnButton").GetComponent<Button>().interactable = false;
            }
            else
            {
                GameObject.Find("Trade-In Button").GetComponent<Button>().interactable = true;
                GameObject.Find("EndTurnButton").GetComponent<Button>().interactable = true;
                updateScore();
                updateLargestNetwork();
            }

            //Let AI make a move
            if(gameType == GameType.AI && gameBoard.getCurrentPlayer() != humanPlayer)
            {
                blockPlayerFromPlaying();
                StartCoroutine(makeAIMove());
            }
            else
            {
                enablePlayerPlaying();
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

    private Button findGameObject(GameBoard.Coordinate Coord)
    {
        return GameObject.FindGameObjectWithTag(Coord.x.ToString() + "," + Coord.y.ToString()).GetComponent<Button>();
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
            updateAnimatorCurrentPlayer(1);
            if(gameBoard.getTurnCounter() > 4)
            {
                collectResources();
            }
            GameObject.Find("OrangePlayer").transform.position = new Vector3(GameObject.Find("OrangePlayer").transform.position.x, -3.75f, GameObject.Find("OrangePlayer").transform.position.z);
            GameObject.Find("PurplePlayer").transform.position = new Vector3(GameObject.Find("PurplePlayer").transform.position.x, 10, GameObject.Find("PurplePlayer").transform.position.z);
        }
        else if(gameBoard.getCurrentPlayer() == GameBoard.Player.Player2)
        {
            updateAnimatorCurrentPlayer(2);
            if(gameBoard.getTurnCounter() > 4)
            {
                collectResources();
            }
            GameObject.Find("OrangePlayer").transform.position = new Vector3(GameObject.Find("OrangePlayer").transform.position.x, -10, GameObject.Find("OrangePlayer").transform.position.z);
            GameObject.Find("PurplePlayer").transform.position = new Vector3(GameObject.Find("PurplePlayer").transform.position.x, 3.75f, GameObject.Find("PurplePlayer").transform.position.z);
        }
    }

    void updateAnimatorCurrentPlayer(int currentPlayer)
    {
        GameObject[] branches = GameObject.FindGameObjectsWithTag("branch");
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("node");
        foreach (GameObject b in branches)
        {
            b.GetComponent<Animator>().SetInteger("currentPlayer", currentPlayer);
        }
        foreach (GameObject n in nodes)
        {
            n.GetComponent<Animator>().SetInteger("currentPlayer", currentPlayer);
        }
    }
    void updateAnimatorAITrigger()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("node");
        GameObject[] branches = GameObject.FindGameObjectsWithTag("branch");
        foreach (GameObject b in branches)
        {
            if(AIPlayer == GameBoard.Player.Player1)
            {
                b.GetComponent<Animator>().SetTrigger("AIMove_O");
            }
            else
            {
                b.GetComponent<Animator>().SetTrigger("AIMove_P");
            }
        }
        foreach (GameObject n in nodes)
        {
            if(AIPlayer == GameBoard.Player.Player1)
            {
                n.GetComponent<Animator>().SetTrigger("AIMove_O");
            }
            else
            {
                n.GetComponent<Animator>().SetTrigger("AIMove_P");
            }
        }
    }

    void collectResources()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("node");
        foreach (GameObject n in nodes)
        {
            n.GetComponent<Animator>().SetTrigger("collectResources");
        }
    }

    private void updateScore()
    {
        GameObject.Find("Player1_ScoreText").GetComponent<Text>().text = "Player 1\nScore: " + gameBoard.getScore(GameBoard.Player.Player1).ToString();
        GameObject.Find("Player2_ScoreText").GetComponent<Text>().text = "Player 2\nScore: " + gameBoard.getScore(GameBoard.Player.Player2).ToString();
    }

    public void undo()
    {
        if(gameBoard.getTurnCounter() <= 4)
        {
            GameObject.Find("EndTurnButton").GetComponent<Button>().interactable = false;
        }

        if(gameBoard.mostRecentMove().moveType != GameBoard.MoveType.Trade)
        {
            GameObject toBeDestroyed = piecesPlacedThisTurn[piecesPlacedThisTurn.Count - 1];
            piecesPlacedThisTurn.RemoveAt(piecesPlacedThisTurn.Count - 1);
            toBeDestroyed.GetComponent<Animator>().SetBool("piecePlaced", false);
            if(toBeDestroyed.tag == "branch")
            {
                toBeDestroyed.GetComponent<Animator>().SetBool("topOrRight", false);
                toBeDestroyed.GetComponent<Animator>().SetBool("bottomOrLeft", false);
            }
            //Destroy(toBeDestroyed);
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

    public void blockPlayerFromPlaying()
    {
        GameObject.Find("Canvas").GetComponent<GraphicRaycaster>().enabled = false;
    }

    public void updateBoardGraphic(GameBoard newBoard)
    {
        updateAnimatorAITrigger();
        for(int i = 0; i < GameBoard.boardSize; ++i)
        {
            for(int j = 0; j < GameBoard.boardSize; ++j)
            {
                if(gameBoard.getGameBoard()[i,j] != null)
                {
                    if(gameBoard.getGameBoard()[i,j].player != newBoard.getGameBoard()[i,j].player)
                    {
                        //TODO: Figure out pieceType and player and spawn in new graphic
                        Button buttonToUpdate = findGameObject(gameBoard.getGameBoard()[i,j].coord);
                        if(gameBoard.isNode(gameBoard.getGameBoard()[i,j].coord))
                        {
                            if(newBoard.getCurrentPlayer() == GameBoard.Player.Player1)
                            {
                                GameObject changedSprite = GameObject.Find(buttonToUpdate.tag);
                                changedSprite.GetComponent<Animator>().SetBool("piecePlaced", true);
                                //Instantiate(orangeSlime, new Vector3(buttonToUpdate.transform.position.x + .25f, buttonToUpdate.transform.position.y+ .25f, 1), Quaternion.identity);
                            }
                            else
                            {
                                GameObject changedSprite = GameObject.Find(buttonToUpdate.tag);
                                changedSprite.GetComponent<Animator>().SetBool("piecePlaced", true);
                                //Instantiate(purpleSlime, new Vector3(buttonToUpdate.transform.position.x + .25f, buttonToUpdate.transform.position.y+ .25f, 1), Quaternion.identity);
                            }
                        }
                        else if(gameBoard.isVerticalBranch(gameBoard.getGameBoard()[i,j].coord))
                        {
                            if(newBoard.getCurrentPlayer() == GameBoard.Player.Player1)
                            {
                                
                                GameObject changedSprite = GameObject.Find(buttonToUpdate.tag);
                                changedSprite.GetComponent<Animator>().SetBool("piecePlaced", true);
                                changedSprite.GetComponent<Animator>().SetBool("topOrRight", true);
                                //Instantiate(orangeVertical, new Vector3(buttonToUpdate.transform.position.x, buttonToUpdate.transform.position.y, 1), Quaternion.identity);
                            }
                            else
                            {
                                GameObject changedSprite = GameObject.Find(buttonToUpdate.tag);
                                changedSprite.GetComponent<Animator>().SetBool("piecePlaced", true);
                                changedSprite.GetComponent<Animator>().SetBool("topOrRight", true);
                                //Instantiate(purpleVertical, new Vector3(buttonToUpdate.transform.position.x, buttonToUpdate.transform.position.y, 1), Quaternion.identity);
                            }
                        }
                        else if(gameBoard.isHorizontalBranch(gameBoard.getGameBoard()[i,j].coord))
                        {
                            if(newBoard.getCurrentPlayer() == GameBoard.Player.Player1)
                            {
                                GameObject changedSprite = GameObject.Find(buttonToUpdate.tag);
                                changedSprite.GetComponent<Animator>().SetBool("piecePlaced", true);
                                changedSprite.GetComponent<Animator>().SetBool("topOrRight", true);
                                //Instantiate(orangeVertical, new Vector3(buttonToUpdate.transform.position.x, buttonToUpdate.transform.position.y, 1), Quaternion.Euler(0, 0, 90));
                            }
                            else
                            {
                                GameObject changedSprite = GameObject.Find(buttonToUpdate.tag);
                                changedSprite.GetComponent<Animator>().SetBool("piecePlaced", true);
                                changedSprite.GetComponent<Animator>().SetBool("topOrRight", true);
                                //Instantiate(purpleVertical, new Vector3(buttonToUpdate.transform.position.x, buttonToUpdate.transform.position.y, 1), Quaternion.Euler(0, 0, 90));
                            }
                        }
                    }
                }
            }
        }
    }

    public void enablePlayerPlaying()
    {
        GameObject.Find("Canvas").GetComponent<GraphicRaycaster>().enabled = true;
    }

    private IEnumerator WaitForSomeTime(int time)
    {
        yield return new WaitForSeconds(time);
    }

    public void updateExhaustedTiles()
    {
        List<GameBoard.Tile> overloadedTiles = gameBoard.overloadedTiles();
        foreach (GameBoard.Tile tile in overloadedTiles) 
        {
            string tileTag = (int)tile.resourceType + "." + tile.maxLoad;
            GameObject tileObject = GameObject.FindGameObjectWithTag(tileTag);
            tileObject.transform.Find("vatIndicator").GetComponent<Animator>().SetBool("closeVat", true);
            //List<GameObject> tilePrefab = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().Where(g=>g.tag == tileTag).ToList();
            //GameObject exhaustedTile = new GameObject();
            //foreach (GameObject o in tilePrefab)
            //{
            //    if(o.name.IndexOf('X') != -1)
            //    {
            //        exhaustedTile = o;
            //        Debug.Log(o + "is the exhausted tile");
            //    }
            //}
            //Instantiate(exhaustedTile, new Vector3(tileObject.transform.position.x, tileObject.transform.position.y, 1), Quaternion.identity);
        }
    }
    public void updateCapturedTiles()
    {
        List<GameBoard.Tile> tiles = gameBoard.getGameTiles();
        foreach (GameBoard.Tile tile in tiles) 
        {
            if(tile.player == GameBoard.Player.Player1)
            {
                string tileTag = (int)tile.resourceType + "." + tile.maxLoad;
                GameObject tileObject = GameObject.FindGameObjectWithTag(tileTag);
                tileObject.transform.Find("vatIndicator").GetComponent<Animator>().SetBool("closeVat", true);
                tileObject.transform.Find("vatIndicator").GetComponent<Animator>().SetBool("captured", true);
                tileObject.transform.Find("vatIndicator").GetComponent<Animator>().SetInteger("player", 1);
                tileObject.transform.Find("vatIndicator").GetComponent<Animator>().SetInteger("resource", (int)tile.resourceType);
                /*List<GameObject> tilePrefab = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().Where(g=>g.tag == tileTag).ToList();
                GameObject capturedTile = new GameObject();
                foreach (GameObject o in tilePrefab)
                {
                    Debug.Log(o);
                    if(o.name.IndexOf("CP2") != -1)
                    {
                        capturedTile = o;
                        Debug.Log(o + "is the captured tile");
                    }
                }
                Instantiate(capturedTile, new Vector3(tileObject.transform.position.x, tileObject.transform.position.y + .19f, 1), Quaternion.identity);*/
            }
            else if (tile.player == GameBoard.Player.Player2)
            {
                string tileTag = (int)tile.resourceType + "." + tile.maxLoad;
                GameObject tileObject = GameObject.FindGameObjectWithTag(tileTag);
                tileObject.transform.Find("vatIndicator").GetComponent<Animator>().SetBool("closeVat", true);
                tileObject.transform.Find("vatIndicator").GetComponent<Animator>().SetBool("captured", true);
                tileObject.transform.Find("vatIndicator").GetComponent<Animator>().SetInteger("player", 2);
                tileObject.transform.Find("vatIndicator").GetComponent<Animator>().SetInteger("resource", (int)tile.resourceType);
                /*List<GameObject> tilePrefab = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().Where(g=>g.tag == tileTag).ToList();
                GameObject capturedTile = new GameObject();
                foreach (GameObject o in tilePrefab)
                {
                    Debug.Log(o);
                    if(o.name.IndexOf("CP1") != -1)
                    {
                        capturedTile = o;
                        Debug.Log(o + "is the captured tile");
                    }
                }
                Instantiate(capturedTile, new Vector3(tileObject.transform.position.x, tileObject.transform.position.y + .19f, 1), Quaternion.identity);*/
            }
        }
    }

    public GameBoard getGameBoard()
    {
        return new GameBoard(gameBoard);
    }
}
