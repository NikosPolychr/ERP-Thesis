using Erp.Model.BasicFiles;
using Erp.Model.Enums;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.CrewScheduling
{
    public class MinMaxData : RecordBaseModel
    {
        private int _RuleId { get; set; }
        private string _Code { get; set; }
        private string _Descr { get; set; }
        private CrewCategData _CrewCat { get; set; }
        private BasicEnums.EmployeeType _Position { get; set; }
        private BasicEnums.RouteCategory _RouteCateg { get; set; }
        private int _Rhs { get; set; }
        private int _PenaltyPoints { get; set; }
        private bool _IsMin { get; set; }
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
        public CrewCategData CrewCat
        {
            get { return _CrewCat; }
            set { _CrewCat = value; OnPropertyChanged("CrewCat"); }
        }
        public BasicEnums.EmployeeType Position
        {
            get { return _Position; }
            set { _Position = value; OnPropertyChanged("Position"); }
        }
        public BasicEnums.RouteCategory RouteCateg
        {
            get { return _RouteCateg; }
            set { _RouteCateg = value; OnPropertyChanged("RouteCateg"); }
        }

        public int Rhs
        {
            get { return _Rhs; }
            set { _Rhs = value; OnPropertyChanged("Rhs"); }
        }

        public int PenaltyPoints
        {
            get { return _PenaltyPoints; }
            set { _PenaltyPoints = value; OnPropertyChanged("PenaltyPoints"); }
        }

        public bool IsMin
        {
            get { return _IsMin; }
            set { _IsMin = value; OnPropertyChanged("IsMin"); }
        }
        public bool IsAct
        {
            get { return _IsAct; }
            set { _IsAct = value; OnPropertyChanged("IsAct"); }
        }
    }

}
