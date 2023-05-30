using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void Awake()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

        if (!Permission.HasUserAuthorizedPermission("android.permission.CAMERA"))
        {
            Permission.RequestUserPermission("android.permission.CAMERA");
        }

        var notification = new AndroidNotification();
        notification.Title = "YOUR TEETH NEED YOU!";
        notification.Text = "Don´t forget to clean your teeth!";
        notification.FireTime = System.DateTime.Today.AddHours(15);
        notification.RepeatInterval = System.TimeSpan.FromHours(24);

        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }
    public void GoTo4Sections(){
        SceneManager.LoadScene("4 sections Minigame", LoadSceneMode.Single);
    }

    public void GoToAllYouCan(){
        SceneManager.LoadScene("Clean All You Can Minigame", LoadSceneMode.Single);
    }

    public void GoToFast(){
        SceneManager.LoadScene("Clean fast Minigame", LoadSceneMode.Single);
    }
    public void Start()
    {
        switch (PlayerPrefs.GetInt("HasPlayed"))
        {
            case 1:
                //Store time
                CalendarSaver.Save(PlayerPrefs.GetString("PlayerTime"));
                PlayerPrefs.SetInt("HasPlayed", 0);
                break;

            case 2:
                //Store percentage
                CalendarSaver.Save(PlayerPrefs.GetFloat("PlayerScore"));
                PlayerPrefs.SetInt("HasPlayed", 0);
                break;

            default:
                break;
        }
    }
}
