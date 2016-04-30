using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    public static Board board;
    public int width;
    public int height;
    public static GameObject[,] candies;
    public static List<GameObject> candiesMatched;
    public static List<int> newSpawnColumn;

    private int totalCandy;
    private int spawnCounter = 0;
    private bool isRefilling = false;

    

    void Awake()
    {
        board = this;
        candies = new GameObject[width,height];
        candiesMatched = new List<GameObject>();
        newSpawnColumn = new List<int>();

        StartCoroutine("FillTheBoard");

    }

    IEnumerator FillTheBoard()
    {
        GameObject candy = Instantiate(Resources.Load("Candy"), new Vector3(totalCandy % width * 2.5f, height * 2.5f, 0), Quaternion.identity) as GameObject;
        //Debug.Log(totalCandy % width);
        candy.GetComponent<Candy>().columnNumber = totalCandy % width;
        totalCandy++;
        //candy.GetComponent<Candy>().candyNumber = totalCandy;
        

        yield return new WaitForSeconds(0.07f);

        if(totalCandy < height * width)
        {
            StartCoroutine("FillTheBoard");
        }
        else
        {
            StopCoroutine("FillTheBoard");
        }
        
    }

    IEnumerator RefillTheBoard()
    {
        for (int i = 0; i < width; i++)
        {
            foreach(var column in newSpawnColumn)
            {
                if(i == column)
                {
                    GameObject candy = Instantiate(Resources.Load("Candy"), new Vector3(column * 2.5f, (height + 1) * 2.5f, 0), Quaternion.identity) as GameObject;
                    candy.GetComponent<Candy>().columnNumber = column;
                    totalCandy++;
                    newSpawnColumn.Remove(column);
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
            //spawnCounter = 0;
        }
    }

    public void CandyDestroy()
    {
        Invoke("DestroyCandies", 0.1f);
    }
    void DestroyCandies()
    {
        //Debug.Log(candiesMatched.Count);
        if(candiesMatched.Count >= 3)
        {
            foreach (var candy in candiesMatched)
            {
                Destroy(candy);
            }
            totalCandy -= candiesMatched.Count;

            if (!isRefilling)
            {
                Debug.Log("Refilling Called!");
                isRefilling = true;
                StartCoroutine("RefillTheBoard");
            }
            //Invoke("CallRefill", 0);
        }

    }
}
