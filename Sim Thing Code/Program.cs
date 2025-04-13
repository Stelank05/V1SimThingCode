using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sim_Thing_Code
{
    class Program
    {
        static void Main()
        {
            Setup();

            Random randomiser = new Random();

            do
            {
                int selectedSeries = SelectSeries();

                Console.Clear();

                if (selectedSeries == 1)
                {
                    StartMainSeries(randomiser);
                }
                else
                {
                    StartJuniorSeries(randomiser);
                }
            }
            while (RunAgain("Continue Running Program?"));

            Console.ReadLine();
        }

        private static void Setup()
        {
            // Setup File Paths
            CommonData.SetRootFolder(Convert.ToString(Directory.GetParent(Directory.GetCurrentDirectory())).Replace("\\Sim Thing Code\\bin", ""));
            CommonData.SetSetupFolder("Setup Files");
            CommonData.SetSaveFolder(@"Results\Season 1");

            CommonData.SetMainSeriesName("Main Series");
            CommonData.SetJuniorSeriesName("Junior Series");

            RaceAdmin.LoadCalendar("Main Series");
            RaceAdmin.LoadCalendar("Junior Series");
        }

        private static int SelectSeries()
        {
            Console.WriteLine("Select Series:\n1. {0}\n2. {1}", CommonData.GetMainSeriesName(), CommonData.GetJuniorSeriesName());
            Console.Write("Choice: ");
            string strSelectedChoice = Console.ReadLine();

            bool validInt = int.TryParse(strSelectedChoice, out int selectedChoice);

            if (!validInt)
            {
                return SelectSeries();
            }

            if (selectedChoice < 1 || selectedChoice > 2)
            {
                return SelectSeries();
            }

            return selectedChoice;
        }

        private static int SelectRound(List<Round> calendar)
        {
            Console.WriteLine("Calendar:\n");
            foreach (Round round in calendar)
            {
                Console.WriteLine(round.GetRoundAsOutputString());
            }
            Console.Write("Choice: ");

            string strSelectedRound = Console.ReadLine();

            bool validInt = int.TryParse(strSelectedRound, out int selectedRound);

            if (!validInt)
            {
                return SelectRound(calendar);
            }

            if (selectedRound < 1 || selectedRound > calendar.Count)
            {
                return SelectRound(calendar);
            }

            return selectedRound - 1;
        }

        private static void StartMainSeries(Random randomiser)
        {
            List<Round> calendar = CommonData.GetMainSeriesCalendar();
            int selectedRound = SelectRound(calendar);

            Console.Clear();

            RunMainSeries(calendar, selectedRound, randomiser);
        }

        private static void RunMainSeries(List<Round> calendar, int selectedRound, Random randomiser)
        {
            Round currentRound = calendar[selectedRound];

            string resultsFolder = Path.Combine(CommonData.GetMainSeriesResultsFolder(), String.Format("R{0} - {1}", currentRound.GetRoundNumber(), currentRound.GetRoundTitle()));
            Directory.CreateDirectory(resultsFolder);

            List<Entrant> entryList = RaceAdmin.LoadEntryList("Main Series");

            int fileNumber = 1;

            // Practice
            SimulatePracticeStint(entryList, randomiser);
            SortEntrants(entryList, 1, entryList.Count);

            Console.WriteLine("Practice Results:\n");
            OutputEntrants(entryList, 1, entryList.Count);

            SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{0} - Practice Results.csv", fileNumber)));
            fileNumber++;

            Console.ReadLine();

            // Set Qualifying 1 Order

            List<int> orderList = new List<int>();

            for (int i = 0; i < entryList.Count; i++)
            {
                orderList.Add(i + 1);
            }

            int qualifyingSpot;

            foreach (Entrant currentEntrant in entryList)
            {
                qualifyingSpot = orderList[randomiser.Next(0, orderList.Count)];
                currentEntrant.SetQualifyingSpot(qualifyingSpot);
                orderList.Remove(qualifyingSpot);
            }

            QualifyingOrderSort(entryList);

            Console.WriteLine("Qualifying Running Order:\n");
            OutputEntrants(entryList, 1, entryList.Count);

            SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{0} - Qualifying Running Order.csv", fileNumber)));
            fileNumber++;

            Console.ReadLine();

            // Qualifying 1

            SimulateQualifyingStint(entryList, randomiser);

            SortEntrants(entryList, 1, entryList.Count);

            Console.WriteLine("Qualifying 1 Results:\n");
            OutputEntrants(entryList, 1, entryList.Count);

            SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{0} - Qualifying 1 Results.csv", fileNumber)));
            fileNumber++;

            Console.ReadLine();

            // Set R2 Grid

            for (int i = 0; i < entryList.Count; i++)
            {
                entryList[i].SetRace2GridSpot(i + 1);
            }

            // Qualifying 2

            List<Entrant> qualifying2EntryList = new List<Entrant>();

            for (int i = 7; i >= 0; i--)
            {
                qualifying2EntryList.Add(entryList[i]);
            }

            SimulateQualifyingStint(qualifying2EntryList, randomiser);

            SortEntrants(qualifying2EntryList, 1, qualifying2EntryList.Count);

            Console.WriteLine("Qualifying 2 Results:\n");
            OutputEntrants(qualifying2EntryList, 1, qualifying2EntryList.Count);
            
            SaveStint(qualifying2EntryList, 1, qualifying2EntryList.Count, Path.Combine(resultsFolder, String.Format("{0} - Qualifying 2 Results.csv", fileNumber)));
            fileNumber++;

            Console.ReadLine();

            // Set R1 Grid

            List<Entrant> backupList = new List<Entrant>();

            foreach (Entrant entrant in entryList)
            {
                backupList.Add(entrant);
            }

            entryList.Clear();

            foreach (Entrant entrant in qualifying2EntryList)
            {
                entryList.Add(entrant);
            }

            for (int i = qualifying2EntryList.Count; i < backupList.Count; i++)
            {
                entryList.Add(backupList[i]);
            }

            Scrutineering(entryList, randomiser);

            Console.WriteLine("Full Qualifying Results:\n");
            OutputEntrants(entryList, 1, entryList.Count);

            SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{0} - Full Qualifying Results.csv", fileNumber)));
            fileNumber++;

            Console.ReadLine();

            /*for (int i = 0; i < entryList.Count; i++)
            {
                entryList[i].SetRace1GridSpot(i + 1);
            }*/

            // Sim Race 1

            SetGrid(entryList, 5, randomiser);

            Console.WriteLine("Grid for Race 1:\n");
            OutputEntrants(entryList, 1, entryList.Count);

            SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{0} - Race 1 Grid.csv", fileNumber)));
            fileNumber++;

            Console.ReadLine();

            MoveNonStarters(entryList);

            for (int stintNumber = 0; stintNumber < currentRound.GetRace1Length(); stintNumber++)
            {
                SimulateRaceStint(entryList, randomiser);
                SortEntrants(entryList, 1, entryList.Count);

                Console.WriteLine("Stint {0} Running Order:\n", stintNumber + 1);
                OutputEntrants(entryList, 1, entryList.Count);

                SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{1} - Race 1 - Stint {0}.csv", stintNumber + 1, fileNumber)));
                fileNumber++;

                Console.ReadLine();
            }

            Scrutineering(entryList, randomiser);

            Console.WriteLine("Race 1 Results:\n");
            OutputEntrants(entryList, 1, entryList.Count);

            SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{0} - Race 1 - Race Results.csv", fileNumber)));
            fileNumber++;

            Console.ReadLine();

            Console.Clear();

            // Sim Race 2

            GridSort(entryList, "Race 2");
            SetGrid(entryList, 5, randomiser);

            Console.WriteLine("Grid for Race 2:\n");
            OutputEntrants(entryList, 1, entryList.Count);

            SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{0} - Race 2 Grid.csv", fileNumber)));
            fileNumber++;
            Console.ReadLine();

            MoveNonStarters(entryList);

            for (int stintNumber = 0; stintNumber < currentRound.GetRace2Length(); stintNumber++)
            {
                SimulateRaceStint(entryList, randomiser);
                SortEntrants(entryList, 1, entryList.Count);

                Console.WriteLine("Stint {0} Running Order:\n", stintNumber + 1);
                OutputEntrants(entryList, 1, entryList.Count);
                
                SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{1} - Race 2 - Stint {0}.csv", stintNumber + 1, fileNumber)));
                fileNumber++;

                Console.ReadLine();
            }

            Scrutineering(entryList, randomiser);

            Console.WriteLine("Race 2 Results:\n");
            OutputEntrants(entryList, 1, entryList.Count);

            SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{0} - Race 2 - Race Results.csv", fileNumber)));
//            fileNumber++;

            Console.ReadLine();

            Console.Clear();

            if (RunAgain("Do Another Race?"))
            {
                RunMainSeries(calendar, selectedRound + 1, randomiser);
            }
        }

        private static void StartJuniorSeries(Random randomiser)
        {
            List<Round> calendar = CommonData.GetJuniorSeriesCalendar();
            int selectedRound = SelectRound(calendar);

            Console.Clear();

            RunJuniorSeries(calendar, selectedRound, randomiser);
        }

        private static void RunJuniorSeries(List<Round> calendar, int selectedRound, Random randomiser)
        {
            Round currentRound = calendar[selectedRound];
            string resultsFolder = Path.Combine(CommonData.GetJuniorSeriesResultsFolder(), String.Format("R{0} - {1}", currentRound.GetRoundNumber(), currentRound.GetRoundTitle()));
            Directory.CreateDirectory(resultsFolder);

            List<Entrant> entryList = RaceAdmin.LoadEntryList("Junior Series");

            int fileNumber = 1;

            /*SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, "Test Output.csv"));

            Console.WriteLine("Junior Series {0} Entrants:\n", currentRound.GetRoundTitle());
            foreach (Entrant entrant in entryList)
            {
                Console.WriteLine(entrant.GetEntrantAsOutputString(1));
            }*/

            // Practice
            SimulatePracticeStint(entryList, randomiser);
            SortEntrants(entryList, 1, entryList.Count);

            Console.WriteLine("Practice Results:\n");
            OutputEntrants(entryList, 1, entryList.Count);

            SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{0} Practice Results.csv", fileNumber)));
            fileNumber++;

            Console.ReadLine();


            List<int> orderList = new List<int>();

            for (int i = 0; i < entryList.Count; i++)
            {
                orderList.Add(i + 1);
            }

            int qualifyingSpot;

            foreach (Entrant currentEntrant in entryList)
            {
                qualifyingSpot = orderList[randomiser.Next(0, orderList.Count)];
                currentEntrant.SetQualifyingSpot(qualifyingSpot);
                orderList.Remove(qualifyingSpot);
            }

            QualifyingOrderSort(entryList);

            Console.WriteLine("Qualifying Running Order:\n");
            OutputEntrants(entryList, 1, entryList.Count);

            SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{0} - Qualifying Running Order.csv", fileNumber)));
            fileNumber++;

            Console.ReadLine();

            // Qualifying 1

            SimulateQualifyingStint(entryList, randomiser);

            SortEntrants(entryList, 1, entryList.Count);

            Console.WriteLine("Qualifying 1 Results:\n");
            OutputEntrants(entryList, 1, entryList.Count);

            SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{0} - Qualifying Results.csv", fileNumber)));
            fileNumber++;

            Console.ReadLine();

            Scrutineering(entryList, randomiser);

            Console.WriteLine("Full Qualifying Results:\n");
            OutputEntrants(entryList, 1, entryList.Count);

            SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{0} - Full Qualifying Results.csv", fileNumber)));
            fileNumber++;

            Console.ReadLine();

            // Set Race Grid

            SetGrid(entryList, 5, randomiser);

            // Sim Race

            Console.WriteLine("Grid for Race 1:\n");
            OutputEntrants(entryList, 1, entryList.Count);

            SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{0} - Grid.csv", fileNumber)));
            fileNumber++;

            Console.ReadLine();

            MoveNonStarters(entryList);

            for (int stintNumber = 0; stintNumber < currentRound.GetRace1Length(); stintNumber++)
            {
                SimulateRaceStint(entryList, randomiser);
                SortEntrants(entryList, 1, entryList.Count);

                Console.WriteLine("Stint {0} Running Order:\n", stintNumber + 1);
                OutputEntrants(entryList, 1, entryList.Count);

                SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{1} - Race - Stint {0}.csv", stintNumber + 1, fileNumber)));
                fileNumber++;

                Console.ReadLine();
            }

            Scrutineering(entryList, randomiser);

            Console.WriteLine("Race 1 Results:\n");
            OutputEntrants(entryList, 1, entryList.Count);

            SaveStint(entryList, 1, entryList.Count, Path.Combine(resultsFolder, String.Format("{0} - Race - Race Results.csv", fileNumber)));

            Console.ReadLine();

            Console.Clear();

            if (RunAgain("Do Another Race?"))
            {
                RunJuniorSeries(calendar, selectedRound + 1, randomiser);
            }
        }

        private static void SimulatePracticeStint(List<Entrant> entryList, Random randomiser)
        {
            foreach (Entrant currentEntrant in entryList)
            {
                if (currentEntrant.GetRunningState() == RunningState.RUNNING)
                {
                    int entrantBaseOVR = currentEntrant.GetBaseOVR();
                    int ovrFraction = Convert.ToInt32(Math.Round(entrantBaseOVR / 12.0, 0));
                    int incidentTrigger = Convert.ToInt32(Math.Round(currentEntrant.GetReliabilityScore() / 20.0, 0));

                    int stintScore = randomiser.Next(Convert.ToInt32(Math.Round(ovrFraction * 0.25, 0)), ovrFraction + 1);
                    int incidentScore = randomiser.Next(1, currentEntrant.GetReliabilityScore() + 1);

                    if (incidentScore < incidentTrigger)
                    {
                        if (incidentScore < 3)
                        {
                            int crashSize = randomiser.Next(1, 21);

                            if (crashSize == 1)
                            {
                                currentEntrant.SetRunningState(RunningState.WITHDRAWN);
                                currentEntrant.SetNonCompeteReason(GenerateWithdrawalReason(randomiser));
                            }

                            else
                            {
                                currentEntrant.UpdateOVR(Convert.ToInt32(Math.Round(entrantBaseOVR + (stintScore * .5), 0)));
                            }
                        }

                        else
                        {
                            currentEntrant.UpdateOVR(Convert.ToInt32(Math.Round(entrantBaseOVR + (stintScore * .8), 0)));
                        }
                    }

                    else
                    {
                        currentEntrant.UpdateOVR(entrantBaseOVR + stintScore);
                    }
                }
            }
        }
        
        private static void SimulateQualifyingStint(List<Entrant> entryList, Random randomiser)
        {
            foreach (Entrant currentEntrant in entryList)
            {
                if (currentEntrant.GetRunningState() == RunningState.RUNNING)
                {
                    int entrantBaseOVR = currentEntrant.GetBaseOVR();
                    int ovrFraction = Convert.ToInt32(Math.Round(entrantBaseOVR / 10.0, 0));
                    int incidentTrigger = Convert.ToInt32(Math.Round(currentEntrant.GetReliabilityScore() / 22.0, 0));

                    int stintScore = randomiser.Next(Convert.ToInt32(Math.Round(ovrFraction * 0.25, 0)), ovrFraction + 1);
                    int incidentScore = randomiser.Next(1, currentEntrant.GetReliabilityScore() + 1);

                    if (incidentScore < incidentTrigger)
                    {
                        if (incidentScore < 3)
                        {
                            int crashSize = randomiser.Next(1, 21);

                            if (crashSize == 1)
                            {
                                currentEntrant.SetRunningState(RunningState.WITHDRAWN);
                                currentEntrant.SetNonCompeteReason(GenerateWithdrawalReason(randomiser));
                            }

                            else
                            {
                                currentEntrant.UpdateOVR(Convert.ToInt32(Math.Round(entrantBaseOVR + (stintScore * .5), 0)));
                            }
                        }

                        else
                        {
                            currentEntrant.UpdateOVR(Convert.ToInt32(Math.Round(entrantBaseOVR + (stintScore * .8), 0)));
                        }
                    }

                    else
                    {
                        currentEntrant.UpdateOVR(entrantBaseOVR + stintScore);
                    }
                }
            }
        }

        private static void SimulateRaceStint(List<Entrant> entryList, Random randomiser)
        {
            foreach (Entrant currentEntrant in entryList)
            {
                if (currentEntrant.GetRunningState() == RunningState.RUNNING)
                {
                    int ovrFraction = Convert.ToInt32(Math.Round(currentEntrant.GetBaseOVR() / 10.0, 0));
                    int incidentTrigger = Convert.ToInt32(Math.Round(currentEntrant.GetReliabilityScore() / 17.0, 0));

                    int stintScore = randomiser.Next(Convert.ToInt32(Math.Round(ovrFraction * 0.25, 0)), ovrFraction + 1);
                    int pitScore = randomiser.Next(4, 11);
                    int incidentScore = randomiser.Next(1, currentEntrant.GetReliabilityScore() + 1);

                    if (incidentScore < incidentTrigger)
                    {
                        if (incidentScore < 5)
                        {
                            currentEntrant.SetRunningState(RunningState.RETIRED);
                            currentEntrant.SetNonCompeteReason(GenerateRetirementReason(randomiser));
                        }

                        else
                        {
                            currentEntrant.UpdateOVR(Convert.ToInt32(Math.Round(currentEntrant.GetOVR() + (stintScore * .8), 0)) + (pitScore - 3));
                        }
                    }

                    else
                    {
                        currentEntrant.UpdateOVR(currentEntrant.GetOVR() + stintScore + pitScore);
                    }
                }
            }
        }

        private static string GenerateWithdrawalReason(Random randomiser)
        {
            string reason;

            int withdrawalReason = randomiser.Next(1, 5);

            /* if (withdrawalReason == 1)
            {
                reason = "Personal";
            } // */

            if (withdrawalReason == 2)
            {
                reason = "Illness";
            }

            else
            {
                reason = "Damage";
            }

            return reason;
        }

        private static string GenerateDNSReason(Random randomiser)
        {
            string reason;

            int dnsReason = randomiser.Next(1, 5);

            if (dnsReason == 1)
            {
                reason = "Illness";
            }

            else if (dnsReason == 2)
            {
                reason = "Car Issue";
            }

            else
            {
                reason = "Excessive Damage";
            }

            return reason;
        }

        private static string GenerateDSQReason(Random randomiser)
        {
            string reason;

            int dsqReason = randomiser.Next(1, 5);

            if (dsqReason == 1)
            {
                reason = "Underweight";
            }

            else if (dsqReason == 2)
            {
                reason = "Fuel Sample";
            }

            else if (dsqReason == 3)
            {
                reason = "Plank Wear";
            }

            else
            {
                reason = "Illegal Component";
            }

            return reason;
        }

        private static string GenerateRetirementReason(Random randomiser)
        {
            string reason;

            int retirementReason = randomiser.Next(1, 6);

            if (retirementReason == 1)
            {
                reason = "Crash";
            }

            else if (retirementReason == 2)
            {
                reason = "Collision";
            }

            else if (retirementReason == 3)
            {
                reason = "Damage";
            }

            else if (retirementReason == 4)
            {
                reason = "Loose Wheel";
            }

            else
            {
                reason = "Failure";
            }

            return reason;
        }

        private static void SortEntrants(List<Entrant> entryList, int startIndex, int endIndex)
        {
            bool swap;

            for (int i = 0; i < endIndex - 1; i++)
            {
                swap = false;

                for (int j = startIndex - 1; j < endIndex - i - 1; j++)
                {
                    if (entryList[j].GetOVR() < entryList[j + 1].GetOVR() && entryList[j + 1].GetRunningState() == RunningState.RUNNING)
                    {
                        swap = true;
                        (entryList[j], entryList[j + 1]) = (entryList[j + 1], entryList[j]);
                    }
                }

                if (!swap)
                {
                    break;
                }
            }

            EnumSort(entryList, startIndex, endIndex);
        }

        private static void EnumSort(List<Entrant> entryList, int startIndex, int endIndex)
        {
            bool swap;

            for (int i = 0; i < endIndex - 1; i++)
            {
                swap = false;

                for (int j = 0; j < endIndex - i - 1; j++)
                {
                    if ((int)entryList[j].GetRunningState() > (int)entryList[j + 1].GetRunningState())
                    {
                        swap = true;
                        (entryList[j], entryList[j + 1]) = (entryList[j + 1], entryList[j]);
                    }
                }

                if (!swap)
                {
                    break;
                }
            }
        }

        private static void QualifyingOrderSort(List<Entrant> entryList)
        {
            bool swap;

            for (int i = 0; i < entryList.Count - 1; i++)
            {
                swap = false;

                for (int j = 0; j < entryList.Count - i - 1; j++)
                {
                    if (entryList[j].GetQualifyingSpot() > entryList[j + 1].GetQualifyingSpot())
                    {
                        swap = true;
                        (entryList[j], entryList[j + 1]) = (entryList[j + 1], entryList[j]);
                    }
                }

                if (!swap)
                {
                    break;
                }
            }
        }

        private static void GridSort(List<Entrant> entryList, string raceNumber)
        {
            bool swap;

            int driver1Position = -1, driver2Position = -1;

            for (int i = 0; i < entryList.Count - 1; i++)
            {
                swap = false;

                for (int j = 0; j < entryList.Count - i - 1; j++)
                {
                    switch (raceNumber)
                    {
                        case "Race 1":
                            driver1Position = entryList[j].GetRace1GridSpot();
                            driver2Position = entryList[j + 1].GetRace1GridSpot();
                            break;
                        case "Race 2":
                            driver1Position = entryList[j].GetRace2GridSpot();
                            driver2Position = entryList[j + 1].GetRace2GridSpot();
                            break;
                    }

                    if (driver1Position > driver2Position)
                    {
                        swap = true;
                        (entryList[j], entryList[j + 1]) = (entryList[j + 1], entryList[j]);
                    }
                }

                if (!swap)
                {
                    break;
                }
            }
        }

        private static void Scrutineering(List<Entrant> entryList, Random randomiser)
        {
            foreach (Entrant entrant in entryList)
            {
                int dsqChance = randomiser.Next(1, 601);

                if (dsqChance == 1)
                {
                    entrant.SetRunningState(RunningState.DISQUALIFIED);
                    entrant.SetRace1GridSpot(entrant.GetRace1GridSpot() + entryList.Count);
                    entrant.SetRace2GridSpot(entrant.GetRace1GridSpot() + entryList.Count);
                    entrant.SetNonCompeteReason(GenerateDSQReason(randomiser));
                }
            }

            List<Entrant> excludeList = new List<Entrant>();

            for (int i = 0; i < entryList.Count; i++)
            {
                if (entryList[i].GetRunningState() == RunningState.DISQUALIFIED)
                {
                    excludeList.Add(entryList[i]);
                    entryList.RemoveAt(i);
                    i--;
                }
            }

            foreach (Entrant entrant in excludeList)
            {
                entryList.Add(entrant);
            }

            excludeList.Clear();
        }

        private static void SetGrid(List<Entrant> entryList, int gridSpacer, Random randomiser)
        {
            int entrantCount = entryList.Count;
            RunningState entrantState;

            for (int i = 0; i < entrantCount; i++)
            {
                entrantState = entryList[i].GetRunningState();
                if (entrantState != RunningState.WITHDRAWN)
                {
                    int dnsChance = randomiser.Next(1, 176);

                    if (dnsChance == 1)
                    {
                        entryList[i].SetRunningState(RunningState.NOT_STARTING);
                        entryList[i].SetNonCompeteReason(GenerateDNSReason(randomiser));
                    }

                    else
                    {
                        entryList[i].SetRunningState(RunningState.RUNNING);
                        entryList[i].UpdateOVR(entryList[i].GetBaseOVR() + (gridSpacer * (entrantCount - i)));
                    }
                }
            }
        }

        private static void MoveNonStarters(List<Entrant> entryList)
        {
            List<Entrant> nonStarters = new List<Entrant>();

            for (int i = 0; i < entryList.Count; i++)
            {
                if (entryList[i].GetRunningState() == RunningState.NOT_STARTING)
                {
                    nonStarters.Add(entryList[i]);
                    entryList.RemoveAt(i);
                    i--;
                }
            }

            foreach (Entrant entrant in nonStarters)
            {
                entryList.Add(entrant);
            }
        }

        private static void OutputEntrants(List<Entrant> entryList, int startIndex, int endIndex)
        {
            string pos = "";

            for (int i = startIndex - 1; i < endIndex; i++)
            {
                switch (entryList[i].GetRunningState())
                {
                    case RunningState.RUNNING:
                        pos = String.Format("P{0}", i + 1).PadRight(3, ' ');
                        break;
                    case RunningState.RETIRED:
                        pos = "DNF";
                        break;
                    case RunningState.NOT_STARTING:
                        pos = "DNS";
                        break;
                    case RunningState.DISQUALIFIED:
                        pos = "DSQ";
                        break;
                    case RunningState.WITHDRAWN:
                        pos = "WD".PadRight(3, ' ');
                        break;
                }

                Console.WriteLine("{0}: {1}", pos, entryList[i].GetEntrantAsOutputString(1));
            }
        }

        private static bool RunAgain(string startMessage)
        {
            Console.Write("{0}\nY - Yes\nN - No\nChoice: ", startMessage);
            string strChoice = Console.ReadLine().ToLower();

            if (strChoice[0] == 'y')
            {
                return true;
            }
            else if (strChoice[0] == 'n')
            {
                return false;
            }

            return RunAgain(startMessage);
        }

        private static void SaveStint(List<Entrant> entryList, int startIndex, int endIndex, string fileName)
        {
            string writeString = "", pos = "";

            for (int i = startIndex - 1; i < endIndex; i++)
            {
                switch (entryList[i].GetRunningState())
                {
                    case RunningState.RUNNING:
                        pos = String.Format("P{0}", i + 1);
                        break;
                    case RunningState.RETIRED:
                        pos = "DNF";
                        break;
                    case RunningState.NOT_STARTING:
                        pos = "DNS";
                        break;
                    case RunningState.DISQUALIFIED:
                        pos = "DSQ";
                        break;
                    case RunningState.WITHDRAWN:
                        pos = "WD";
                        break;
                }

                writeString += pos + "," + entryList[i].GetEntrantAsWriteString(2) + "\n";
            }

            File.WriteAllText(fileName, writeString);
        }
    }
}
