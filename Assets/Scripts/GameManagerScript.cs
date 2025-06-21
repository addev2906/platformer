using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    public GameObject doorOpen;
    public GameObject doorClose;
    [SerializeField] private GameObject loseText;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject retryButton;
    [SerializeField] private GameObject levelSelectButton;


    public void Win() {winText.SetActive(true); levelSelectButton.SetActive(true); }

    public void Lose() {loseText.SetActive(true) ; retryButton.SetActive(true); }

    public void Level1(){SceneManager.LoadScene("Level1");}
    public void Level2() { SceneManager.LoadScene("Level2"); }
    public void Level3() { SceneManager.LoadScene("Level3"); }

    public void StartLevel(){SceneManager.LoadScene("LevelSelect");}
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
