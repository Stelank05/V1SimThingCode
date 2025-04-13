using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim_Thing_Code
{
    public enum RunningState
    {
        RUNNING = 1,
        RETIRED = 2,
        NOT_STARTING = 3,
        DISQUALIFIED = 4,
        WITHDRAWN = 5
    }

    public class Entrant
    {
        string driverName, carNumber, teamName, enteredSeries;
        int entrantIndex;

        string chassisLong, engineLong, tyresLong;
        char chassisShort, engineShort, tyresShort;

        int currentOVR, baseOVR, reliabilityScore;

        int qualifyingRunningSlot, race1GridSpot, race2GridSpot;
        string nonCompeteReason;

        RunningState runningState = RunningState.RUNNING;

        public Entrant(string[] entrantData, string enteredSeries, int index)
        {
            driverName = entrantData[0];
            carNumber = entrantData[1];
            teamName = entrantData[2];

            this.enteredSeries = enteredSeries;
            entrantIndex = index;

            chassisLong = entrantData[3];
            engineLong = entrantData[4];
            tyresLong = entrantData[5];

            chassisShort = chassisLong[0];
            engineShort = engineLong[0];
            tyresShort = tyresLong[0];

            currentOVR = Convert.ToInt32(entrantData[6]);
            baseOVR = Convert.ToInt32(entrantData[6]);
            reliabilityScore = Convert.ToInt32(entrantData[7]);
        }

        public string GetDriverName()
        {
            return driverName;
        }

        public string GetCarNumber()
        {
            return carNumber;
        }

        public string GetTeamName()
        {
            return teamName;
        }

        public string GetChassis()
        {
            return chassisLong;
        }

        public string GetEngine()
        {
            return engineLong;
        }

        public string GetTyres()
        {
            return tyresLong;
        }

        public int GetEntrantIndex()
        {
            return entrantIndex;
        }

        public void UpdateOVR(int newOVR)
        {
            currentOVR = newOVR;
        }

        public void ResetOVR()
        {
            currentOVR = baseOVR;
        }

        public int GetOVR()
        {
            return currentOVR;
        }

        public int GetBaseOVR()
        {
            return baseOVR;
        }

        public int GetReliabilityScore()
        {
            return reliabilityScore;
        }

        public void SetNonCompeteReason(string reason)
        {
            nonCompeteReason = reason;
        }

        public string GetNonCompeteReason()
        {
            return nonCompeteReason;
        }

        public void SetQualifyingSpot(int newPosition)
        {
            qualifyingRunningSlot = newPosition;
        }

        public int GetQualifyingSpot()
        {
            return qualifyingRunningSlot;
        }

        public void SetRace1GridSpot(int newGridSpot)
        {
            race1GridSpot = newGridSpot;
        }

        public int GetRace1GridSpot()
        {
            return race1GridSpot;
        }

        public void SetRace2GridSpot(int newGridSpot)
        {
            race2GridSpot = newGridSpot;
        }

        public int GetRace2GridSpot()
        {
            return race2GridSpot;
        }

        public void SetRunningState(RunningState newRunningState)
        {
            runningState = newRunningState;
        }

        public RunningState GetRunningState()
        {
            return runningState;
        }

        public string GetEntrantAsOutputString(int componentDisplayType)
        {
            string components = "";
            string displayOVR;

            List<int> outputSpacers = new List<int>();

            switch (enteredSeries)
            {
                case "Main Series":
                    outputSpacers = CommonData.GetMainSeriesSpacers();
                    switch (componentDisplayType)
                    {
                        case 1:
                            components = String.Format("{0} - {1} - {2}", chassisLong.PadRight(outputSpacers[3], ' '), engineLong.PadRight(outputSpacers[4], ' '), tyresLong.PadRight(outputSpacers[5], ' ')); // Needs Spacering
                            break;

                        case 2:
                            components = String.Format("{0} - {1} - {2}", chassisShort, engineShort, tyresShort);
                            break;
                    }
                    break;
                case "Junior Series":
                    outputSpacers = CommonData.GetJuniorSeriesSpacers();
                    components = engineLong.PadRight(outputSpacers[3], ' ');
                    break;
            }

            if (runningState == RunningState.RUNNING)
            {
                displayOVR = Convert.ToString(currentOVR);
            }
            else
            {
                displayOVR = String.Format("{0} - {1}", currentOVR, nonCompeteReason);
            }

            return String.Format("{0} - {1} {2} - {3} - {4}", driverName.PadRight(outputSpacers[0], ' '), carNumber.PadRight(outputSpacers[1], ' '), teamName.PadRight(outputSpacers[2], ' '), components, displayOVR); // Needs Spacering
        }

        public string GetEntrantAsWriteString(int componentDisplayType)
        {
            string components = "";
            string displayOVR;

            switch (enteredSeries)
            {
                case "Main Series":
                    switch (componentDisplayType)
                    {
                        case 1:
                            components = String.Format("{0},{1},{2}", chassisLong, engineLong, tyresLong);
                            break;

                        case 2:
                            components = String.Format("{0},{1},{2}", chassisShort, engineShort, tyresShort);
                            break;
                    }
                    break;
                case "Junior Series":
                    components = engineLong;
                    break;
            }

            if (runningState == RunningState.RUNNING)
            {
                displayOVR = Convert.ToString(currentOVR);
            }
            else
            {
                displayOVR = nonCompeteReason;
            }

            return String.Format("{0},{1},{2},{3},{4}", driverName, carNumber, teamName, components, displayOVR);
        }

        public string GetEntrantAsSaveString()
        {
            return String.Format("{0},{1},{2},{3},{4},{5},{6},{7}\n", driverName, carNumber, teamName, chassisLong, engineLong, tyresLong, baseOVR, reliabilityScore);
        }
    }
}
