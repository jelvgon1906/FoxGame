using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public AudioSource audioPick;
    public void startGame()
    {
        audioPick.Play();
        SceneManager.LoadScene("Level 1");
    }
    public void controls()
    {
        audioPick.Play();
        SceneManager.LoadScene("controls");
    }
    public void menu()
    {
        audioPick.Play();
        SceneManager.LoadScene("Menu");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }
}
