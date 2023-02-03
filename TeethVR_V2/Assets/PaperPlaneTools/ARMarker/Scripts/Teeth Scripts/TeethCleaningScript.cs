using UnityEngine;
using UnityEngine.UI;

public class TeethCleaningScript : MonoBehaviour
{
    public Text actCounter;

    public ParentController gameController;
    private Renderer rend;

    private Vector3 originPos;
    private bool cleaned = false;
    private int numCleaned = 0;
    private const int MAXCLEANING = 5;
    private const float TOOTHWIDTH = 0.05f;
    
    public void Awake() {
        rend = GetComponent<Renderer>();
    }

    //Turn tooth into clean (white) color
    public void TurnClean(){
        rend.material.color = gameController.whiteColor;
    }

    //Turn tooth into dirty color
    public void TurnDirty(){
        rend.material.color = gameController.dirtyColor;
    }

    private void OnTriggerEnter(Collider other) {
        if(!cleaned){
            originPos = other.transform.position;
            //actCounter.gameObject.transform.parent.gameObject.SetActive(true);
            //actCounter.text = this.name + "\n" + numCleaned + "/" + MAXCLEANING;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(!cleaned){
            if(Vector3.Distance(other.transform.position, originPos) >= TOOTHWIDTH){
                numCleaned++;
                if(numCleaned != MAXCLEANING){
                    //Change color
                    switch(numCleaned){
                        case 1: rend.material.color = gameController.firstColor; break;
                        case 2: rend.material.color = gameController.secondColor; break;
                        case 3: rend.material.color = gameController.thirdColor; break;
                        case 4: rend.material.color = gameController.fourthColor; break;
                    }
                }else{
                    //Change color
                    rend.material.color = gameController.whiteColor;
                    //End cleaning for this tooth
                    cleaned = true;
                    this.GetComponent<Collider>().enabled = false;
                    //gameController.AddCleaned();
                }
            }
        }
    }

    /*private void OnTriggerStay(Collider other) {
        if(!cleaned){
            if(Vector3.Distance(other.transform.position, originPos) > 0.05f){

                numCleaned++;
                //Update counter
                //actCounter.text = this.name + "\n" + numCleaned + "/" + MAXCLEANING;

                if(numCleaned != MAXCLEANING){
                    //Change color
                    switch(numCleaned){
                        case 1: rend.material.color = gameController.firstColor; break;
                        case 2: rend.material.color = gameController.secondColor; break;
                        case 3: rend.material.color = gameController.thirdColor; break;
                        case 4: rend.material.color = gameController.fourthColor; break;
                    }
                }else{
                    //Change color
                    rend.material.color = gameController.whiteColor;
                    //End cleaning for this tooth
                    cleaned = true;
                    this.GetComponent<Collider>().enabled = false;
                    //gameController.AddCleaned();
                }
            }
        }
    }*/

    /*private void OnCollisionEnter(Collision other) {

            RaycastHit hit = new RaycastHit();
            Ray ray = new Ray(other.GetContact(0).point - other.GetContact(0).normal, other.GetContact(0).normal);
            if(Physics.Raycast(ray, out hit)){
                Debug.Log(hit.textureCoord);
                pixelUV = hit.textureCoord;
                pixelUV.x *= tex.width;
                pixelUV.y *= tex.height;

                tex.SetPixels((int) pixelUV.x, (int) pixelUV.y, 100, 100, colors);
                tex.Apply(true);
                
                rend.material.mainTexture = tex;
            }
            Debug.Log(this.name);
    }*/
}
