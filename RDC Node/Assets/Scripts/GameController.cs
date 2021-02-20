using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private GameBoard gameBoard;
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
    //make setGameControllerReference
    // Start is called before the first frame update
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

    public void endTurn()
    {
        gameBoard.endTurn();
    }
}
