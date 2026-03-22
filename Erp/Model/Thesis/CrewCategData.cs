using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis
{
    public class CrewCategData : RecordBaseModel
    {
        private int _CrewCatId { get; set; }
        private string _CrewCatCode { get; set; }

        private string _CrewCatDescr { get; set; }

        public int CrewCatId
        {
            get { return _CrewCatId; }
            set { _CrewCatId = value; OnPropertyChanged("CrewCatId"); }
        }


        public string CrewCatCode
        {
            get { return _CrewCatCode; }
            set { _CrewCatCode = value; OnPropertyChanged("CrewCatCode"); }
        }


        public string CrewCatDescr
        {
            get { return _CrewCatDescr; }
            set { _CrewCatDescr = value; OnPropertyChanged("CrewCatDescr"); }
        }

    }
}
