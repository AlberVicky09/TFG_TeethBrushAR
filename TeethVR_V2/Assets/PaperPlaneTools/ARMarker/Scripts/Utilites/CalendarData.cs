using System;
using System.Collections.Generic;

[Serializable]
public class CalendarData{
   public List<DateTime> dates;

    public CalendarData()
    {
        dates = new List<DateTime>();
    }
}
