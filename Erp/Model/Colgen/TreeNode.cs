using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Colgen
{
    public class TreeNode : INotifyPropertyChanged
    {
        // Event for property change notifications
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Reference to an external solver object (CPXLPptr)
        private object _lp; // Placeholder for the LP pointer; replace 'object' with the actual solver object type
        public object Lp
        {
            get => _lp;
            set { _lp = value; OnPropertyChanged(); }
        }

        // Indicates whether column generation was performed
        private bool _cgPerformed;
        public bool CGPerformed
        {
            get => _cgPerformed;
            set { _cgPerformed = value; OnPropertyChanged(); }
        }

        // Unique identifier for the tree node
        private int _uid;
        public int UId
        {
            get => _uid;
            set { _uid = value; OnPropertyChanged(); }
        }

        // Master objective value
        private double _masterObjVal;
        public double MasterObjVal
        {
            get => _masterObjVal;
            set { _masterObjVal = value; OnPropertyChanged(); }
        }

        // Array to store times each route must be covered
        private int[] _timesToCover;
        public int[] TimesToCover
        {
            get => _timesToCover;
            set { _timesToCover = value; OnPropertyChanged(); }
        }

        // Number of network route nodes, including one fictitious node
        private int _numberOfNetworkRouteNodes;
        public int NumberOfNetworkRouteNodes
        {
            get => _numberOfNetworkRouteNodes;
            set { _numberOfNetworkRouteNodes = value; OnPropertyChanged(); }
        }

        // Reference to the network list (list of network nodes)
        private NetworkNode[] _networkList;
        public NetworkNode[] NetworkList
        {
            get => _networkList;
            set { _networkList = value; OnPropertyChanged(); }
        }

        // Reference to the next tree node in the list
        private TreeNode _rightNode;
        public TreeNode RightNode
        {
            get => _rightNode;
            set { _rightNode = value; OnPropertyChanged(); }
        }

        // Constructor to initialize the array
        public TreeNode(int timesToCoverSize)
        {
            TimesToCover = new int[timesToCoverSize];
        }
    }
}
