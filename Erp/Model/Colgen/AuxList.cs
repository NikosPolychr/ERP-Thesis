using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Colgen
{
    public class AuxList : INotifyPropertyChanged
    {
        // Event for property change notifications
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Unique identifier
        private int _id;
        public int ID
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        // Lower bound ID
        private int _lBid;
        public int LBid
        {
            get => _lBid;
            set { _lBid = value; OnPropertyChanged(); }
        }

        // Upper bound ID
        private int _uBid;
        public int UBid
        {
            get => _uBid;
            set { _uBid = value; OnPropertyChanged(); }
        }

        // Initial route count
        private int _rtCntInitial;
        public int RtcntInitial
        {
            get => _rtCntInitial;
            set { _rtCntInitial = value; OnPropertyChanged(); }
        }

        // Final route count
        private int _rtCntFinal;
        public int RtcntFinal
        {
            get => _rtCntFinal;
            set { _rtCntFinal = value; OnPropertyChanged(); }
        }

        // Array of indices of variables to subtract
        private int[] _subtractVarsInd;
        public int[] SubtractVarsInd
        {
            get => _subtractVarsInd;
            set { _subtractVarsInd = value; OnPropertyChanged(); }
        }

        // Array of route indices
        private int[] _rtIndices;
        public int[] RtIndices
        {
            get => _rtIndices;
            set { _rtIndices = value; OnPropertyChanged(); }
        }

        // Array of roster IDs
        private int[] _idRoster;
        public int[] IdRoster
        {
            get => _idRoster;
            set { _idRoster = value; OnPropertyChanged(); }
        }

        // Final roster flight hours
        private int _finalRosterFlHrs;
        public int FinalRosterFlHrs
        {
            get => _finalRosterFlHrs;
            set { _finalRosterFlHrs = value; OnPropertyChanged(); }
        }

        // Reference to the right node in the auxiliary list
        private AuxList _rightNode;
        public AuxList RightNode
        {
            get => _rightNode;
            set { _rightNode = value; OnPropertyChanged(); }
        }
    }
}
