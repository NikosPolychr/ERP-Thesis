using ILOG.CPLEX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.CrewScheduling
{
    public class TreeNode
    {
        public Cplex lp; // cplex pointer to associated master optimization problem
        public double MasterObjVal; // master objective
        public int[] timestocover/*[N + R]*/; // indicates how many times corresponding master row still needs to be covered 
        public int uid; // index number of tree-node
        public bool CGPerformed;  // true when column generation is fully performed for that tree node 
        public int NumberOfNetworkRouteNodes; // contains the number of Rt nodes of the Network of this TreeNode +1 for the fictitious node
        public NetworkNode[] Networklist;
    };
}
