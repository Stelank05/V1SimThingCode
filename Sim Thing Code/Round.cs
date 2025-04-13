using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim_Thing_Code
{
    public class Round
    {
        string roundTitle, trackName, countryName, applicableSeries;
        int roundNumber, race1Length, race2Length;

        int incidentModifier, dnfModifier;

        public Round(string[] roundData, int roundIndex, string applicableSeries)
        {
            roundTitle = roundData[0];
            trackName = roundData[1];
            countryName = roundData[2];

            this.applicableSeries = applicableSeries;
            roundNumber = roundIndex;

            race1Length = Convert.ToInt32(roundData[3]);
            race2Length = Convert.ToInt32(roundData[4]);

            incidentModifier = Convert.ToInt32(roundData[5]);
            dnfModifier = Convert.ToInt32(roundData[6]);
        }

        public string GetRoundTitle()
        {
            return roundTitle;
        }

        public string GetTrackName()
        {
            return trackName;
        }

        public string GetCountryName()
        {
            return countryName;
        }

        public int GetRoundNumber()
        {
            return roundNumber;
        }

        public int GetRace1Length()
        {
            return race1Length;
        }

        public int GetRace2Length()
        {
            return race2Length;
        }

        public string GetRoundAsOutputString()
        {
            List<int> spacers = new List<int>();

            switch (applicableSeries)
            {
                case "Main Series":
                    spacers = CommonData.GetMainCalendarSpacerList();
                    break;
                case "Junior Series":
                    spacers = CommonData.GetJuniorCalendarSpacerList();
                    break;
            }

            return String.Format("R{0}: {1} - {2} - {3}", Convert.ToString(roundNumber).PadRight(2, ' '), roundTitle.PadRight(spacers[0], ' '), trackName.PadRight(spacers[1], ' '), countryName.PadRight(spacers[2], ' '));
        }
    }
}
