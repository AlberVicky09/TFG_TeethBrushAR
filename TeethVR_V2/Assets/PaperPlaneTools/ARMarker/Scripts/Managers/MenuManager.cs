using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void GoTo4Sections(){
        SceneManager.LoadScene("4 sections Minigame", LoadSceneMode.Single);
    }

    public void GoToAllYouCan(){
        SceneManager.LoadScene("Clean All You Can Minigame", LoadSceneMode.Single);
    }

    public void GoToFast(){
        SceneManager.LoadScene("Clean fast Minigame", LoadSceneMode.Single);
    }
}
