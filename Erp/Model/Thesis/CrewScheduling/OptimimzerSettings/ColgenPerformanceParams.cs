using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.CrewScheduling.OptimimzerSettings
{
    public class ColgenPerformanceParams : RecordBaseModel
    {
        private int _Buckets;
        private int _Fictbuckets;
        private int _ReducedCostCut;
        private double _NIDRAI;

        public int Buckets
        {
            get { return _Buckets; }
            set { _Buckets = value; OnPropertyChanged("Buckets"); }
        }
        public int Fictbuckets
        {
            get { return _Fictbuckets; }
            set { _Fictbuckets = value; OnPropertyChanged("Fictbuckets"); }
        }
        public int ReducedCostCut
        {
            get { return _ReducedCostCut; }
            set { _ReducedCostCut = value; OnPropertyChanged("ReducedCostCut"); }
        }
        public double NIDRAI
        {
            get { return _NIDRAI; }
            set { _NIDRAI = value; OnPropertyChanged("NIDRAI"); }
        }

    }
}
