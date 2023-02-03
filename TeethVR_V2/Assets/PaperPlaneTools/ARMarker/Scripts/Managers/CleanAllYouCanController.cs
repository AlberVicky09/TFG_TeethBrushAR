using System.Collections.Generic;
using UnityEngine;

public class CleanAllYouCanController : ParentController
{
    private int numCleaned;

    // Start is called before the first frame update
    public void Start()
    {
        //Make a random teeth dirty
        teethList[Random.Range(0, teethList.Count)].GetComponent<TeethCleaningScript>().TurnDirty();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        //Check if time has already turn out
    }
}
