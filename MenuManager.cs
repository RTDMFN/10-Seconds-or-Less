using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    public float score;

    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI nextOrderInText;
    public TMPro.TextMeshProUGUI decayInText;

    public GameObject gameScreen;
    public GameObject loseScreen;
    public GameObject pauseScreen;

    private void Awake(){
        //DontDestroyOnLoad(this.gameObject);

        if(instance == null){
            instance = this;
        }else{
            Destroy(this.gameObject);
        }
    }

    private void HideAllScreens(){
        gameScreen.SetActive(true);
        loseScreen.SetActive(true);
        pauseScreen.SetActive(true);
        gameScreen.transform.localScale = Vector3.zero;
        loseScreen.transform.localScale = Vector3.zero;
        pauseScreen.transform.localScale = Vector3.zero;
    }

    private void ShowScreen(GameObject screenToShow){
        HideAllScreens();
        screenToShow.transform.localScale = Vector3.one;
    }

    public void ShowPauseScreen(){
        ShowScreen(pauseScreen);
    }

    public void ShowGameScreen(){
        ShowScreen(gameScreen);
    }

    public void ShowLoseScreen(){
        ShowScreen(loseScreen);
    }

    public void UpdateScoreText(){
        score += 100;
        scoreText.text = "$" + score.ToString();
    }

    public void UpdateNextOrderInText(float timeRemaining){
        nextOrderInText.text = "Next Order in " + timeRemaining.ToString("F2") + "s";
    }

    public void UpdateDecayInText(float timeRemaining){
        decayInText.text = "Pizza Decay in " + timeRemaining.ToString("F2") + "s";
    }
}
