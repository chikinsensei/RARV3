using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public class Timer : MonoBehaviour
{
    public float timeRemaining10 = 10;
    public float timeRemaining3 = 180;
    public bool timer10IsRunning = false;
    public bool timer3IsRunning = false;
    public bool gameIsEnded = false;
    public Text timeText;
    private void Start()
    {
        // Starts the timer automatically
        timer10IsRunning = true;
        timer3IsRunning = false;
    }
    void Update()
    {
        if (timer10IsRunning)
        {
            if (timeRemaining10 > 0)
            {
                timeRemaining10 -= Time.deltaTime;
                DisplayTime(timeRemaining10);
            }
            else
            {
                Debug.Log("Recherche lancée !");
                timeRemaining10 = 0;
                timer10IsRunning = false;
                DisplayTime(timeRemaining3);
                timer3IsRunning = true;
            }
        }
        if (timer3IsRunning)
        {
            if (timeRemaining3 > 0)
            {
                timeRemaining3 -= Time.deltaTime;
                DisplayTime(timeRemaining3);
            }
            else
            {
                Debug.Log("Fin de la partie !");
                timeRemaining3 = 0;
                timer3IsRunning = false;
                gameIsEnded = true;
            }
        }
        if (gameIsEnded)
        {
            LaunchLobby();
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void LaunchLobby()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}