using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;


public class End : MonoBehaviour
{
    public TextMeshProUGUI txtScore;
    int finalScore;
    void Start()
    {
        GameManager.instance.scoreCalculator();
        finalScore = GameManager.instance.score;
        txtScore.text = finalScore + " ptos!!!";
    }
}
