using Erp.Model.BasicFiles;
using Erp.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis.Filters
{
    public class AirportsFilterData : RecordBaseModel
    {


        private CityData _City { get; set; }

        public CityData City
        {
            get { return _City; }
            set { _City = value; OnPropertyChanged("City"); }
        }


        private bool _ShowDeleted { get; set; }

        public bool ShowDeleted
        {
            get { return _ShowDeleted; }
            set { _ShowDeleted = value; OnPropertyChanged("ShowDeleted"); }
        }
    }

}
