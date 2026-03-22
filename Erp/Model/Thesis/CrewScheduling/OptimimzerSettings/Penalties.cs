using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.CrewScheduling.OptimimzerSettings
{
    public class Penalties : RecordBaseModel
    {
        private int _UnderCoverPenalty;
        private int _HourPenalty;
        private int _MinMaxPenalty;
        private int _WithWithoutPenalty;
        private int _GenderPenalty;


        public int UnderCoverPenalty
        {
            get { return _UnderCoverPenalty; }
            set { _UnderCoverPenalty = value; OnPropertyChanged("UnderCoverPenalty"); }
        }
        public int HourPenalty
        {
            get { return _HourPenalty; }
            set { _HourPenalty = value; OnPropertyChanged("HourPenalty"); }
        }
        public int MinMaxPenalty
        {
            get { return _MinMaxPenalty; }
            set { _MinMaxPenalty = value; OnPropertyChanged("MinMaxPenalty"); }
        }
        public int WithWithoutPenalty
        {
            get { return _WithWithoutPenalty; }
            set { _WithWithoutPenalty = value; OnPropertyChanged("WithWithoutPenalty"); }
        }
        public int GenderPenalty
        {
            get { return _GenderPenalty; }
            set { _GenderPenalty = value; OnPropertyChanged("GenderPenalty"); }
        }
    }
}
