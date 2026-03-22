using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Thesis
{
    public class EmpCrewCategsData : INotifyPropertyChanged
    {
        private int finalemployeeid { get; set; }

        private CrewCategData _CrewCateg{ get; set; }

        private bool crewcatflag { get; set; }

        private bool newcrewcatflag { get; set; }
        private bool existingflag { get; set; }

        public bool ExistingFlag
        {
            get { return existingflag; }
            set { existingflag = value; OnPropertyChanged("ExistingFlag"); }
        }
        public bool NewCrewCatFlag
        {
            get { return newcrewcatflag; }
            set { newcrewcatflag = value; OnPropertyChanged("NewCrewCatFlag"); }
        }
        public bool CrewCatFlag
        {
            get { return crewcatflag; }
            set { crewcatflag = value; OnPropertyChanged("CrewCatFlag"); }
        }

        public int FinalEmployeeId
        {
            get { return finalemployeeid; }
            set { finalemployeeid = value; OnPropertyChanged("FinalEmployeeId"); }
        }


        public CrewCategData CrewCateg
        {
            get { return _CrewCateg; }
            set { _CrewCateg = value; OnPropertyChanged("CrewCateg"); }
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
