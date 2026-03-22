using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.CrewScheduling.OptimimzerSettings
{
    public class BranchingParameters : RecordBaseModel
    {
        private int _AbsBacktrack;
        private double _PerceBacktrack;
        private int _AbsMIP;
        private double _PerceMIP;
        private int _NumberOfBacktracksLimit;
        private int _NumberOfTreeNodesLimit;
        private int _TreelistSortingIdsFixed;

        public int AbsBacktrack
        {
            get { return _AbsBacktrack; }
            set { _AbsBacktrack = value; OnPropertyChanged("AbsBacktrack"); }
        }
        public double PerceBacktrack
        {
            get { return _PerceBacktrack; }
            set { _PerceBacktrack = value; OnPropertyChanged("PerceBacktrack"); }
        }
        public int AbsMIP
        {
            get { return _AbsMIP; }
            set { _AbsMIP = value; OnPropertyChanged("AbsMIP"); }
        }
        public double PerceMIP
        {
            get { return _PerceMIP; }
            set { _PerceMIP = value; OnPropertyChanged("PerceMIP"); }
        }
        public int NumberOfBacktracksLimit
        {
            get { return _NumberOfBacktracksLimit; }
            set { _NumberOfBacktracksLimit = value; OnPropertyChanged("NumberOfBacktracksLimit"); }
        }
        public int NumberOfTreeNodesLimit
        {
            get { return _NumberOfTreeNodesLimit; }
            set { _NumberOfTreeNodesLimit = value; OnPropertyChanged("NumberOfTreeNodesLimit"); }
        }
        public int TreelistSortingIdsFixed
        {
            get { return _TreelistSortingIdsFixed; }
            set { _TreelistSortingIdsFixed = value; OnPropertyChanged("TreelistSortingIdsFixed"); }
        }
    }
}
