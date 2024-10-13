using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    // Entities and their states / Model
    private List<string> heap = new List<string>();
    private string[] dir = new string[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

    // Keep track of a player's or opponent's hand
    private List<string> enemy = new List<string>();
    private List<string> player = new List<string>();

    public GameObject cardPrefab;
    public GameObject enemyGrid;
    public GameObject playerGrid;
    public GameObject winShow;
    public GameObject loseShow;
    public GameObject drawShow;
    public GameObject startButton;
    public GameObject exerciser;

    // Judge if it's a player round or if the player will draw cards
    // -1 means that the player will no longer draw cards
    // 1 is the current player's turn, 0 is the opponent's turn
    private static int signal = 0;

    private bool gameRunning = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning)
        {
            if (signal == 1)
            {
                signal = 0;
                DrawCard(true);
                int sum = Calculate(player);
                // Judge if the player bust
                if (sum > 21)
                {
                    GameEnd(-1);
                }
            }
            else
            if (signal == 0 || signal == -1) 
            {
                int sum = Calculate(enemy);
                // Judge if the opponent should draw card
                if (sum < 15)
                {
                    DrawCard(false);
                    sum = Calculate(enemy);
                }
                // Judge if the opponent bust
                if (sum > 21)
                {
                    GameEnd(1);
                }
                if (sum >= 15 && sum <= 21 && signal == -1)
                {
                    int score = Calculate(player);
                    if (score == sum)
                    {
                        GameEnd(0);
                    }
                    else
                    if (score > sum)
                    {
                        GameEnd(1);
                    }
                    else
                    if (score < sum)
                    {
                        GameEnd(-1);
                    }
                }
                // If the player will draw card, wait for it's operation
                if (signal == 0)
                {
                    signal = 10;
                }
            }
        }
    }

    // Components /controls
    List<string> Shuffle(List<string> list)
    {
        System.Random random = new System.Random();
        List<string> newList = new List<string>();
        foreach(string item in list)
        {
            newList.Insert(random.Next(newList.Count + 1), item);
        }
        return newList;
    }

    // Calculate the maximum number of points that can be represented by all hands of a side
    int Calculate(List<string> list)
    {
        int ans = 0, A_num = 0;
        foreach(string item in list)
        {
            switch(item) 
            { 
                case "A": A_num++; break;
                case "K": ans += 10; break;
                case "Q": ans += 10; break;
                case "J": ans += 10; break;
                default:ans += int.Parse(item); break;
            }
        }
        if (A_num > 0)
        {
            ans += A_num;
            if (ans + 10 <= 21)
            {
                ans += 10;
            }
        }
        return ans;
    }

    void DrawToEnemy()
    {
        GameObject newCard = GameObject.Instantiate(cardPrefab, enemyGrid.transform);
        enemy.Add(heap[0]);
        newCard.GetComponent<CardDisplay>().SetValueText(heap[0]);
        heap.RemoveAt(0);
    }

    void DrawToPlayer()
    {
        GameObject newCard = GameObject.Instantiate(cardPrefab, playerGrid.transform);
        player.Add(heap[0]);
        newCard.GetComponent<CardDisplay>().SetValueText(heap[0]);
        heap.RemoveAt(0);
    }
    
    void DrawCard(bool flag)
    {
        if (!flag)
        {
            exerciser.GetComponent<Exercisers>().Init("up");
            DrawToEnemy();
        }
        else
        {
            exerciser.GetComponent<Exercisers>().Init("down");
            DrawToPlayer();
        }
    }

    // Button interactions
    public void OnClickContinue()
    {
        if(gameRunning)
        {
            signal = 1;
        }
    }
    public void OnClickEnd()
    {
        if (gameRunning)
        {
            signal = -1;
        }
    }
    public void OnClickStart()
    {
        GameInit();
        GameRunning();
    }

    void GameInit()
    {
        // Refresh model
        signal = 0;
        heap.Clear();
        enemy.Clear();
        player.Clear();
        gameRunning = false;

        // Set the UI controls to invisible
        winShow.SetActive(false);
        loseShow.SetActive(false);
        drawShow.SetActive(false);
        startButton.SetActive(false);

        // Clear Grid
        foreach (Transform child in enemyGrid.GetComponent<Transform>())
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in playerGrid.GetComponent<Transform>())
        {
            Destroy(child.gameObject);
        }

        // Initialize the deck and shuffle
        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                heap.Add(dir[i]);
            }
        }
        heap = Shuffle(heap);

        // Hide the opponent's hand
        enemyGrid.transform.position = enemyGrid.transform.position + new Vector3(0, 200, 0);

        // Initial dealing
        DrawCard(false); DrawCard(false); DrawCard(true); DrawCard(true);
    }

    void GameRunning()
    {
        // Judge Black Jack 
        int playerSum = Calculate(player);
        int enemySum = Calculate(enemy);
        if (playerSum == 21 || enemySum == 21)
        {
            if (playerSum == enemySum)
            {
                GameEnd(0);
            }
            else
            if (playerSum == 21)
            {
                GameEnd(1);
            }
            else
            if(enemySum == 21)
            {
                GameEnd(-1);
            }
            return;
        }

        gameRunning = true;
    }

    void GameEnd(int flag)
    {
        gameRunning = false;

        // Set the correct end window to visible
        switch (flag)
        {
            case 1:winShow.SetActive(true);break ;
            case -1:loseShow.SetActive(true);break ;
            case 0:drawShow.SetActive(true) ;break ;
        }

        // Show the opponent's hand
        enemyGrid.transform.position = enemyGrid.transform.position + new Vector3(0, -200, 0);

        // Prepare for the next game
        startButton.SetActive(true);
    }
}
