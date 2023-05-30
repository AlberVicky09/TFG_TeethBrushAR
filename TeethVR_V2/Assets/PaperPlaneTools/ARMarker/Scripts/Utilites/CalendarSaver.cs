using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class CalendarSaver : MonoBehaviour
{
    public static CalendarData saveData;

    private void Awake()
    {
        System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        saveData = new CalendarData();
        saveData.dates.Add(new System.DateTime(2023, 2, 10));
        DontDestroyOnLoad(this);
    }

    public static void Save(string timeTaken) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/CalendarData.dat");

        saveData.dates.Add(System.DateTime.Now);
        saveData.time.Add(timeTaken);
        saveData.percentage.Add(-1f);

        bf.Serialize(file, saveData);
        file.Close();
    }

    public static void Save(float percentage)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/CalendarData.dat");

        saveData.dates.Add(System.DateTime.Now);
        saveData.time.Add("");
        saveData.percentage.Add(percentage);

        bf.Serialize(file, saveData);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/CalendarData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + "/CalendarData.dat", FileMode.Open);
            saveData = bf.Deserialize(fs) as CalendarData;
            fs.Close();
        } 
    }
}
