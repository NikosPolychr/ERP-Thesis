using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ILOG.CPLEX.Cplex.Callback.Context;

namespace Erp.Model.Thesis.CrewScheduling.OptimimzerSettings
{
    public class SpecialConstraintsParameters : RecordBaseModel
    {
        #region Activations
        private bool _MinMax_Act;
        private bool _With_Act;
        private bool _Without_Act;
        private bool _Gender_Act;

        public bool MinMaxAct
        {
            get { return _MinMax_Act; }
            set { _MinMax_Act = value; OnPropertyChanged("MinMaxAct"); }
        }
        public bool WithAct
        {
            get { return _With_Act; }
            set { _With_Act = value; OnPropertyChanged("WithAct"); }
        }
        public bool WithoutAct
        {
            get { return _Without_Act; }
            set { _Without_Act = value; OnPropertyChanged("WithoutAct"); }
        }
        public bool GenderAct
        {
            get { return _Gender_Act; }
            set { _Gender_Act = value; OnPropertyChanged("GenderAct"); }
        }
        #endregion

        #region Soft

        private bool _MinMax_Soft;
        private bool _With_Soft;
        private bool _Without_Soft;
        private bool _Gender_Soft;

        public bool MinMaxSoft
        {
            get { return _MinMax_Soft; }
            set { _MinMax_Soft = value; OnPropertyChanged("MinMaxSoft"); }
        }
        public bool WithSoft
        {
            get { return _With_Soft; }
            set { _With_Soft = value; OnPropertyChanged("WithSoft"); }
        }
        public bool WithoutSoft
        {
            get { return _Without_Soft; }
            set { _Without_Soft = value; OnPropertyChanged("WithoutSoft"); }
        }
        public bool GenderSoft
        {
            get { return _Gender_Soft; }
            set { _Gender_Soft = value; OnPropertyChanged("GenderSoft"); }
        }
        #endregion

        #region Priority

        private bool _WithoutVarPriority;
        private bool _GenderVarPriority;

        public bool WithoutPriority
        {
            get { return _WithoutVarPriority; }
            set { _WithoutVarPriority = value; OnPropertyChanged("WithoutPriority"); }
        }
        public bool GenderPriority
        {
            get { return _GenderVarPriority; }
            set { _GenderVarPriority = value; OnPropertyChanged("GenderPriority"); }
        }
        #endregion

    }
}
