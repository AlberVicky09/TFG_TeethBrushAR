using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class ParentController : MonoBehaviour
{
    public GameObject pauseCanvas;
    public Text timer;
    public Text teethCounter;
    public PaperPlaneTools.AR.MyMainScript mainScript;

    public List<GameObject> teethList;
    public Color32 whiteColor, dirtyColor, firstColor, secondColor, thirdColor, fourthColor;

    protected float actTime = 0f;
    protected int seconds;
    protected int minutes;

    public static bool gamePaused = false;

    public virtual void Update() {

        //Check pause
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause(){
        //Toggle time and pause canvas
        gamePaused = !gamePaused;
        Time.timeScale = gamePaused ? 0f : 1f;
        pauseCanvas.gameObject.SetActive(gamePaused);
    }

    public void EndGame(bool storeTime, int numCleaned){
        //Turn off camera
        mainScript.TurnOff();

        //Store time
        if(storeTime){
            if(minutes < 10 && seconds < 10){
                PlayerPrefs.SetString("PlayerTime", "0" + minutes + ":0" + seconds);
            }else if(minutes < 10){
                PlayerPrefs.SetString("PlayerTime", "0" + minutes + ":" + seconds);
            }else if(seconds < 10){
                PlayerPrefs.SetString("PlayerTime", minutes + ":0" + seconds);
            }else{
                PlayerPrefs.SetString("PlayerTime", minutes + ":" + seconds);
            }
            PlayerPrefs.SetInt("HasPlayed", 1);
        }
        else
        {
            PlayerPrefs.SetFloat("PlayerScore", numCleaned / 30);
            PlayerPrefs.SetInt("HasPlayed", 2);
        }
        
        //Return to main menu
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
}
