﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Model.Enums
{
    public class BasicEnums : INotifyPropertyChanged
    {
        #region Inventory Control


        public enum PeriodType
        {
         Monthly,
         Yearly
        }
        public enum Timebucket
        {
            Daily,
            Weekly,
            Monthly,
            Quarterly

        }
        public enum ActivePanel
        {
            None,
            BasicEOQ,
            RefillTimeEOQ,
            PendingOrders,
            LostSales,
            Discount_Large_Orders,
            Multiple_Products_Single_Supplier,
            Multiple_Products_Multiple_Supplier,
            TimeVarying_Infinite_Capacity,
            TimeVarying_Finite_Capacity
        }

        public enum DemandType
        {
            Constant_Demand,
            Time_Varying_Demand,
            Uncertain_Demand

        }
        public enum ConstantDemandType
        {
            Basic_EOQ,
            Refill_Time_EOQ,
            Pending_Orders,
            Lost_Sales,
            Discount_Large_Orders,
            Multiple_Products_Single_Supplier,
            Multiple_Products_Multiple_Supplier,

        }
        public enum TimeVaryingDemandType
        {
            Infinite_Capacity,
            Finite_Capacity

        }
        #endregion

        public enum CustomerType
        {
            Retail,
            Wholesale
        }

        public enum ItemType
        {
            Cement,
            Dust,
            NoType
        }

        public enum Assembly
        {
            Finished,
            SemiFinished,
            RawMaterial
        }
        public enum MachStatus
        {
            Inactive,  // Machine is not currently in use
            Active,    // Machine is currently in use
            Maintenance, // Machine is under maintenance
            Error,     // Machine has encountered an error
            Offline,   // Machine is not connected
            Online     // Machine is connected

        }
        public enum MachType
        {
            Grinding,
            Vertical_Drilling,
            Horizontal_Drilling,
            Boring,
            Planning,
            AssemblyLine,  // Machine used in an assembly line process
            Packaging,     // Machine used for packaging products
            Sorting,       // Machine used for sorting inventory
            QualityControl, // Machines used for testing or quality control
            Conveyor,      // Conveyor systems for moving products
            Loading,       // Machines used for loading goods (like forklift)
            Palletizing,   // Machines used for palletizing goods
            Labelling,     // Machines used for labeling products
            RoboticArm,    // Robotic arm used in various tasks
            CNCMachining,  // Computer numerical control machine for precise tasks
            Welding,       // Machines used for welding tasks
            Printing       // Machines used for printing labels, instructions, etc.
        }
        public enum OrderStatus
        {
            Ready,
            Processing,
            Shipped,
            Delivered,
            Cancelled
            
        }

        public enum Incoterms
        {
            EXW,
            FCA,
            FAS,
            FOB,
            CFR,
            CIF,
            CPT,
            CIP,
            DAP,
            DPU,
            DDP
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
