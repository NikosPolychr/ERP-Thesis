using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.CrewScheduling.OptimimzerSettings
{
    public class NumericParameters : RecordBaseModel
    {
        private double _MaxDouble;
        private double _Eps;

        public double MaxDouble
        {
            get { return _MaxDouble; }
            set { _MaxDouble = value; OnPropertyChanged("MaxDouble"); }
        }
        public double Eps
        {
            get { return _Eps; }
            set { _Eps = value; OnPropertyChanged("Eps"); }
        }
    }
}
