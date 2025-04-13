using System;
using System.Collections.Generic;
using System.IO;

namespace Sim_Thing_Code
{
    class RaceAdmin
    {
        public static List<Entrant> LoadEntryList(string selectedSeries)
        {
            List<Entrant> returnList = new List<Entrant>();
            List<int> spacerList = new List<int>() { 0, 0, 0, 0, 0, 0 };

            Entrant newEntrant;

            string entryListFile = CommonData.GetEntryListFile(selectedSeries);

            string[] entryListData = File.ReadAllLines(entryListFile);

            for (int i = 1; i < entryListData.Length; i++)
            {
                newEntrant = new Entrant(entryListData[i].Split(','), selectedSeries, i);

                switch (selectedSeries)
                {
                    case "Main Series":
                        UpdateMainComponentSpacerList(newEntrant, spacerList);
                        break;
                    case "Junior Series":
                        UpdateJuniorComponentSpacerList(newEntrant, spacerList);
                        break;
                }
                
                returnList.Add(newEntrant);
            }

            switch (selectedSeries)
            {
                case "Main Series":
                    CommonData.SetMainSeriesSpacers(spacerList);
                    break;
                case "Junior Series":
                    CommonData.SetJuniorSeriesSpacers(spacerList);
                    break;
            }

            return returnList;
        }

        public static void LoadCalendar(string selectedSeries)
        {
            string calendarFile = CommonData.GetCalendarFile(selectedSeries);
            List<int> spacerList = new List<int>() { 0, 0, 0 };

            List<Round> newCalendar = new List<Round>();

            Round newRound;

            string[] calendarData = File.ReadAllLines(calendarFile);

            for (int i = 1; i < calendarData.Length; i++)
            {
                newRound = new Round(calendarData[i].Split(','), i, selectedSeries);

                UpdateCalendarSpacerList(newRound, spacerList);

                newCalendar.Add(newRound);
            }

            switch (selectedSeries)
            {
                case "Main Series":
                    CommonData.SetMainCalendarSpacerList(spacerList);
                    break;
                case "Junior Series":
                    CommonData.SetJuniorCalendarSpacerList(spacerList);
                    break;
            }

            if (selectedSeries == "Main Series")
            {
                CommonData.SetMainSeriesCalendar(newCalendar);
            }
            else
            {
                CommonData.SetJuniorSeriesCalendar(newCalendar);
            }
        }

        private static void UpdateMainComponentSpacerList(Entrant currentEntrant, List<int> spacerList)
        {
            List<int> entrantLengths = new List<int>();

            entrantLengths.Add(currentEntrant.GetDriverName().Length);
            entrantLengths.Add(currentEntrant.GetCarNumber().Length);
            entrantLengths.Add(currentEntrant.GetTeamName().Length);

            entrantLengths.Add(currentEntrant.GetChassis().Length);
            entrantLengths.Add(currentEntrant.GetEngine().Length);
            entrantLengths.Add(currentEntrant.GetTyres().Length);

            for (int i = 0; i < entrantLengths.Count; i++)
            {
                if (entrantLengths[i] > spacerList[i])
                {
                    spacerList[i] = entrantLengths[i];
                }
            }
        }

        private static void UpdateJuniorComponentSpacerList(Entrant currentEntrant, List<int> spacerList)
        {
            List<int> entrantLengths = new List<int>();

            entrantLengths.Add(currentEntrant.GetDriverName().Length);
            entrantLengths.Add(currentEntrant.GetCarNumber().Length);
            entrantLengths.Add(currentEntrant.GetTeamName().Length);

            entrantLengths.Add(currentEntrant.GetEngine().Length);

            for (int i = 0; i < entrantLengths.Count; i++)
            {
                if (entrantLengths[i] > spacerList[i])
                {
                    spacerList[i] = entrantLengths[i];
                }
            }
        }

        private static void UpdateCalendarSpacerList(Round currentRound, List<int> spacerList)
        {
            List<int> entrantLengths = new List<int>();

            entrantLengths.Add(currentRound.GetRoundTitle().Length);
            entrantLengths.Add(currentRound.GetTrackName().Length);
            entrantLengths.Add(currentRound.GetCountryName().Length);

            for (int i = 0; i < entrantLengths.Count; i++)
            {
                if (entrantLengths[i] > spacerList[i])
                {
                    spacerList[i] = entrantLengths[i];
                }
            }
        }
    }
}
