using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState State;

    private void Awake(){
        //DontDestroyOnLoad(this.gameObject);

        if(instance == null){
            instance = this;
        }else{
            Destroy(this.gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start(){
        if(SceneManager.GetSceneByName("Game").isLoaded){
            SwitchState(GameState.Playing);
        }else if(SceneManager.GetSceneByName("MainMenu").isLoaded){
            SwitchState(GameState.MainMenu);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        if(SceneManager.GetSceneByName("Game").isLoaded){
            SwitchState(GameState.Playing);
        }else if(SceneManager.GetSceneByName("MainMenu").isLoaded){
            SwitchState(GameState.MainMenu);
        }
    }

    //Handlers

    private void SwitchState(GameState state){
        switch(state){
            case GameState.Playing:
                HandlePlayingState();
                break;
            case GameState.Paused:
                HandlePausedState();
                break;
            case GameState.MainMenu:
                HandleMainMenuState();
                break;
            case GameState.Lost:
                HandleLostState();
                break;
            case GameState.Default:
                HandleDefaultState();
                break;
        }
    }

    private void HandlePlayingState(){
        State =  GameState.Playing;
        MenuManager.instance.ShowGameScreen();
        Time.timeScale = 1f;
    }

    private void HandlePausedState(){
        State =  GameState.Paused;
        MenuManager.instance.ShowPauseScreen();
        Time.timeScale = 1f;
    }

    private void HandleMainMenuState(){
        State =  GameState.MainMenu;
        Time.timeScale = 1f;
    }

    private void HandleLostState(){
        State =  GameState.Lost;
        MenuManager.instance.ShowLoseScreen();
        Time.timeScale = 1f;
    }

    private void HandleDefaultState(){
        State =  GameState.Default;
        //Why on earth are we in here...
        Debug.Log("How did you manage this...");
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //Public Functions
    public void Lose(){
        SwitchState(GameState.Lost);
    }

    public void Pause(){
        if(State == GameState.Paused) SwitchState(GameState.Playing);
        else if(State == GameState.Playing) SwitchState(GameState.Paused);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------

    //Scene Management

    public void LoadGameScene(){
        StartCoroutine(ChangeScene("Game"));
        //SwitchState(GameState.Playing);
    }

    public void LoadMainMenuScene(){
        StartCoroutine(ChangeScene("MainMenu"));
        //SwitchState(GameState.MainMenu);
    }

    public void LoadTutorialScene(){
        StartCoroutine(ChangeScene("Tutorial"));
    }

    IEnumerator ChangeScene(string name){
        AudioManager.instance.Play("Menu Click");
        yield return new WaitForSeconds(0.1f);
        Debug.Log("ChangingScene");
        SceneManager.LoadScene(name);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------------

}

public enum GameState{
    Default,
    Playing,
    Paused,
    MainMenu,
    Lost
}
