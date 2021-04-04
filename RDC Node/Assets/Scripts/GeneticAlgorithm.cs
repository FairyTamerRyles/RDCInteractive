using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;
using UnityEngine.SceneManagement;


public class GeneticAlgorithm : MonoBehaviour
{
    public static int geneLength = 13; //change this when new genes are added
    public class Individual
    {
        public float[] genes;
        public string status;
        public float fitnessScore;
        public AI ai;


        public Individual(string stat, string[] genesToCopy)
        {
            genes = new float[geneLength];
            status = stat;
            for(int i = 0; i < genesToCopy.Length; i++)
            {
                if(i != genesToCopy.Length - 1)
                {
                    genes[i] = float.Parse(genesToCopy[i]);
                }
            }
            fitnessScore = 0;
        }

        public Individual()
        {
            genes = new float[geneLength];
            for(int i = 0; i < genes.Length; i++)
            {
                genes[i] = Random.Range(-1.0f, 1.0f);
            }
            status = "P";
            fitnessScore = 0;
        }

        public Individual(Individual parent1, Individual parent2, string stat, int crossPoint)
        {
            genes = new float[geneLength];

            //up to the crossPoint, the child gets parent1's genes
            //after, the child gets parent2's genes
            for(int i = 0; i < crossPoint; i++)
            {
                genes[i] = parent1.genes[i];
            }
            for(int i = crossPoint; i < geneLength; i++)
            {
                genes[i] = parent2.genes[i];
            }
            for(int i = 0; i < geneLength; i++)
            {
                float mutationChance = Random.Range(0.0f, 100.0f);
                if(mutationChance < 4.0f)
                {
                    genes[i] = Random.Range(-5.0f, 5.0f);
                }

            }
            status = stat;
            fitnessScore = 0;
        }
    }
    
    public Individual monarch;
    public List<Individual> kingdom;
    
    public void reign(int generations)
    {
        float[] royalGenes = new float[geneLength];
        while(generations != 0)
        {
            monarch = kingdom[0];
            //first, the kingdom must all be scored against the Monarch
            for (int i = 1; i < kingdom.Count; i++)
            {
                kingdom[i].fitnessScore = scoreAgainstMonarch(kingdom[i]);
            }

            //now that everyone has been scored, sort them to find the best candidates
            //Note: the king can be overthrown during this process
            kingdom.Sort((x, y) => y.fitnessScore.CompareTo(x.fitnessScore));
            Debug.Log("Top kingdom dwellers in decending order: " + kingdom[0].fitnessScore + " " + kingdom[1].fitnessScore + " " + kingdom[2].fitnessScore);

            //the weak must die. Remove the last six kingdom dwellers
            for(int i = 0; i < 6; i++)
            {
                kingdom.Remove(kingdom[kingdom.Count - 1]);
            }

            //its baby time. Start popping out children. If they aren't the prince and princess, then add them to kingdom.
            float rnd = Random.Range(1.0f, (float)geneLength - 1);
            int crossPoint = (int)(Floor(rnd));
            Individual prince = new Individual(kingdom[0], kingdom[1], "P", crossPoint);
            Individual princess = new Individual(kingdom[1], kingdom[0], "P", crossPoint);

            //marry suitor and second
            rnd = Random.Range(1.0f, (float)geneLength - 1);
            crossPoint = (int)(Floor(rnd));
            kingdom.Add(new Individual(kingdom[1], kingdom[2], "P", crossPoint));
            kingdom.Add(new Individual(kingdom[2], kingdom[1], "P", crossPoint));

            //marry second and third
            rnd = Random.Range(1.0f, (float)geneLength - 1);
            crossPoint = (int)(Floor(rnd));
            kingdom.Add(new Individual(kingdom[3], kingdom[2], "P", crossPoint));
            kingdom.Add(new Individual(kingdom[2], kingdom[3], "P", crossPoint));

            //its time to d-d-d-d-d-d-d--duel!!!!!! First the king faces the prince, winner faces the princess, final winner is the new monarch. Losers are added to the kingdom.
            prince.fitnessScore = scoreAgainstMonarch(prince);
            if(prince.fitnessScore > .5f)
            {
                kingdom.Insert(0, prince);
                //change the new monarch score to .5
            }
            else
            {
                kingdom.Add(prince);
            }
            princess.fitnessScore = scoreAgainstMonarch(princess);
            if(princess.fitnessScore > .5f)
            {
                kingdom.Insert(0, princess);
            }
            else
            {
                kingdom.Add(princess);
            }

            //The kingdom has been fully established. Update the heirarchy
            updateHierarchy();

            //now a new generation rolls in. Continue the simulation
            generations--;
        }
        royalGenes = kingdom[0].genes;
        Debug.Log("Royal Genes: ");
        foreach(float x in royalGenes)
        {
            Debug.Log(x);
        }
    }

