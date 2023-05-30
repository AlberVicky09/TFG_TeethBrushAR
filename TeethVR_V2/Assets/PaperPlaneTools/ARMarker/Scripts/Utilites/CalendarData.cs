using System;
using System.Collections.Generic;

[Serializable]
public class CalendarData{
    public List<DateTime> dates;
    public List<string> time;
    public List<float> percentage;

    public CalendarData()
    {
        dates = new List<DateTime>();
        time = new List<string>();
        percentage = new List<float>();
    }
}
