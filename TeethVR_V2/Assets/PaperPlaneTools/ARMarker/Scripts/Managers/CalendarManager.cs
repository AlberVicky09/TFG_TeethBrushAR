using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class CalendarManager : MonoBehaviour
{
    public GameObject monthAndYear;
    public CalendarWeek[] calendarDays;
    private DateTime auxDateTime;
    private int auxDateDifference;
    private List<DateTime> auxCalendarData;
    private Color fillColor = new Color(0.1f, 0.9f, 0.8f);
    private int auxYear = DateTime.Today.Year;
    private int auxMonth = DateTime.Today.Month;
    private int currentDayOfWeek;
    private int currentWeek;
    private int auxDateCounter;
    private int auxDaysInMonth;
    
    public void UpdateCalendar()
    {
        //Set month and year
        monthAndYear.GetComponentInChildren<Text>().text = new DateTime(auxYear, auxMonth, 1).ToString("MMMM yyyy", CultureInfo.InvariantCulture);

        //Re enable all possible days
        for(int i = 0; i < 7; i++)
        {
            calendarDays[0].daysOfWeek[i].SetActive(true);
            calendarDays[4].daysOfWeek[i].SetActive(true);
            calendarDays[5].daysOfWeek[i].SetActive(true);
        }

        //Repaint days background
        auxDaysInMonth = DateTime.DaysInMonth(auxYear, auxMonth);
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                calendarDays[i].daysOfWeek[j].GetComponent<Image>().color = Color.white;
            }
        }

        //Set current day of week
        currentDayOfWeek = (int) new DateTime(auxYear, auxMonth, 1).DayOfWeek;
        if(currentDayOfWeek == 0)
        {
            currentDayOfWeek = 7;
        }
        currentWeek = 0;

        //Dissable days of prev month
        for(int i = 0; i < currentDayOfWeek - 1; i++)
        {
            calendarDays[currentWeek].daysOfWeek[i].SetActive(false);
        }

        //Set days number and mark the ones that have been done
        auxCalendarData = CalendarSaver.saveData.dates;
        auxDateCounter = 0;
        for(int i = 1; i <= auxDaysInMonth; i++){
            calendarDays[currentWeek].daysOfWeek[currentDayOfWeek-1].GetComponentInChildren<Text>().text = i.ToString();
            
            for(int j = auxDateCounter; j < auxCalendarData.Count; j++) {

                auxDateDifference = auxCalendarData[j].Subtract(new DateTime(auxYear, auxMonth, i).Date).Days;
                if (auxDateDifference == 0)
                {
                    calendarDays[currentWeek].daysOfWeek[currentDayOfWeek - 1].GetComponent<Image>().color = fillColor;
                    break;
                }else if (auxDateDifference < 0) {
                    auxDateCounter++;
                }
                else {
                    break;
                }
            }   
            
            if(currentDayOfWeek == 7){
                currentDayOfWeek = 1;
                currentWeek++;
            }else {
                currentDayOfWeek++;
            }
        }

        //Disable remaining calendar days
        while(currentWeek < 6)
        {
            calendarDays[currentWeek].daysOfWeek[currentDayOfWeek-1].SetActive(false);
            if (currentDayOfWeek == 7) {
                currentDayOfWeek = 1;
                currentWeek++;
            }else {
                currentDayOfWeek++;
            }
        }
    }

    public void UpdateMonth(int direction)
    {
        if(direction > 0){
            if (auxMonth == 12){
                auxYear++;
                auxMonth = 1;
            }else{
                auxMonth++;
            }
        }
        else{
            if(auxMonth == 1)
            {
                auxYear--;
                auxMonth = 12;
            }
            else { 
                auxMonth--;
            }
        }

        UpdateCalendar();
    }

    public void UpdateYear(int direction){
        auxYear += direction;
        UpdateCalendar();
    }

    public void RestartCalendar()
    {
        auxYear = DateTime.Today.Year;
        auxMonth = DateTime.Today.Month;
    }

    [System.Serializable]
    public class CalendarWeek
    {
        [SerializeField]
        public GameObject[] daysOfWeek;
    }
}
