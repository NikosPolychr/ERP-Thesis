using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.CrewScheduling
{
    public class NetworkNode
    {
        public int Rtind { get; set; }
        public int Index { get; set; }
        public int Team { get; set; }
        public int[] PrevRejected { get; set; }
        public NetworkNode[] Prev { get; set; }
        public int[] PrevPath { get; set; }
        public int[] IdDaysOff { get; set; }
        public int[] FltHrsSinceLastRest { get; set; }
        public double[] RC { get; set; }
        public double[] DualSum { get; set; }
        public int[] FlightHrsSum { get; set; }
        public NetworkNode RightNode { get; set; }
    }
}