    public void updateHierarchy()
    {
        for(int i = 0; i < kingdom.Count; i++)
        {
            if (i == 0)
            {
                kingdom[i].status = "M";
            }
            else
            {
                kingdom[i].status = "P";
            }
        }
    }

    public float scoreAgainstMonarch(Individual peasant)
    {
        int matchesPlayed = 0;
        float peasantWins = 0;
        float monarchWins = 0;
        while(matchesPlayed <= 10)
        {
            //first, randomly generate a gameboard
            GameBoard startingBoard = new GameBoard();

            //if it is an even numbered match, monarch is player one
            //if it is an odd numbered match, peasant(i) goes first
            GameBoard.Player monarchPlayer;
            GameBoard.Player peasantPlayer;
            GameBoard.Player winner;
            if(matchesPlayed % 2 == 0)
            {
                monarchPlayer = GameBoard.Player.Player1;
                peasantPlayer = GameBoard.Player.Player2;
                monarch.ai = new AI(peasantPlayer, startingBoard, true);
                peasant.ai = new AI(monarchPlayer, startingBoard, true);
                monarch.ai.hw = monarch.genes;
                peasant.ai.hw = peasant.genes;
                winner = simulateGame(peasant, monarch, true, startingBoard, 0);
            }
            else
            {
                monarchPlayer = GameBoard.Player.Player2;
                peasantPlayer = GameBoard.Player.Player1;
                monarch.ai = new AI(peasantPlayer, startingBoard, true);
                peasant.ai = new AI(monarchPlayer, startingBoard, true);
                monarch.ai.hw = monarch.genes;
                peasant.ai.hw = peasant.genes;
                winner = simulateGame(peasant, monarch, false, startingBoard, 0);
            }
            if(winner == peasantPlayer)
            {
                peasantWins += 1;
            }
            matchesPlayed++;
        }
        Debug.Log("Peasant Score: " + peasantWins / 10.0f);
        return peasantWins / 10.0f;
    }

    public GameBoard.Player simulateGame(Individual challenger, Individual challengedMonarch, bool monarchTurn, GameBoard currentBoard, int turns)
    {
        GameBoard movedBoard;
        while(turns < 15)
        {    
            if(monarchTurn)
            {
                movedBoard = challengedMonarch.ai.makeMove(new GameBoard(currentBoard));
            }
            else
            {
                movedBoard = challenger.ai.makeMove(new GameBoard(currentBoard));
            }
            movedBoard.endTurn();
            if(movedBoard.checkForWin() == GameBoard.Player.None)
            {
                //the game is still being played. Send it to the next player
                monarchTurn = !monarchTurn;
                turns++;
            }
            else if(movedBoard.checkForWin() == challengedMonarch.ai.self)
            {
                Debug.Log("---------------------------Game Over---------------------------------");
                return challengedMonarch.ai.self;
            }
            else
            {
                Debug.Log("---------------------------Game Over---------------------------------");
                return challenger.ai.self;
            }
        }
        Debug.Log("---------------------------Game Over---------------------------------");
        return GameBoard.Player.None;
    }

    public string serializeIndividual(Individual i)
    {
        string individualString = "";
        if(i == monarch)
        {
            individualString = individualString + "M:";
        }
        else
        {
            individualString = individualString + "P:";
        }
        foreach(float gene in i.genes)
        {
            individualString = individualString + gene + "*";
        }
        individualString = individualString + i.fitnessScore;
        return individualString;
    }

    public string serializeKingdom(List<Individual> land)
    {
        string kingdomString = "";
        kingdomString = kingdomString + serializeIndividual(monarch) + ",";
        for (int i = 1; i < land.Count; i++)
        {
            kingdomString = kingdomString + serializeIndividual(land[i]);
            if(i != land.Count - 1)
            {
                kingdomString = kingdomString + ",";
            }
        }
        return kingdomString;
    }

    public void deserializeKingdom(string kingdom)
    {
        string[] kingdomIndividuals = kingdom.Split(',');
        foreach (string i in kingdomIndividuals)
        {
            deserializeIndividual(i);
        }
    }

    public void deserializeIndividual(string kingdomIndividual)
    {
        string[] classificationAndGenes = kingdomIndividual.Split(':');
        string[] genesAndFitnessScore = classificationAndGenes[1].Split('*');
        if(classificationAndGenes[0] == "M")
        {
            monarch = new Individual("M", genesAndFitnessScore);
        }
        else
        {
            kingdom.Add(new Individual("P", genesAndFitnessScore));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("kingdom") && PlayerPrefs.GetString("Kingdom") != "")
        {
            deserializeKingdom(PlayerPrefs.GetString("kingdom"));
            monarch = kingdom[0];
        }
        else
        {
            kingdom = new List<Individual>();
            for(int i = 0; i < 24; i++)
            {
                kingdom.Add(new Individual());
            }
            monarch = kingdom[0];
        }
        Debug.Log("Start Complete");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
