using Accord.Statistics.Filters;
using Erp.Model.BasicFiles;
using Erp.Model.Interfaces;
using Erp.Model.Thesis.CrewScheduling.OptimimzerSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.CrewScheduling
{
    public class OptimizerSettingsData : RecordBaseModel
    {
        private int _Id { get; set; }


        private string _Code { get; set; }

        private string _Descr { get; set; }


        private RosterLegalityRulesParameters _LegalityRulesParams { get; set; }
        private BranchingParameters _BranchingParams { get; set; }
        private ColgenPerformanceParams _ColgenParams { get; set; }
        private NumericParameters _NumericParams { get; set; }
        private Penalties _PenaltiesParams { get; set; }
        private SpecialConstraintsParameters _SpecialConsParams{ get; set; }

        public int Id
        {
            get { return _Id; }
            set { _Id = value; OnPropertyChanged("Id"); }
        }
        public string Code
        {
            get { return _Code; }
            set { _Code = value; OnPropertyChanged("Code"); }
        }
        public string Descr
        {
            get { return _Descr; }
            set { _Descr = value; OnPropertyChanged("Descr"); }
        }
        public RosterLegalityRulesParameters LegalityRulesParams
        {
            get { return _LegalityRulesParams; }
            set { _LegalityRulesParams = value; OnPropertyChanged("LegalityRulesParams"); }
        }

        public BranchingParameters BranchingParams
        {
            get { return _BranchingParams; }
            set { _BranchingParams = value; OnPropertyChanged("BranchingParams"); }
        }
        public ColgenPerformanceParams ColgenParams
        {
            get { return _ColgenParams; }
            set { _ColgenParams = value; OnPropertyChanged("ColgenParams"); }
        }
        public NumericParameters NumericParams
        {
            get { return _NumericParams; }
            set { _NumericParams = value; OnPropertyChanged("NumericParams"); }
        }
        public Penalties PenaltiesParams
        {
            get { return _PenaltiesParams; }
            set { _PenaltiesParams = value; OnPropertyChanged("PenaltiesParams"); }
        }
        public SpecialConstraintsParameters SpecialConsParams
        {
            get { return _SpecialConsParams; }
            set { _SpecialConsParams = value; OnPropertyChanged("SpecialConsParams"); }
        }
    }
}
