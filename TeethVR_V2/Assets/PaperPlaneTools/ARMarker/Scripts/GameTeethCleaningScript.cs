using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTeethCleaningScript : MonoBehaviour
{
    public Text actCounter;

    public GameController gameController;
    private Renderer rend;
    private Texture2D tex;
    private Texture2D originalTex;
    private Vector2 pixelUV;
    private Color[] colors;

    private Vector3 originPos;
    private bool cleaned = false;
    private int numCleaned = 0;
    private const int MAXCLEANING = 5;
    
    public void StartClean(){

        rend = GetComponent<Renderer>();

        originalTex = rend.material.mainTexture as Texture2D;

        tex = new Texture2D(originalTex.width, originalTex.height);
        tex.SetPixels(originalTex.GetPixels());
        rend.material.mainTexture = tex;
        tex.Apply(true);

        tex.SetPixels32(gameController.whiteArray);
        tex.Apply(true);
        rend.material.mainTexture = tex;
        
        GetComponent<MeshCollider>().enabled = false;
    }

    public void StartDirty() {

        rend = GetComponent<Renderer>();

        originalTex = rend.material.mainTexture as Texture2D;

        tex = new Texture2D(originalTex.width, originalTex.height);
        tex.SetPixels(originalTex.GetPixels());
        rend.material.mainTexture = tex;
        tex.Apply(true);       
    }

    private void OnTriggerEnter(Collider other) {
        if(!cleaned){
            originPos = other.transform.position;
            //actCounter.gameObject.transform.parent.gameObject.SetActive(true);
            //actCounter.text = this.name + "\n" + numCleaned + "/" + MAXCLEANING;
        }
    }

    private void OnTriggerStay(Collider other) {
        if(!cleaned){
            if(Vector3.Distance(other.transform.position, originPos) > 0.05f){

                numCleaned++;
                //Update counter
                //actCounter.text = this.name + "\n" + numCleaned + "/" + MAXCLEANING;

                if(numCleaned != MAXCLEANING){
                    //Change color
                    switch(numCleaned){
                        case 1: tex.SetPixels32(gameController.firstColorArray); break;
                        case 2: tex.SetPixels32(gameController.secondColorArray); break;
                        case 3: tex.SetPixels32(gameController.thirdColorArray); break;
                        case 4: tex.SetPixels32(gameController.fourthColorArray); break;
                    }
                    tex.Apply(true);
                    rend.material.mainTexture = tex;
                }else{
                    //Change color
                    tex.SetPixels32(gameController.whiteArray);
                    tex.Apply(true);
                    rend.material.mainTexture = tex;
                    //End cleaning for this tooth
                    cleaned = true;
                    //gameController.AddCleaned();
                }
            }
        }
    }

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
