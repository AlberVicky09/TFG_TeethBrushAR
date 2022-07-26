using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject pauseCanvas;
    public Text timer;
    public Text teethCounter;
    public List<GameObject> teethList;
    private bool[] initiated;
    public PaperPlaneTools.AR.MyMainScript mainScript;

    private float actTime = 0f;
    private int seconds;
    private int minutes;

    private const int NUMDIRTY = 3;
    private int numCleaned = 0;
    private int r;
    public bool gamePaused = false;

    public Color32[] whiteArray, greenArray, firstColorArray, secondColorArray, thirdColorArray, fourthColorArray;
    public Color32 originalTeethColor, firstColor, secondColor, thirdColor, fourthColor;

    private void Start() {

        //Get size of textures
        int size = teethList[0].GetComponent<Renderer>().material.mainTexture.width * teethList[0].GetComponent<Renderer>().material.mainTexture.height;
        whiteArray = new Color32[size];
        greenArray = new Color32[size];
        firstColorArray = new Color32[size];
        secondColorArray = new Color32[size];
        thirdColorArray = new Color32[size];
        fourthColorArray = new Color32[size];

        //Get initial color
        originalTeethColor = ((Texture2D) teethList[0].GetComponent<Renderer>().material.mainTexture).GetPixel(0,0);
        firstColor = new Color(originalTeethColor.r * 1.25f, originalTeethColor.g * 1.25f, originalTeethColor.b * 1.25f, 1);
        secondColor = new Color(firstColor.r * 1.25f, firstColor.g * 1.25f, firstColor.b * 1.25f, 1);
        thirdColor = new Color(secondColor.r * 1.25f, secondColor.g * 1.25f, secondColor.b * 1.25f, 1);
        fourthColor = new Color(thirdColor.r * 1.25f, thirdColor.g * 1.25f, thirdColor.b * 1.25f, 1);

        //Initialize arrays
        initiated = new bool[teethList.Count];
        for(int i = 0; i < teethList.Count; i++)
            initiated[i] = false;

        for(int i = 0; i < size; i++){
            whiteArray[i] = Color.white;
            greenArray[i] = Color.green;
            firstColorArray[i] = firstColor;
            secondColorArray[i] = secondColor;
            thirdColorArray[i] = thirdColor;
            fourthColorArray[i] = fourthColor;
        }

        //Start NUMDIRTY teeth as dirty
        for(int i = 0; i < NUMDIRTY; i++){
            r = Random.Range(0, teethList.Count);
            teethList[r].GetComponent<GameTeethCleaningScript>().StartDirty();
            initiated[r] = true;
        }

        //Start the rest clean
        for(int i = 0; i < teethList.Count; i++){
            if(!initiated[i])
                teethList[i].GetComponent<GameTeethCleaningScript>().StartClean();
        }

        teethCounter.text = "Cleaned counter\n" + numCleaned + "/" + NUMDIRTY;

        //Test to check if score works
        /*
        actTime = Random.Range(90.0f, 180.0f);
        minutes = (int) actTime / 60;
        seconds = (int) actTime % 60;
        string minutesString;
        string secondsString;

        if(minutes < 10 && seconds < 10){
            minutesString = "0" + minutes;
            secondsString =  "0" + seconds;
        }else if(minutes < 10){
            minutesString = "0" + minutes;
            secondsString =  "" + seconds;
        }else if(seconds < 10){
            minutesString = "" + minutes;
            secondsString =  "0" + seconds;
        }else{
            minutesString = "" + minutes;
            secondsString =  "" + seconds;
        }

        PlayerPrefs.SetString("PlayerTime", minutesString + ":" + secondsString);
        mainScript.TurnOff();
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
        */
    }

    private void Update() {

        actTime += Time.deltaTime;

        minutes = (int) actTime / 60;
        seconds = (int) actTime % 60;
        if(minutes < 10 && seconds < 10)
            timer.text = "0" + minutes + ":0" + seconds;
        else if(minutes < 10)
            timer.text = "0" + minutes + ":" + seconds;
        else if(seconds < 10)
            timer.text = minutes + ":0" + seconds;
        else
            timer.text = minutes + ":" + seconds;        

        if (Input.GetKeyDown(KeyCode.Escape)){
            if(!gamePaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    public void AddCleaned(){

        numCleaned++;
        teethCounter.text = "Cleaned counter\n" + numCleaned + "/" + NUMDIRTY;

        if(numCleaned == NUMDIRTY){

            minutes = (int) actTime / 60;
            seconds = (int) actTime % 60;
            string minutesString;
            string secondsString;

            if(minutes < 10 && seconds < 10){
                minutesString = "0" + minutes;
                secondsString =  "0" + seconds;
            }else if(minutes < 10){
                minutesString = "0" + minutes;
                secondsString =  "" + seconds;
            }else if(seconds < 10){
                minutesString = "" + minutes;
                secondsString =  "0" + seconds;
            }else{
                minutesString = "" + minutes;
                secondsString =  "" + seconds;
            }

            PlayerPrefs.SetString("PlayerTime", minutesString + ":" + secondsString);
            mainScript.TurnOff();
            SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
        }
    }

    public void PauseGame(){
        gamePaused = true;
        Time.timeScale = 0f;
        pauseCanvas.gameObject.SetActive(true);
    }

    public void ResumeGame(){
        gamePaused = false;
        Time.timeScale = 1f;
        pauseCanvas.gameObject.SetActive(false);
    }

    public void BackToMenu(){
        mainScript.TurnOff();
        PlayerPrefs.SetInt("HasPlayed", 1);
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
    }
}
