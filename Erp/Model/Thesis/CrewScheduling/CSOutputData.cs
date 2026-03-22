using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.CrewScheduling
{
    public class CSOutputData : RecordBaseModel
    {
        public Stopwatch Clock_cs { get; set; }
        private double _ObjValue;
        public double ObjValue
        {
            get { return _ObjValue; }
            set
            {
                _ObjValue = value;
                OnPropertyChanged(nameof(ObjValue));
            }
        }
    }
}
