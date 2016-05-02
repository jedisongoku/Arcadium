using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    public static Board board; //referance to the board
    public delegate void EndGame();
    public static event EndGame EndBoard;

    public int width; // width of the board
    public int height; // height of the board

    public static List<GameObject> candies = new List<GameObject>(); //all of the candy objects in the game
    public static List<GameObject> candiesMatched = new List<GameObject>(); //candy objects that are matched
    public static List<int> newSpawnColumn = new List<int>(); //column numbers for where to spawn new candies

    public static int playerMove = 0; // stores the player move
    public static int playerScore = 0; // stores the player score


    private AudioSource audioPlayer;
    private int totalCandy; // stores the total cumber of candies on the board
    private bool isRefilling = false; // used for checking if the board is already spawning new candies


    void Awake()
    {
        board = this; // set the reference of the board
        audioPlayer = GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        //Instantiate number of candies to use in object pooling
        for(int i = 0; i < width * height; i++)
        {
            GameObject candy = Instantiate(Resources.Load("Candy"), Vector3.zero, Quaternion.identity) as GameObject;
            candy.SetActive(false);
            candies.Add(candy);
        }

        StartCoroutine("FillTheBoard");

    }

    //Coroutine to fill the board at the beginning
    IEnumerator FillTheBoard()
    {
        //Checks for disabled candies and use them as a new object
        if (!candies[totalCandy].activeInHierarchy)
        {
            candies[totalCandy].transform.position = new Vector3(totalCandy % width * 2.5f, height * 2.5f, 0);
            candies[totalCandy].SetActive(true);
            candies[totalCandy].GetComponent<Candy>().columnNumber = totalCandy % width;
            audioPlayer.Play();

            totalCandy++;
        }

        yield return new WaitForSeconds(0.07f);

        if(totalCandy < candies.Count)
        {
            StartCoroutine("FillTheBoard");
        }
        else
        {
            StopCoroutine("FillTheBoard");
        }
        
    }

    //Coroutine to refill the board
    IEnumerator RefillTheBoard()
    {
        for (int i = 0; i < width; i++)
        {
            //checks each column number and spawns in order to make spawning faster and prevent collision
            foreach (var column in newSpawnColumn)
            {
                if (i == column)
                {

                    for(int j = 0; j < candies.Count; j++)
                    {
                        //Checks for disabled candies and use them as a new object
                        if (!candies[j].activeInHierarchy)
                        {
                            candies[j].SetActive(true);
                            candies[j].transform.position = new Vector3(column * 2.5f, height * 2.5f, 0);
                            candies[j].transform.localScale = new Vector3(1, 1, 1);
                            candies[j].GetComponent<Candy>().columnNumber = column;
                            newSpawnColumn.Remove(column);
                            totalCandy++;
                            audioPlayer.Play();
                            break;
                        }
                    }
                    break;
                } 
            }
        }

        yield return new WaitForSeconds(0.6f);

        if (newSpawnColumn.Count > 0)
        {
            StartCoroutine("RefillTheBoard");
        }
        else
        {
            StopCoroutine("RefillTheBoard");
            isRefilling = false;
            newSpawnColumn.Clear();
            CheckEndGame();
        }
    }

    //A method to call DestroyCandies a bit delayed to make sure candies check their neighbors
    public void BoardCheck()
    {
        Invoke("DestroyCandies", 0);
    }

    //If there is match 3+, Updates the hud and starts refilling the board
    void DestroyCandies()
    {
        if(candiesMatched.Count >= 3)
        {
            
            totalCandy -= candiesMatched.Count;
            if(candiesMatched.Count >= 5)
            {
                playerScore += candiesMatched.Count * candiesMatched.Count;
            }
            else
            {
                playerScore += candiesMatched.Count;
            }
            
            HUD_Manager.hud.UpdateHUD();

            if (!isRefilling)
            {
                isRefilling = true;
                StartCoroutine("RefillTheBoard");
            }
        }
        //Event call for candies that subscribed.
        Candy.CallOnMatch();

    }

    // Call each candy to check their neighbors to see if any move left on the board
    void CheckEndGame()
    {
        foreach(var candy in candies)
        {
            candy.GetComponent<Candy>().CheckEndGame();
        }
    }

    // After each candy checks their neighbors, this method called to check if it is a game over
    public void AnyMoveLeft(bool moveLeft)
    {

        if(moveLeft)
        {
            Candy.moveLeft = false;
        }
        else
        {
            HUD_Manager.hud.gameOverText.gameObject.SetActive(true);
        }
    }
}
