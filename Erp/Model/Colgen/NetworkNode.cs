using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Colgen
{
    public class NetworkNode : INotifyPropertyChanged
    {
        // Event for property change notifications
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Index of the route
        private int _rtind;
        public int Rtind
        {
            get => _rtind;
            set { _rtind = value; OnPropertyChanged(); }
        }

        // Node index
        private int _index;
        public int Index
        {
            get => _index;
            set { _index = value; OnPropertyChanged(); }
        }

        // Team associated with the node
        private int _team;
        public int Team
        {
            get => _team;
            set { _team = value; OnPropertyChanged(); }
        }

        // Array to hold rejected previous nodes (or IDs)
        private int[] _prevRejected;
        public int[] PrevRejected
        {
            get => _prevRejected;
            set { _prevRejected = value; OnPropertyChanged(); }
        }

        // Array of previous nodes
        private NetworkNode[] _prev;
        public NetworkNode[] Prev
        {
            get => _prev;
            set { _prev = value; OnPropertyChanged(); }
        }

        // Array to hold paths from previous nodes
        private int[] _prevPath;
        public int[] PrevPath
        {
            get => _prevPath;
            set { _prevPath = value; OnPropertyChanged(); }
        }

        // Array holding the number of days off for IDs
        private int[] _idDaysOff;
        public int[] IdDaysOff
        {
            get => _idDaysOff;
            set { _idDaysOff = value; OnPropertyChanged(); }
        }

        // Array holding flight hours since the last rest for IDs
        private int[] _fltHrsSinceLastRest;
        public int[] FltHrsSinceLastRest
        {
            get => _fltHrsSinceLastRest;
            set { _fltHrsSinceLastRest = value; OnPropertyChanged(); }
        }

        // Array of reduced costs for IDs
        private double[] _rc;
        public double[] RC
        {
            get => _rc;
            set { _rc = value; OnPropertyChanged(); }
        }

        // Array of dual sums for IDs
        private double[] _dualSum;
        public double[] DualSum
        {
            get => _dualSum;
            set { _dualSum = value; OnPropertyChanged(); }
        }

        // Array holding the sum of flight hours
        private int[] _flightHrsSum;
        public int[] FlightHrsSum
        {
            get => _flightHrsSum;
            set { _flightHrsSum = value; OnPropertyChanged(); }
        }

        // Reference to the right node in the network
        private NetworkNode _rightNode;
        public NetworkNode RightNode
        {
            get => _rightNode;
            set { _rightNode = value; OnPropertyChanged(); }
        }
    }
}
