using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Math;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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
    public GameObject gameOver;
    public GameObject connectionManager;
    public GameObject matchmakingManager;
    public GameObject gameNetworkingManager;
    public GameObject soundController;
    public GameObject connectionErrorBox;
    public GameObject endGameCanvas;

    public enum GameType
    {
        Local = 0,
        AI = 1,
        Network = 2
    }

    void Start()
    {
        blockPlayerFromPlaying();
        piecesPlacedThisTurn = new List<GameObject>();
        connectionManager = GameObject.Find("ConnectionManager");
        matchmakingManager = GameObject.Find("MatchmakingManager");
        gameNetworkingManager = GameObject.Find("GameNetworkingManager");

        var connectionManagerS = connectionManager.GetComponent<ConnectionManager>();
        var matchmakingManagerS = matchmakingManager.GetComponent<MatchmakingManager>();
        var gameNetworkingManagerS = gameNetworkingManager.GetComponent<GameNetworkingManager>();

        gameNetworkingManagerS.OnOpponentMoved_Callback = null;
        gameNetworkingManagerS.OnRoomFull_Callback = null;
        matchmakingManagerS.OnJoinedRoom_Callback = null;
        matchmakingManagerS.OnLeftRoom_Callback = null;
        matchmakingManagerS.OnJoinRandomFailed_Callback = null;
        matchmakingManagerS.OnCreatePrivateRoomFailed_Callback = null;
        matchmakingManagerS.OnJoinRoomFailed_Callback = null;
        matchmakingManagerS.OnHostSet_Callback = null;
        connectionManagerS.FailedToConnect_Callback = null;
        connectionManagerS.OnConnected_Callback = null;
        connectionManagerS.OnDisconnected_Callback = null;

        soundController = GameObject.Find("SoundManager");
        soundController.GetComponent<SoundManager>().MusicSlider = GameObject.Find("Music").GetComponent<Slider>();
        soundController.GetComponent<SoundManager>().SFXSlider = GameObject.Find("SFX").GetComponent<Slider>();
        soundController.GetComponent<SoundManager>().Play("Intern");

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.EndDrag;
        entry.callback.AddListener( (eventData) => {soundController.GetComponent<SoundManager>().ChangeSFXMasterVolume();} );

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.EndDrag;
        entry2.callback.AddListener( (eventData) => {soundController.GetComponent<SoundManager>().ChangeSoundMasterVolume();} );

        GameObject.Find("SFX").GetComponent<EventTrigger>().triggers.Add(entry);
        GameObject.Find("Music").GetComponent<EventTrigger>().triggers.Add(entry2);

        connectionManager.GetComponent<ConnectionManager>().OnDisconnected_Callback = () => {Disconnected();};
        matchmakingManager.GetComponent<MatchmakingManager>().OnLeftRoom_Callback = () => {Disconnected();};

        if(PlayerPrefs.HasKey("gameType"))
        {
            string gt = PlayerPrefs.GetString("gameType");

            if(gt == "AI")
            {
                gameType = GameType.AI;
            }
            else if(gt == "Network")
            {
                gameType = GameType.Network;
            }
            else
            {
                gameType = GameType.Local;
            }
        }
        else
        {
            gameType = GameType.AI;
        }

        if(PlayerPrefs.HasKey("humanPlayer"))
        {
            if(PlayerPrefs.GetInt("humanPlayer") == 1)
            {
                humanPlayer = GameBoard.Player.Player1;
                AIPlayer = GameBoard.Player.Player2;
            }
            else
            {
                humanPlayer = GameBoard.Player.Player2;
                AIPlayer = GameBoard.Player.Player1;
            }
        }
        else
        {
            humanPlayer = GameBoard.Player.Player1;
            AIPlayer = GameBoard.Player.Player2;
        }

        if(!connectionManager)
        {
            gameType = GameType.AI;
            humanPlayer = GameBoard.Player.Player1;
            AIPlayer = GameBoard.Player.Player2;
        }

        if(gameType != GameType.Network)
        {
            gameBoard = new GameBoard();
            if(PlayerPrefs.GetString("difficulty") == "Hard")
            {
                testAI = new AI(humanPlayer, gameBoard, true);
            }
            else
            {
                testAI = new AI(humanPlayer, gameBoard, false);
            }
            //randomAI = new AdamRandomAI(gameBoard);
            testAI.AIGameBoard = gameBoard;
            initializeTileGraphics();
            updateCurrentPlayer();
            GameObject.Find("UndoButton").GetComponent<Button>().interactable = false;

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
        else
        {
            gameBoard = new GameBoard();
            if(humanPlayer == GameBoard.Player.Player1)
            {
                gameNetworkingManager.GetComponent<GameNetworkingManager>().Board = gameBoard.serializeBoard();
                gameNetworkingManager.GetComponent<GameNetworkingManager>().OnOpponentMoved_Callback = () => {onNetworkOpponentMoved();};

                initializeTileGraphics();
                updateCurrentPlayer();
                GameObject.Find("UndoButton").GetComponent<Button>().interactable = false;
                enablePlayerPlaying();
            }
            else
            {
                gameNetworkingManager.GetComponent<GameNetworkingManager>().OnOpponentMoved_Callback = () => {
                    gameBoard = new GameBoard(gameBoard.deserializeBoard(gameNetworkingManager.GetComponent<GameNetworkingManager>().Board));
                    initializeTileGraphics();
                    updateCurrentPlayer();
                    gameNetworkingManager.GetComponent<GameNetworkingManager>().OnOpponentMoved_Callback = () => {onNetworkOpponentMoved();};
                };
            }
        }
    }

    public void initializeTileGraphics()
    {
        foreach (GameBoard.Tile tile in gameBoard.getGameTiles())
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
            Instantiate(startTile, new Vector3(tileObject.transform.position.x, tileObject.transform.position.y + 0.1f, 1), Quaternion.identity);
        }
    }

    public void updateGameInfoGraphics()
    {
        updateCurrentPlayer();
        updateExhaustedTiles();
        updateCapturedTiles();
        if(gameBoard.getTurnCounter() > 4)
        {
            updateScore();
            updateResourceCounters();
            updateLargestNetwork();
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
                        newGameObject = GameObject.Find(button.tag);
                        int rSound = (int)(Floor(Random.Range(1.0f, 3.0f)));
                        soundController.GetComponent<SoundManager>().PlaySFX("SlimeSpawn" + rSound);
                        newGameObject.GetComponent<Animator>().ResetTrigger("collectResources");
                        newGameObject.GetComponent<Animator>().SetBool("piecePlaced", true);
                        break;

                    case "B":
                        newGameObject = GameObject.Find(button.tag);
                        newGameObject.GetComponent<Animator>().SetBool("piecePlaced", true);
                        newGameObject.GetComponent<Animator>().SetBool("topOrRight", true);
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

    private void onNetworkOpponentMoved()
    {
        GameBoard boardAfterNetworkMove = gameBoard.deserializeBoard(gameNetworkingManager.GetComponent<GameNetworkingManager>().Board);
        updateBoardGraphic(boardAfterNetworkMove);
        gameBoard = new GameBoard(boardAfterNetworkMove);
        updateGameInfoGraphics();
        if(gameBoard.getCurrentPlayer() == humanPlayer)
        {
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
            enablePlayerPlaying();
        }

        if(gameBoard.checkForWin() != GameBoard.Player.None)
        {
            endGame();
        }
    }

    private IEnumerator makeAIMove()
    {
        yield return new WaitForSeconds(5);
        GameBoard boardAfterAIMove = testAI.makeMove(new GameBoard(gameBoard));
        updateBoardGraphic(boardAfterAIMove);
        gameBoard = new GameBoard(boardAfterAIMove);
        updateResourceCounters();
        endTurn();
        enablePlayerPlaying();
    }

    public void endGame()
    {
        updateScore();
        GameObject.Find("Canvas").GetComponent<GraphicRaycaster>().enabled = false;
        if(gameBoard.getScore(GameBoard.Player.Player1) >= 10)
        {
            //TODO: Change Intern to winner
            //TODO: Change Scientist to loser
            GameObject.Find("GameOverBlocker").GetComponent<Animator>().SetInteger("winner", 1);
            GameObject.Find("GameOverBlocker").GetComponent<Animator>().SetInteger("humanPlayer", (int)humanPlayer);
        }
        else
        {
            //TODO: Change Intern to winner
            //TODO: Change Scientist to loser
            GameObject.Find("GameOverBlocker").GetComponent<Animator>().SetInteger("winner", 2);
            GameObject.Find("GameOverBlocker").GetComponent<Animator>().SetInteger("humanPlayer", (int)humanPlayer);
        }

        if(gameType == GameType.Network && gameBoard.checkForWin() == humanPlayer)
        {
            gameNetworkingManager.GetComponent<GameNetworkingManager>().Board = gameBoard.serializeBoard();
        }

        if(gameType == GameType.Network)
        {
            GameObject.Find("PlayAgain").SetActive(false);
        }
        else
        {
            GameObject.Find("PlayAgain").SetActive(true);
            //Bring up end game with both buttons
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
            updateGameInfoGraphics();
            endGame();
        }
        else //Not end of game
        {
            updateGameInfoGraphics();
            if(gameBoard.getTurnCounter() <= 4)
            {
                GameObject.Find("EndTurnButton").GetComponent<Button>().interactable = false;
            }
            else
            {
                GameObject.Find("Trade-In Button").GetComponent<Button>().interactable = true;
                GameObject.Find("EndTurnButton").GetComponent<Button>().interactable = true;
            }

            if(gameType == GameType.Network)
            {
                gameNetworkingManager.GetComponent<GameNetworkingManager>().Board = gameBoard.serializeBoard();
            }

            //Let AI or Network opponent make a move
            if(gameBoard.getCurrentPlayer() != humanPlayer)
            {
                blockPlayerFromPlaying();
                if(gameType == GameType.AI)
                {
                    StartCoroutine(makeAIMove());
                }
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
            GameObject.Find("FickleOrange").GetComponent<Animator>().SetInteger("largestNetworkOwner", 1);
            GameObject.Find("FicklePurple").GetComponent<Animator>().SetInteger("largestNetworkOwner", 1);
            GameObject.Find("FickleN").GetComponent<Animator>().SetInteger("largestNetworkOwner", 1);
        }
        else if(gameBoard.playerWithLargestNetwork() == GameBoard.Player.Player2)
        {
            GameObject.Find("FickleOrange").GetComponent<Animator>().SetInteger("largestNetworkOwner", 2);
            GameObject.Find("FicklePurple").GetComponent<Animator>().SetInteger("largestNetworkOwner", 2);
            GameObject.Find("FickleN").GetComponent<Animator>().SetInteger("largestNetworkOwner", 2);
        }
        else if(gameBoard.playerWithLargestNetwork() == GameBoard.Player.None)
        {
            GameObject.Find("FickleOrange").GetComponent<Animator>().SetInteger("largestNetworkOwner", 0);
            GameObject.Find("FicklePurple").GetComponent<Animator>().SetInteger("largestNetworkOwner", 0);
            GameObject.Find("FickleN").GetComponent<Animator>().SetInteger("largestNetworkOwner", 0);
        }
    }

    public void updateCurrentPlayer()
    {
        if(gameBoard.getCurrentPlayer() == GameBoard.Player.Player1)
        {
            updateAnimatorCurrentPlayer(1);
            soundController.GetComponent<SoundManager>().Transition("Intern");
            if(gameBoard.getTurnCounter() > 4)
            {
                collectResources();
            }
            GameObject.Find("OrangePlayer").GetComponent<Animator>().SetInteger("currentPlayer", 1);
            GameObject.Find("PurplePlayer").GetComponent<Animator>().SetInteger("currentPlayer", 1);
        }
        else if(gameBoard.getCurrentPlayer() == GameBoard.Player.Player2)
        {
            updateAnimatorCurrentPlayer(2);
            if(gameBoard.getTurnCounter() != 3)
            {
                soundController.GetComponent<SoundManager>().Transition("Scientist");
            }
            if(gameBoard.getTurnCounter() > 4)
            {
                collectResources();
            }
            GameObject.Find("OrangePlayer").GetComponent<Animator>().SetInteger("currentPlayer", 2);
            GameObject.Find("PurplePlayer").GetComponent<Animator>().SetInteger("currentPlayer", 2);
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
        if(gameType == GameType.AI)
        {
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
        else if(gameType == GameType.Network)
        {
            foreach (GameObject b in branches)
            {
                if(gameBoard.getCurrentPlayer() == GameBoard.Player.Player1)
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
                if(gameBoard.getCurrentPlayer() == GameBoard.Player.Player1)
                {
                    n.GetComponent<Animator>().SetTrigger("AIMove_O");
                }
                else
                {
                    n.GetComponent<Animator>().SetTrigger("AIMove_P");
                }
            }
        }
    }

    void collectResources()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("node");
        foreach (GameObject n in nodes)
        {
            soundController.GetComponent<SoundManager>().PlaySFX("SlimeCollect");
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

    public void triggerWait(GameBoard.Player player)
    {
        
    }

    public void updateBoardGraphic(GameBoard newBoard)
    {
        updateAnimatorAITrigger();
        bool nodePlaced = false;
        bool branchPlaced = false;
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
                                nodePlaced = true;
                            }
                            else
                            {
                                GameObject changedSprite = GameObject.Find(buttonToUpdate.tag);
                                changedSprite.GetComponent<Animator>().SetBool("piecePlaced", true);
                                nodePlaced = true;
                            }
                        }
                        else if(gameBoard.isVerticalBranch(gameBoard.getGameBoard()[i,j].coord))
                        {
                            if(newBoard.getCurrentPlayer() == GameBoard.Player.Player1)
                            {

                                GameObject changedSprite = GameObject.Find(buttonToUpdate.tag);
                                changedSprite.GetComponent<Animator>().SetBool("piecePlaced", true);
                                changedSprite.GetComponent<Animator>().SetBool("topOrRight", true);
                                branchPlaced = true;
                            }
                            else
                            {
                                GameObject changedSprite = GameObject.Find(buttonToUpdate.tag);
                                changedSprite.GetComponent<Animator>().SetBool("piecePlaced", true);
                                changedSprite.GetComponent<Animator>().SetBool("topOrRight", true);
                                branchPlaced = true;
                            }
                        }
                        else if(gameBoard.isHorizontalBranch(gameBoard.getGameBoard()[i,j].coord))
                        {
                            if(newBoard.getCurrentPlayer() == GameBoard.Player.Player1)
                            {
                                GameObject changedSprite = GameObject.Find(buttonToUpdate.tag);
                                changedSprite.GetComponent<Animator>().SetBool("piecePlaced", true);
                                changedSprite.GetComponent<Animator>().SetBool("topOrRight", true);
                                branchPlaced = true;
                            }
                            else
                            {
                                GameObject changedSprite = GameObject.Find(buttonToUpdate.tag);
                                changedSprite.GetComponent<Animator>().SetBool("piecePlaced", true);
                                changedSprite.GetComponent<Animator>().SetBool("topOrRight", true);
                                branchPlaced = true;
                            }
                        }
                    }
                }
            }
        }
        if(nodePlaced == true)
        {
            int rSound = (int)(Floor(Random.Range(1.0f, 3.0f)));
            soundController.GetComponent<SoundManager>().PlaySFX("SlimeSpawn" + rSound);
        }
        if(branchPlaced == true)
        {
            //TODO: play branch SFX
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
        //soundController.GetComponent<SoundManager>().PlaySFX("Vat Close Sound effect");
        List<GameBoard.Tile> overloadedTiles = gameBoard.overloadedTiles();
        foreach (GameBoard.Tile tile in overloadedTiles)
        {
            string tileTag = (int)tile.resourceType + "." + tile.maxLoad;
            GameObject tileObject = GameObject.FindGameObjectWithTag(tileTag);
            tileObject.transform.Find("vatIndicator").GetComponent<Animator>().SetBool("closeVat", true);
        }
    }
    public void updateCapturedTiles()
    {
        //soundController.GetComponent<SoundManager>().PlaySFX("Vat Capture Sound Effect");
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
            }
            else if (tile.player == GameBoard.Player.Player2)
            {
                string tileTag = (int)tile.resourceType + "." + tile.maxLoad;
                GameObject tileObject = GameObject.FindGameObjectWithTag(tileTag);
                tileObject.transform.Find("vatIndicator").GetComponent<Animator>().SetBool("closeVat", true);
                tileObject.transform.Find("vatIndicator").GetComponent<Animator>().SetBool("captured", true);
                tileObject.transform.Find("vatIndicator").GetComponent<Animator>().SetInteger("player", 2);
                tileObject.transform.Find("vatIndicator").GetComponent<Animator>().SetInteger("resource", (int)tile.resourceType);
            }
        }
    }

    public void playMenu()
    {
        soundController.GetComponent<SoundManager>().Transition("Menu");
    }

    public void turnOffMenu()
    {
        if(gameBoard.getCurrentPlayer() == GameBoard.Player.Player1)
        {
            soundController.GetComponent<SoundManager>().Play("Intern");
        }
        else
        {
            soundController.GetComponent<SoundManager>().Play("Scientist");
        }
    }

    public GameBoard getGameBoard()
    {
        return new GameBoard(gameBoard);
    }

    public void Disconnected()
    {
        if(gameBoard.checkForWin() == GameBoard.Player.None)
        {
            Debug.Log("Disconnected");
            connectionErrorBox.SetActive(true);
            blockPlayerFromPlaying();
            soundController.GetComponent<SoundManager>().Stop();
            connectionErrorBox.GetComponent<GraphicRaycaster>().enabled = true;
        }
    }

    public void returnToMainMenu()
    {
        PlayerPrefs.DeleteKey("humanPlayer");
        PlayerPrefs.DeleteKey("gameType");
        PlayerPrefs.DeleteKey("difficulty");
        //add checks here
        if(matchmakingManager.GetComponent<MatchmakingManager>().InRoom)
        {
            matchmakingManager.GetComponent<MatchmakingManager>().LeaveRoom(() =>{
                Debug.Log("Left Room");
                connectionManager.GetComponent<ConnectionManager>().Disconnect(() =>{
                    Debug.Log("disconnected");
                    Destroy(gameNetworkingManager, 0);
                    Destroy(matchmakingManager, 0);
                    Destroy(connectionManager, 0);
                    Destroy(GameObject.Find("NetworkingObjects"), 0);
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                });
            });
        }
        else if(ConnectionManager.IsConnected())
        {
            connectionManager.GetComponent<ConnectionManager>().Disconnect(() =>{
                Debug.Log("disconnected");
                Destroy(gameNetworkingManager, 0);
                Destroy(matchmakingManager, 0);
                Destroy(connectionManager, 0);
                Destroy(GameObject.Find("NetworkingObjects"), 0);
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            });
        }
        else
        {
            Destroy(gameNetworkingManager, 0);
            Destroy(matchmakingManager, 0);
            Destroy(connectionManager, 0);
            Destroy(GameObject.Find("NetworkingObjects"), 0);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }

    public void playAgain()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void playClick()
    {
        soundController.GetComponent<SoundManager>().PlaySFX("MarkerCap");
    }
}
