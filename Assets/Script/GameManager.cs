using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int score;
    private bool win;
    public int levelNumber;
    public int maxLevel = 3;
    public int levelTimeLeft;
    GameObject player;


    public int Score { get => score; set => score = value; }
    public bool Win { get => win; set => win = value; }

    public static GameManager instance;


    private void Start()
    {
        instance = this;
        if (instance != null)
            DontDestroyOnLoad(gameObject);
        else Destroy(gameObject);
    }
    

    public void NextLevel()
    {
        if (levelNumber < maxLevel) {
            levelNumber += 1;
            SceneManager.LoadScene("Level " + levelNumber);
        }
        else {
            
            /*player.SendMessage("howMuchTime");*/
            SceneManager.LoadScene("End");

            
        }
    }

    public void scorePlus()
    {
        score = score * 250;
    }

    public void scoreCalculator()
    {
        score = score + levelTimeLeft * 100 ;
    }

}
