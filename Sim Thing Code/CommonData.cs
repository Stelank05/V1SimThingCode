using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sim_Thing_Code
{
    public static class CommonData
    {
        static string rootFolder;
        static string setupFolder, saveFolder;
        static string mainSeriesName, juniorSeriesName;

        static List<int> mainSeriesSpacers, juniorSeriesSpacers;
        static List<int> mainCalendarSpacers, juniorCalendarSpacers;

        static List<Round> mainSeriesCalendar, juniorSeriesCalendar;

        public static void SetRootFolder(string newRootFolder)
        {
            rootFolder = newRootFolder;
        }

        public static void SetSetupFolder(string newSetupFolder)
        {
            setupFolder = newSetupFolder;
        }

        public static string GetSetupFolder()
        {
            return Path.Combine(rootFolder, setupFolder);
        }

        public static void SetSaveFolder(string newSaveFolder)
        {
            saveFolder = newSaveFolder;
        }

        public static void SetMainSeriesName(string newMainSeriesName)
        {
            mainSeriesName = newMainSeriesName;
        }

        public static string GetMainSeriesName()
        {
            return mainSeriesName;
        }

        public static void SetJuniorSeriesName(string newJuniorSeriesName)
        {
            juniorSeriesName = newJuniorSeriesName;
        }

        public static string GetJuniorSeriesName()
        {
            return juniorSeriesName;
        }

        public static string GetEntryListFile(string selectedSeries)
        {
            string series;

            if (selectedSeries == "Main Series")
            {
                series = mainSeriesName;
            }
            else
            {
                series = juniorSeriesName;
            }

            return Path.Combine(rootFolder, setupFolder, series + " Entry List.csv");
        }

        public static string GetCalendarFile(string selectedSeries)
        {
            string series;

            if (selectedSeries == "Main Series")
            {
                series = mainSeriesName;
            }
            else
            {
                series = juniorSeriesName;
            }

            return Path.Combine(rootFolder, setupFolder, series + " Calendar.csv");
        }

        public static string GetMainSeriesResultsFolder()
        {
            return Path.Combine(rootFolder, saveFolder, mainSeriesName);
        }

        public static string GetJuniorSeriesResultsFolder()
        {
            return Path.Combine(rootFolder, saveFolder, juniorSeriesName);
        }

        public static void SetMainSeriesSpacers(List<int> newSpacerList)
        {
            mainSeriesSpacers = newSpacerList;
        }

        public static List<int> GetMainSeriesSpacers()
        {
            return mainSeriesSpacers;
        }

        public static void SetJuniorSeriesSpacers(List<int> newSpacerList)
        {
            juniorSeriesSpacers = newSpacerList;
        }

        public static List<int> GetJuniorSeriesSpacers()
        {
            return juniorSeriesSpacers;
        }

        public static void SetMainCalendarSpacerList(List<int> newSpacerList)
        {
            mainCalendarSpacers = newSpacerList;
        }

        public static List<int> GetMainCalendarSpacerList()
        {
            return mainCalendarSpacers;
        }

        public static void SetJuniorCalendarSpacerList(List<int> newSpacerList)
        {
            juniorCalendarSpacers = newSpacerList;
        }

        public static List<int> GetJuniorCalendarSpacerList()
        {
            return juniorCalendarSpacers;
        }

        public static void SetMainSeriesCalendar(List<Round> newMainSeriesCalendar)
        {
            mainSeriesCalendar = newMainSeriesCalendar;
        }

        public static List<Round> GetMainSeriesCalendar()
        {
            return mainSeriesCalendar;
        }

        public static void SetJuniorSeriesCalendar(List<Round> newJuniorSeriesCalendar)
        {
            juniorSeriesCalendar = newJuniorSeriesCalendar;
        }

        public static List<Round> GetJuniorSeriesCalendar()
        {
            return juniorSeriesCalendar;
        }
    }
}
