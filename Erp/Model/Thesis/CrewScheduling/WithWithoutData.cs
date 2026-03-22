using Erp.Model.Enums;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.CrewScheduling
{
    public class WithWithoutData : RecordBaseModel
    {
        private int _RuleId { get; set; }
        private string _Code { get; set; }
        private string _Descr { get; set; }
        private CrewCategData _CrewCat1 { get; set; }
        private BasicEnums.EmployeeType _Position1 { get; set; }
        private CrewCategData _CrewCat2 { get; set; }
        private BasicEnums.EmployeeType _Position2 { get; set; }
        private BasicEnums.RouteCategory _RouteCateg { get; set; }
        private int _PenaltyPoints { get; set; }
        private bool _IsWith { get; set; }
        private bool _IsAct { get; set; }

        public int RuleId
        {
            get { return _RuleId; }
            set { _RuleId = value; OnPropertyChanged("RuleId"); }
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
        public CrewCategData CrewCat1
        {
            get { return _CrewCat1; }
            set { _CrewCat1 = value; OnPropertyChanged("CrewCat1"); }
        }
        public BasicEnums.EmployeeType Position1
        {
            get { return _Position1; }
            set { _Position1 = value; OnPropertyChanged("Position1"); }
        }
        public CrewCategData CrewCat2
        {
            get { return _CrewCat2; }
            set { _CrewCat2 = value; OnPropertyChanged("CrewCat2"); }
        }
        public BasicEnums.EmployeeType Position2
        {
            get { return _Position2; }
            set { _Position2 = value; OnPropertyChanged("Position2"); }
        }

        public BasicEnums.RouteCategory RouteCateg
        {
            get { return _RouteCateg; }
            set { _RouteCateg = value; OnPropertyChanged("RouteCateg"); }
        }
        public int PenaltyPoints
        {
            get { return _PenaltyPoints; }
            set { _PenaltyPoints = value; OnPropertyChanged("PenaltyPoints"); }
        }
        public bool IsWith
        {
            get { return _IsWith; }
            set { _IsWith = value; OnPropertyChanged("IsWith"); }
        }
        public bool IsAct
        {
            get { return _IsAct; }
            set { _IsAct = value; OnPropertyChanged("IsAct"); }
        }
    }
}
