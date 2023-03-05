using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievements : MonoBehaviour
{
    public Text achievementTitleText;
    public Text achievementDescriptionText;
    private List<string> achievementTitles;
    private List<string> achievementDescriptions;

    private void Awake()
    {
        achievementTitles = new List<string>();
        achievementTitles.Add("More to come..."); 
        achievementTitles.Add("Beginners guide"); 
        achievementTitles.Add("3 in a row");
        achievementTitles.Add("5 streak");
        achievementTitles.Add("Weekly cleaner");
        achievementTitles.Add("Monthly wash!");

        achievementDescriptions = new List<string>();
        achievementDescriptions.Add("More achievements will come in the future... Keep an eye!");
        achievementDescriptions.Add("Play for the first time");
        achievementDescriptions.Add("Play for 3 days consecutively");
        achievementDescriptions.Add("Play for 5 days consecutively");
        achievementDescriptions.Add("Play for 7 days consecutively");
        achievementDescriptions.Add("Play for 30 days consecutively");

        achievementTitleText.text = achievementTitles[0];
        achievementDescriptionText.text = achievementDescriptions[0];
    }

    public void SelectAchievement(int index)
    {
        achievementTitleText.text = achievementTitles[index];
        achievementDescriptionText.text = achievementDescriptions[index];
    }

}
