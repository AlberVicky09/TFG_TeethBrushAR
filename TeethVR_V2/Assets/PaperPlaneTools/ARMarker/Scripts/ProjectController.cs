using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProjectController : MonoBehaviour
{
    public GameObject pauseCanvas;
    public Text timer;
    public List<GameObject> teethList;
    public PaperPlaneTools.AR.MyMainScript mainScript;

    private float actTime = 120.0f;
    private int actState = 0;
    private int seconds;
    private int minutes;

    private int r;
    public bool gamePaused = false;

    public Color32[] whiteArray, greenArray, firstColorArray, secondColorArray, thirdColorArray, fourthColorArray;
    public Color32 originalTeethColor, firstColor, secondColor, thirdColor, fourthColor;

    // Start is called before the first frame update
    void Start()
    {
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
        for(int i = 0; i < size; i++){
            whiteArray[i] = Color.white;
            greenArray[i] = Color.green;
            firstColorArray[i] = firstColor;
            secondColorArray[i] = secondColor;
            thirdColorArray[i] = thirdColor;
            fourthColorArray[i] = fourthColor;
        }

        //Start quarter as green
        for(int i = 0; i < 8; i++)
            teethList[i].GetComponent<ProjectTeethCleaningScript>().StartGreen();

        //Start the rest as clean
        for(int i = 8; i < teethList.Count; i++)
            teethList[i].GetComponent<ProjectTeethCleaningScript>().StartClean();
        
    }

    // Update is called once per frame
    void Update()
    {
        actTime -= Time.deltaTime;

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

        //Change green quarter
        if(seconds / 30 == 0){
            if(actState == 0 && actTime > 89.5f && actTime < 90.5f){
                for(int i = 0; i < 8; i++)
                    teethList[i].GetComponent<ProjectTeethCleaningScript>().TurnClean();
                for(int i = 8; i < 15; i++)
                    teethList[i].GetComponent<ProjectTeethCleaningScript>().TurnGreen();
                for(int i = 15; i < teethList.Count; i++)
                    teethList[i].GetComponent<ProjectTeethCleaningScript>().TurnClean();
                actState++;
            }else if(actState == 1 && actTime > 59.5f && actTime < 60.5f){
                for(int i = 0; i < 15; i++)
                    teethList[i].GetComponent<ProjectTeethCleaningScript>().TurnClean();
                for(int i = 15; i < 23; i++)
                    teethList[i].GetComponent<ProjectTeethCleaningScript>().TurnGreen();
                for(int i = 23; i < teethList.Count; i++)
                    teethList[i].GetComponent<ProjectTeethCleaningScript>().TurnClean();
                actState++;
            }else if(actState == 2 && actTime > 29.5f && actTime < 30.5f){
                for(int i = 0; i < 23; i++)
                    teethList[i].GetComponent<ProjectTeethCleaningScript>().TurnClean();
                for(int i = 23; i < teethList.Count; i++)
                    teethList[i].GetComponent<ProjectTeethCleaningScript>().TurnGreen();
                actState++;
            }else if(actState == 3 && actTime < 0.5f){
                PlayerPrefs.SetInt("Project", 1);
                mainScript.TurnOff();
                SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)){
            if(!gamePaused)
                PauseGame();
            else
                ResumeGame();
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
