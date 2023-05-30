using System.Collections.Generic;
using UnityEngine;

public class Sections4GameController : ParentController
{
    //List of teeth in each quarter
    public List<List<GameObject>> mouthParts;

    //Number of teeth in each quarter (should be the same)
    private const int NUMTEETH1PART = 8;
    private const int NUMTEETH2PART = 7;
    private const int NUMTEETH3PART = 7;
    private const int NUMTEETH4PART = 8;

    //Adding of teeth in each part with the prev one
    private const int NUMTEETH12PARTS = NUMTEETH1PART + NUMTEETH2PART;
    private const int NUMTEETH123PARTS = NUMTEETH12PARTS + NUMTEETH3PART;

    private float activateChange = 0f;
    private bool quarterAlreadyChanged = true;
    private int currentQuarter = 0;
    int numCleaned = 0;

    // Start is called before the first frame update
    public void Start()
    {

        //Divide teeth in 4 lists
        mouthParts = new List<List<GameObject>>();
        for(int i = 0; i < 4; i++)
            mouthParts.Add(new List<GameObject>());
        for(int i = 0; i < teethList.Count; i++){
            if(i < NUMTEETH1PART){
                mouthParts[0].Add(teethList[i]);
            }else if(i < NUMTEETH12PARTS){
                mouthParts[1].Add(teethList[i]);
            }else if(i < NUMTEETH123PARTS){
                mouthParts[2].Add(teethList[i]);
            }else{
                mouthParts[3].Add(teethList[i]);
            }
        }

        //Make the first forth of the mouth dirty
        for(int i = 0; i < NUMTEETH1PART; i++){
            mouthParts[0][i].GetComponent<TeethCleaningScript>().TurnDirty();
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        //Add time
        actTime += Time.deltaTime;

        //Update timer
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

        //Check if quarter has already changed (avoid multiple changes in a second)
        if(!quarterAlreadyChanged){
            //Update quarter each 30 seconds
            if((seconds < 1) || (seconds > 29 && seconds < 31)){
                //Change quarter
                switch(currentQuarter){
                    case 0:
                        //Make the first forth of the mouth clean
                        for(int i = 0; i < NUMTEETH1PART; i++){
                            mouthParts[0][i].GetComponent<TeethCleaningScript>().TurnClean();
                        }
                        //Make the second forth of the mouth dirty
                        for(int i = 0; i < NUMTEETH2PART; i++){
                            mouthParts[1][i].GetComponent<TeethCleaningScript>().TurnDirty();
                        }
                    break;

                    case 1:
                        //Make the second forth of the mouth clean
                        for(int i = 0; i < NUMTEETH2PART; i++){
                            mouthParts[1][i].GetComponent<TeethCleaningScript>().TurnClean();
                        }
                        //Make the third forth of the mouth dirty
                        for(int i = 0; i < NUMTEETH3PART; i++){
                            mouthParts[2][i].GetComponent<TeethCleaningScript>().TurnDirty();
                        }
                    break;

                    case 2:
                        //Make the third forth of the mouth clean
                        for(int i = 0; i < NUMTEETH3PART; i++){
                            mouthParts[2][i].GetComponent<TeethCleaningScript>().TurnClean();
                        }
                        //Make the last part of the mouth dirty
                        for(int i = 0; i < NUMTEETH4PART; i++){
                            mouthParts[3][i].GetComponent<TeethCleaningScript>().TurnDirty();
                        }
                    break;

                    case 3:
                        //End the game
                        EndGame(false, numCleaned);
                    break;
                }
                quarterAlreadyChanged = true;
                activateChange = 0f;
                currentQuarter++;
            }
        }else{
            //Dont check changes until 20 seconds have passed
            activateChange += Time.deltaTime;
            if(activateChange > 20.0f)
                quarterAlreadyChanged = false;
        }
    }
}
