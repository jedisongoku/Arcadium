using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Candy : MonoBehaviour
{
    public delegate void MatchCandy();
    public static event MatchCandy OnMatch;

    public List<Sprite> candySprites = new List<Sprite>();

    public bool isChecked = false;
    public bool isMatched = false;
    public int columnNumber;
    public int candyNumber;

    private int candySpriteIndex; // sprite index

    private SpriteRenderer candyRenderer;
    private Vector2 position;
    private List<Candy> neighbors = new List<Candy>();

    void Awake()
    {
        //OnMatch += CandyMatch;
        candyRenderer = GetComponent<SpriteRenderer>();
        candySpriteIndex = Random.Range(0, candySprites.Count);
        candyRenderer.sprite = candySprites[candySpriteIndex];
    }

    public void SetPosition(int x, int y)
    {
        position = new Vector2(x, y);
    }

    public int GetCandyNumber()
    {
        return candySpriteIndex;
    }

    void OnMouseDown()
    {
        Board.candiesMatched.Clear();
        //Board.newSpawnColumn.Clear();

        foreach (var candy in neighbors)
        {
            //Debug.Log("Neighbor " + candy.columnNumber);
        }
        
        CheckNeighbor(candySpriteIndex);
        Board.board.CandyDestroy();
        
    }

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


    public void CheckNeighbor(int _candyNumber)
    {
        Board.newSpawnColumn.Add(columnNumber);
        Board.candiesMatched.Add(gameObject);
        //Debug.Log("Column " + columnNumber);
        isChecked = true;

        foreach (var candy in neighbors)
        {
            if(candy.candySpriteIndex == _candyNumber && !candy.isChecked)
            {
                candy.CheckNeighbor(candySpriteIndex);
            }
        }
    }
















    

    /*public void CheckNeighbor(int _candyNumber)
    {
        if(!isChecked)
        {
            isChecked = true;
            if(candyNumber == _candyNumber)
            {
                isMatched = true;

                if (position.x - 1 >= 0)
                {
                    Board.candies[(int)position.x - 1, (int)position.y].GetComponent<Candy>().CheckNeighbor(candyNumber);
                }
                if (position.x + 1 <= Board.board.width)
                {
                    Board.candies[(int)position.x + 1, (int)position.y].GetComponent<Candy>().CheckNeighbor(candyNumber);
                }
                if (position.y - 1 >= 0)
                {
                    Board.candies[(int)position.x, (int)position.y - 1].GetComponent<Candy>().CheckNeighbor(candyNumber);
                }
                if (position.y + 1 >= Board.board.height)
                {
                    Board.candies[(int)position.x, (int)position.y + 1].GetComponent<Candy>().CheckNeighbor(candyNumber);
                }

            }

        }

    }*/




}
