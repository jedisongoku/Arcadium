using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Candy : MonoBehaviour
{
    public delegate void MatchCandy();
    public static event MatchCandy OnMatch;

    public List<Sprite> candySprites = new List<Sprite>(); // Sprite types to be selected randomly

    public bool isChecked = false; // used to make sure the same candy would not be checked again
    public bool isEndChecked = false;
    public int columnNumber; // column number of the candy. It is used for refilling the board.
    public int isEnough = 0; // used to see if a candy have enough neigbors to match
    public static int totalCandyChecked = 0; // used to call "AnyMoveLeft" function on the board after all candies check their neighbors
    public static bool moveLeft = false; // used to check if any move left


    private int candySpriteIndex; // sprite index of the candy. It is also used to check neighbors if they are the same type
    private SpriteRenderer candyRenderer;
    private Vector2 position;
    private List<Candy> neighbors = new List<Candy>(); // list of neighbors of a candy

    void Awake()
    {
        candyRenderer = GetComponent<SpriteRenderer>(); 
    }

    void OnEnable()
    {
        isEndChecked = false;
        isChecked = false;

        if (Random.Range(0,100) > 2)
        {
            candySpriteIndex = Random.Range(0, candySprites.Count - 1); // Randomly select the type of candy
            candyRenderer.sprite = candySprites[candySpriteIndex];
        }
        else
        {
            candySpriteIndex = candySprites.Count - 1; // Randomly select the type of candy
            candyRenderer.sprite = candySprites[candySpriteIndex];
        }
        
    }

    //When player click a candy, this function is called.
    void OnMouseDown()
    {
        Board.candiesMatched.Clear(); //Clears the earlier matches
        Board.playerMove++; //Increments the player move
        HUD_Manager.hud.UpdateHUD(); // updates the HUD
        CheckNeighbor(candySpriteIndex); // Calls a function to check candy's neighbors
        Board.board.BoardCheck(); // Calls for a Board Check to see if there will be any changes
    }

    //Adds the adjacent candies into the neigbors list
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Candy>() != null)
        {
            if (!neighbors.Contains(other.GetComponent<Candy>()))
            {
                neighbors.Add(other.GetComponent<Candy>());
            }
        }
        
    }

    //Removes the candies that are no longer adjacent
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Candy>() != null)
        {
            if (neighbors.Contains(other.GetComponent<Candy>()))
            {
                neighbors.Remove(other.GetComponent<Candy>());
            }
        }
    }

    //Check neigbors. If they are the same type of candy, tell them to check their neigbors as well.
    public void CheckNeighbor(int _candyNumber)
    {
        Board.newSpawnColumn.Add(columnNumber);
        Board.candiesMatched.Add(gameObject);
        OnMatch += IsMatch;
        isChecked = true;

        foreach (var candy in neighbors)
        {
            if((candy.candySpriteIndex == _candyNumber || candy.candySpriteIndex == candySprites.Count - 1) && !candy.isChecked)
            {
                candy.CheckNeighbor(_candyNumber);
            }
        }
    }

    //Check neigbors for end game
    public void CheckEndGame()
    {
        isEnough = 0;
        totalCandyChecked++;

        foreach (var candy in neighbors)
        {
            
            if (candy.candySpriteIndex == candySpriteIndex || candy.candySpriteIndex == candySprites.Count - 1)
            {
                isEnough++;        
            }

        }

        if (isEnough >= 2)
        {
            moveLeft = true;
        }

        if (totalCandyChecked == Board.candies.Count)
        {
            totalCandyChecked = 0;
            Board.board.AnyMoveLeft(moveLeft);

        }
    }

    //A method used to call OnMatch event outside of the script
    public static void CallOnMatch()
    {
        OnMatch();
    }

    void IsMatch()
    {
        if(Board.candiesMatched.Count < 3)
        {
            //prevents new candies to be spawned if there is no match 3+
            Board.newSpawnColumn.Remove(columnNumber);
            isChecked = false;
        }
        else
        {
            //Unsubscribes from the event and plays animation if it is a match 3+
            GetComponent<Animation>().Play("Candy_Disappear");
            GetComponent<AudioSource>().Play();
        }
        OnMatch -= IsMatch;

    }

    //At the end of the animation, this method is being called
    void DestroyCandy()
    {
        gameObject.SetActive(false);
    }


}
