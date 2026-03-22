using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.CrewScheduling.OptimimzerSettings
{
    public class RosterLegalityRulesParameters : RecordBaseModel
    {
        private int _DaysOff { get; set; }
        private int _MinHoursBetween { get; set; }
        private int _LatestArrivalTime { get; set; }
        private int _EarliestDepartureTime { get; set; }
        private int _XHoursBreak { get; set; }
        private int _YFlightHours { get; set; }
        private int _MaximumFlightHours { get; set; }

        public int DaysOff
        {
            get { return _DaysOff; }
            set { _DaysOff = value; OnPropertyChanged("DaysOff"); }
        }
        public int MinHoursBetween
        {
            get { return _MinHoursBetween; }
            set { _MinHoursBetween = value; OnPropertyChanged("MinHoursBetween"); }
        }
        public int LatestArrivalTime
        {
            get { return _LatestArrivalTime; }
            set { _LatestArrivalTime = value; OnPropertyChanged("LatestArrivalTime"); }
        }
        public int EarliestDepartureTime
        {
            get { return _EarliestDepartureTime; }
            set { _EarliestDepartureTime = value; OnPropertyChanged("EarliestDepartureTime"); }
        }
        public int XHoursBreak
        {
            get { return _XHoursBreak; }
            set { _XHoursBreak = value; OnPropertyChanged("XHoursBreak"); }
        }
        public int YFlightHours
        {
            get { return _YFlightHours; }
            set { _YFlightHours = value; OnPropertyChanged("YFlightHours"); }
        }
        public int MaximumFlightHours
        {
            get { return _MaximumFlightHours; }
            set { _MaximumFlightHours = value; OnPropertyChanged("MaximumFlightHours"); }
        }
    }
}
