using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Erp.ViewModel;
using MaterialDesignThemes.Wpf;
using Erp.View.BasicFiles;
using Erp.Model;
using Erp.View.Suppliers;
using Erp.View.Manufacture;
using Erp.View.Inventory;
using Erp.View.SupplyChain;
using Erp.ViewModel.Data_Analytics;
using Erp.View.Data_Analytics;
using Erp.DataBase;
using Microsoft.EntityFrameworkCore;
using Erp.View.Customers;
using System.ComponentModel;
using Erp.View.Schedules;
using System.Collections.ObjectModel;
using MahApps.Metro.Controls;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Data.Extensions;
using Erp.View.Thesis;
using Erp.Model.Thesis;
using MaterialDesignColors;
using Erp.ViewSearch;
using Erp.ViewModelSearch.Thesis;
using Erp.ViewModel.Thesis;
using Erp.ViewModel.BasicFiles;

namespace Erp.View
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView  : Window, INotifyPropertyChanged
    {
        private MainViewModel _mainViewModel;

        public void INotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region Buttons SubItems  
        private List<SubItem> subItems;

        public List<SubItem> SubItems
        {
            get { return subItems; }
            set
            {
                subItems = value;
                INotifyPropertyChanged(nameof(SubItems));
            }
        }

        private List<SubItem> subItemsSearch;

        public List<SubItem> SubItemsSearch
        {
            get { return subItemsSearch; }
            set
            {
                subItemsSearch = value;
                INotifyPropertyChanged(nameof(SubItemsSearch));
            }
        }
        #endregion

        public MainView()
        {
            InitializeComponent();
            this.Height = SystemParameters.PrimaryScreenHeight * 0.70;
            this.Width = SystemParameters.PrimaryScreenWidth * 0.70;

            SubItems = new List<SubItem>();
            SubItemsSearch = new List<SubItem>();
            DataContext = new MainViewModel();

            #region BasicFiles
            var menuBasic = new List<SubItem>();
            var menuBasicSearch = new List<SubItem>();

            menuBasic.Add(new SubItem("Products", () => new ItemView(), searchKey: "Products"));
            menuBasic.Add(new SubItem("Countries", () => new CountryView(), searchKey: "Countries"));
            menuBasic.Add(new SubItem("Prefectures", () => new PrefectureView(), searchKey: "Prefectures"));
            menuBasic.Add(new SubItem("Cities", () => new CityView(), searchKey: "Cities"));
            menuBasic.Add(new SubItem("Routes", () => new RoutesView(), searchKey: "Routes"));

            var BasicFiles = new ItemMenu("Basic Files", menuBasic, PackIconKind.Schedule);
            #endregion

            #region Customers
            var menuCustomers = new List<SubItem>();
            var menuCustomersSearch = new List<SubItem>();

            menuCustomers.Add(new SubItem("Customers", () => new CustomerView(), searchKey: "Customers"));
            menuCustomers.Add(new SubItem("Customer Price Catalogs", () => new PriceListView(), searchKey: "Customer Price Catalogs"));
            menuCustomers.Add(new SubItem("Customer Orders", () => new CustomerOrderView(), searchKey: "Customer Orders"));

            var Customers = new ItemMenu("Customers", menuCustomers, PackIconKind.Register);
            #endregion

            #region Vendors
            var menuVendors = new List<SubItem>();
            var menuVendorsSearch = new List<SubItem>();

            menuVendors.Add(new SubItem("Vendors", () => new SupplierInfoView(), searchKey: "Vendors"));

            // If you later add a vendor search screen:
            // menuVendorsSearch.Add(new SubItem("Vendors Search", () => new VendorSearchView(), () => new VendorSearchPopupView(), searchKey: "Vendors"));

            var Vendors = new ItemMenu("Vendors", menuVendors, PackIconKind.Schedule);
            #endregion

            #region Inventory
            var menuInv = new List<SubItem>();
            var menuInvSearch = new List<SubItem>();

            menuInv.Add(new SubItem("Inventories/Stock", () => new InventoryView(), searchKey: "Inventories/Stock"));
            var Inventory = new ItemMenu("Inventory", menuInv, PackIconKind.Schedule);
            #endregion

            #region Productions
            var menuProd = new List<SubItem>();
            var menuProdSearch = new List<SubItem>();

            menuProd.Add(new SubItem("Machines", () => new MachineView(), searchKey: "Machines"));
            menuProd.Add(new SubItem("Maintenance Schedule", () => new MachMaintenaceView(), searchKey: "Maintenance Schedule"));
            menuProd.Add(new SubItem("MPS", () => new MPSView(), searchKey: "MPS"));
            menuProd.Add(new SubItem("MRP", () => new MRPView(), searchKey: "MRP"));
            menuProd.Add(new SubItem("MRP Results", () => new MRPResultsView(), searchKey: "MRP Results"));

            var Production = new ItemMenu("Production", menuProd, PackIconKind.Schedule);
            #endregion

            #region SupplyChain
            var menuSpCh = new List<SubItem>();
            var menuSpChSearch = new List<SubItem>();

            menuSpCh.Add(new SubItem("Clustering TSP", () => new Clustering_Vrp_View(), searchKey: "Clustering TSP"));
            var SupChain = new ItemMenu("Supply Chain", menuSpCh, PackIconKind.Schedule);
            #endregion

            #region Data Analysis
            var menuData = new List<SubItem>();
            var menuDataSearch = new List<SubItem>();

            menuData.Add(new SubItem("Forecast Demand", () => new ForecastDemandView(), searchKey: "Forecast Demand"));
            var DataAnalysis = new ItemMenu("Data Analysis", menuData, PackIconKind.Schedule);
            #endregion

            #region Schedules
            var menuScdles = new List<SubItem>();
            var menuScdlesSearch = new List<SubItem>();

            menuScdles.Add(new SubItem("Project Management Schedule", () => new PrManagementScheduleView(), searchKey: "Project Management Schedule"));
            var Schedules = new ItemMenu("Schedules", menuScdles, PackIconKind.Schedule);
            #endregion

            #region Diplwmatikh Kozanidhs
            var menuSetup = new List<SubItem>();
            var menuFlights = new List<SubItem>();
            var menuFlightsSearch = new List<SubItem>();

   
            menuSetup.Add(new SubItem("Countries", () => new CountryView(), searchKey: "Countries",
                                 viewModelFactory: () => new CountryViewModel()));
            menuSetup.Add(new SubItem("Prefectures", () => new PrefectureView(), searchKey: "Prefectures",
                                             viewModelFactory: () => new PrefectureViewModel()));
            menuSetup.Add(new SubItem("Cities", () => new CityView(), searchKey: "Cities",
                                             viewModelFactory: () => new CityViewModel()));
            menuSetup.Add(new SubItem("Crew Categories", () => new CrewCategView(), searchKey: "CrewCategories",
                        viewModelFactory: () => new CrewCategViewModel()));

            var SetupThesis = new ItemMenu("Setup Menu", menuSetup, PackIconKind.Schedule);

            menuFlights.Add(new SubItem("Employees", () => new EmployeeView(), searchKey: "Employees",
                                             viewModelFactory: () => new EmployeeViewModel()));
            menuFlights.Add(new SubItem("Requirements Schedule", () => new RequirementsScheduleView(), searchKey: "Requirements Schedule", 
                                             viewModelFactory: () => new RequirementsScheduleViewModel()));
            menuFlights.Add(new SubItem("Vacation Planning", () => new VacationsPlanningView(), searchKey: "Vacation Planning",
                                             viewModelFactory: () => new VacationsPlanningViewModel()));

            var VPThesis = new ItemMenu("Vacation Planning Menu", menuFlights, PackIconKind.Schedule);

            var menuFlights2 = new List<SubItem>();
            var menuFlights2Search = new List<SubItem>();


            menuFlights2.Add(new SubItem("Airports", () => new AirportsView(), searchKey: "Airports",
                                             viewModelFactory: () => new AirportsViewmodel()));
            menuFlights2.Add(new SubItem("Flight Legs", () => new FlightLegsView(), searchKey: "Flight Legs",
                                             viewModelFactory: () => new FlightLegsViewModel()));
            menuFlights2.Add(new SubItem("Flight Routes", () => new FlightRoutesView(), searchKey: "Flight Routes",
                                             viewModelFactory: () => new FlightRoutesViewModel()));

            menuFlights2.Add(new SubItem("Min Max", () => new MinMaxView(), searchKey: "MinMax Constraints",
                                 viewModelFactory: () => new MinMaxViewModel()));


            menuFlights2.Add(new SubItem("With Without", () => new WithWithoutView(), searchKey: "WithWithout Constraints",
                                 viewModelFactory: () => new WithWithoutViewModel()));

            menuFlights2.Add(new SubItem("Optimizer Settings", () => new OptimizerSettingsView(), searchKey: "Optimizer Settings",
                                             viewModelFactory: () => new OptimizerSettingsViewModel()));

            menuFlights2.Add(new SubItem("Crew Rostering", () => new CrewSchedulingView(), searchKey: "Crew Rostering",
                                             viewModelFactory: () => new CrewSchedulingViewModel()));

            // Search entry for Airports
            menuFlights2Search.Add(new SubItem(
                "Airports Search",
                () => new AirportsSearchView(),
                () => new AirportSearchPopupView(),
                searchKey: "Airports",
                viewModelFactory: () => new AirportSearchViewModel()
            ));
            var CSThesis = new ItemMenu("Crew Rostering Menu", menuFlights2, PackIconKind.Schedule);
            #endregion

            #region SubItems
            // Register all SubItems
            SubItems.AddRange(menuBasic);
            SubItems.AddRange(menuCustomers);
            SubItems.AddRange(menuInv);
            SubItems.AddRange(menuProd);
            SubItems.AddRange(menuData);
            SubItems.AddRange(menuSpCh);

            SubItems.AddRange(menuSetup);
            SubItems.AddRange(menuFlights);
            SubItems.AddRange(menuFlights2);

            // Register all Search SubItems
            SubItemsSearch.AddRange(menuBasicSearch);
            SubItemsSearch.AddRange(menuCustomersSearch);
            SubItemsSearch.AddRange(menuInvSearch);
            SubItemsSearch.AddRange(menuProdSearch);
            SubItemsSearch.AddRange(menuDataSearch);
            SubItemsSearch.AddRange(menuSpChSearch);


            SubItemsSearch.AddRange(menuFlightsSearch);
            SubItemsSearch.AddRange(menuFlights2Search);

            #endregion


            // Add menus to UI
            //Menu.Children.Add(new UserControlMenuItem(BasicFiles, this));
            //Menu.Children.Add(new UserControlMenuItem(Customers, this));
            //Menu.Children.Add(new UserControlMenuItem(Inventory, this));
            //Menu.Children.Add(new UserControlMenuItem(Production, this));
            //Menu.Children.Add(new UserControlMenuItem(SupChain, this));
            //Menu.Children.Add(new UserControlMenuItem(DataAnalysis, this));
            Menu.Children.Add(new UserControlMenuItem(SetupThesis, this));
            Menu.Children.Add(new UserControlMenuItem(VPThesis, this));
            Menu.Children.Add(new UserControlMenuItem(CSThesis, this));
        }


        //public MainView()
        //{

        //    InitializeComponent();
        //    //this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        //    this.Height = SystemParameters.PrimaryScreenHeight * 0.70;
        //    this.Width = SystemParameters.PrimaryScreenWidth * 0.70;


        //    SubItems = new List<SubItem>();
        //    SubItemsSearch = new List<SubItem>();

        //    DataContext = new MainViewModel();


        //    #region BasicFiles

        //    var menuBasic = new List<SubItem>();
        //    var menuBasicSearch = new List<SubItem>();

        //    menuBasic.Add(new SubItem("Products", new ItemView()));
        //    menuBasic.Add(new SubItem("Countries", new CountryView()));
        //    menuBasic.Add(new SubItem("Prefectures", new PrefectureView()));
        //    menuBasic.Add(new SubItem("Cities", new CityView()));
        //    menuBasic.Add(new SubItem("Routes", new RoutesView()));

        //    var BasicFiles = new ItemMenu("Basic Files", menuBasic, PackIconKind.Schedule);

        //    #endregion

        //    #region Customers
        //    var menuCustomers = new List<SubItem>();
        //    var menuCustomersSearch = new List<SubItem>();

        //    menuCustomers.Add(new SubItem("Customers", new CustomerView()));
        //    menuCustomers.Add(new SubItem("Customer Price Catalogs", new PriceListView()));
        //    menuCustomers.Add(new SubItem("Customer Orders", new CustomerOrderView()));

        //    var Customers = new ItemMenu("Customers", menuCustomers, PackIconKind.Register);
        //    #endregion

        //    #region Vendors


        //    var menuVendors = new List<SubItem>();
        //    var menuVendorsSearch = new List<SubItem>();

        //    menuVendors.Add(new SubItem("Vendors", new SupplierInfoView()));
        //    menuVendors.Add(new SubItem("Vendors Search"));
        //    var Vendors = new ItemMenu("Vendors", menuVendors, PackIconKind.Schedule);

        //    #endregion

        //    #region Inventory

        //    var menuInv = new List<SubItem>();
        //    var menuInvSearch = new List<SubItem>();

        //    menuInv.Add(new SubItem("Inventories/Stock", new InventoryView()));
        //    //menuInv.Add(new SubItem("Inventory Control", new InventoryControlView()));
        //    ////menuInv.Add(new SubItem("Inventory optimization", new GurobiView()));
        //    //menuInv.Add(new SubItem("Inventory Diagrams", new InvVisualisationView()));

        //    var Inventory = new ItemMenu("Inventory", menuInv, PackIconKind.Schedule);

        //    #endregion

        //    #region Productions

        //    var menuProd = new List<SubItem>();
        //    var menuProdSearch = new List<SubItem>();

        //    menuProd.Add(new SubItem("Machines", new MachineView()));
        //    menuProd.Add(new SubItem("Maintenance Schedule", new MachMaintenaceView()));
        //    menuProd.Add(new SubItem("MPS", new MPSView()));
        //    menuProd.Add(new SubItem("MRP", new MRPView()));
        //    menuProd.Add(new SubItem("MRP Results", new MRPResultsView()));
        //    //menuProd.Add(new SubItem("Gantt Diagram",new GanttChartView()));
        //    var Production = new ItemMenu("Production", menuProd, PackIconKind.Schedule);

        //    #endregion

        //    #region SupplyChain

        //    var menuSpCh = new List<SubItem>();
        //    var menuSpChSearch = new List<SubItem>();

        //    menuSpCh.Add(new SubItem("Clustering TSP", new Clustering_Vrp_View()));
        //    //menuSpCh.Add(new SubItem("VRP", new VehicleRoutingView()));
        //    //menuSpCh.Add(new SubItem("SupplyChainViewModel", new SupplyChainView()));
        //    //menuSpCh.Add(new SubItem("VrpVisualisation" , new VrpVisView()));
        //    var SupChain = new ItemMenu("Supply Chain", menuSpCh, PackIconKind.Schedule);

        //    #endregion

        //    #region Data Analysis

        //    var menuData = new List<SubItem>();
        //    var menuDataSearch = new List<SubItem>();

        //    menuData.Add(new SubItem("Forecast Demand", new ForecastDemandView()));

        //    //menuData.Add(new SubItem("MrpVisualisations", new MRPVisualisationView()));
        //    //menuData.Add(new SubItem("Sales Analysis", new SalesDashBoardView()));
        //    var DataAnalysis = new ItemMenu("Data Analysis", menuData, PackIconKind.Schedule);

        //    #endregion

        //    #region Schedules


        //    var menuScdles = new List<SubItem>();
        //    var menuScdlesSearch = new List<SubItem>();

        //    menuScdles.Add(new SubItem("Project Management Schedule", new PrManagementScheduleView()));
        //    var Schedules = new ItemMenu("Schedules", menuScdles, PackIconKind.Schedule);
        //    #endregion

        //    #region Diplwmatikh Kozanidhs

        //    var menuFlights = new List<SubItem>();
        //    var menuFlightsSearch = new List<SubItem>();

        //    //menuFlights.Add(new SubItem("Languages", new LanguageView()));
        //    menuFlights.Add(new SubItem("Employees", new EmployeeView()));
        //    menuFlights.Add(new SubItem("Requirements Schedule", new RequirementsScheduleView()));
        //    menuFlights.Add(new SubItem("Vacation Planning", new VacationsPlanningView()));

        //    var VPThesis = new ItemMenu("Vacation Planning Menu", menuFlights, PackIconKind.Schedule);


        //    var menuFlights2 = new List<SubItem>();
        //    var menuFlights2Search = new List<SubItem>();

        //    menuFlights2.Add(new SubItem("Countries", new CountryView()));
        //    menuFlights2.Add(new SubItem("Prefectures", new PrefectureView()));
        //    menuFlights2.Add(new SubItem("Cities", new CityView()));
        //    menuFlights2.Add(new SubItem("Airports", new AirportsView()));
        //    menuFlights2.Add(new SubItem("Flight Legs", new FlightLegsView()));
        //    menuFlights2.Add(new SubItem("Flight Routes", new FlightRoutesView()));
        //    menuFlights2.Add(new SubItem("Crew Rostering", new CrewSchedulingView()));

        //    menuFlights2Search.Add(new SubItem("Airports Search", new AirportsSearchView(), new AirportSearchPopupView()));

        //    var CSThesis = new ItemMenu("Crew Rostering Menu", menuFlights2, PackIconKind.Schedule);
        //    CSThesis.SubItems.Concat(menuFlights2Search);

        //    #endregion

        //    SubItems = SubItems.Concat(menuBasic).ToList();
        //    SubItems = SubItems.Concat(menuCustomers).ToList();
        //    SubItems = SubItems.Concat(menuInv).ToList();
        //    SubItems = SubItems.Concat(menuProd).ToList();
        //    SubItems = SubItems.Concat(menuData).ToList();
        //    SubItems = SubItems.Concat(menuSpCh).ToList();
        //    SubItems = SubItems.Concat(menuFlights).ToList();

        //    SubItemsSearch = SubItemsSearch.Concat(menuBasicSearch).ToList();
        //    SubItemsSearch = SubItemsSearch.Concat(menuCustomersSearch).ToList();
        //    SubItemsSearch = SubItemsSearch.Concat(menuInvSearch).ToList();
        //    SubItemsSearch = SubItemsSearch.Concat(menuProdSearch).ToList();
        //    SubItemsSearch = SubItemsSearch.Concat(menuDataSearch).ToList();
        //    SubItemsSearch = SubItemsSearch.Concat(menuSpChSearch).ToList();
        //    SubItemsSearch = SubItemsSearch.Concat(menuFlights2Search).ToList();

        //    Menu.Children.Add(new UserControlMenuItem(BasicFiles, this));
        //    Menu.Children.Add(new UserControlMenuItem(Customers, this));
        //    //Menu.Children.Add(new UserControlMenuItem(Schedules, this));
        //    //Menu.Children.Add(new UserControlMenuItem(Vendors, this));
        //    Menu.Children.Add(new UserControlMenuItem(Inventory, this));
        //    Menu.Children.Add(new UserControlMenuItem(Production, this));
        //    Menu.Children.Add(new UserControlMenuItem(SupChain, this));
        //    Menu.Children.Add(new UserControlMenuItem(DataAnalysis, this));
        //    Menu.Children.Add(new UserControlMenuItem(VPThesis, this));
        //    Menu.Children.Add(new UserControlMenuItem(CSThesis, this));



        //}

        public void ClearSelectionExcept(UserControlMenuItem selectedItem)
        {
            foreach (var menuItem in Menu.Children.OfType<UserControlMenuItem>())
            {
                if (menuItem != selectedItem)
                {
                    menuItem.ClearSelection();
                }
            }
        }

        private void TabControlExt_OnCloseButtonClick(object sender, CloseTabEventArgs e)
        {

            var selectedTab = e.TargetTabItem;

            MyTabControl.Items.Remove(selectedTab);

        }
        private void TabControlExt_OnCloseAllTabs(object sender, CloseTabEventArgs e)
        {
            for (int i = MyTabControl.Items.Count - 1; i >= 0; i--)
            {
                MyTabControl.Items.RemoveAt(i);
            }
        }

        private void TabControlExt_OnCloseOtherTabs(object sender, CloseTabEventArgs e)
        {
            var selectedTab = e.TargetTabItem;

            for (int i = MyTabControl.Items.Count - 1; i >= 0; i--)
            {
                var item = MyTabControl.Items[i];
                if (item != selectedTab)
                {
                    MyTabControl.Items.RemoveAt(i);
                }
            }
        }

        internal void SwitchScreen2(object sender, string HeaderText)
        {
            var screen = ((UserControl)sender);

            if (screen != null)
            {
                // Check if the screen is already in the tabs list
                var existingTab = MyTabControl.Items.OfType<TabItem>().FirstOrDefault(tab => tab.Content == screen);

                if (existingTab != null)
                {
                    // The screen is already in the tabs list, so select it
                    MyTabControl.SelectedItem = existingTab;

                }
                else
                {

                    var TabInput = new TabItemExt();

                    TabInput.Header = HeaderText;
                    TabInput.Header = new TextBlock
                    {
                        Text = HeaderText,
                        FontWeight = FontWeights.Bold, 
                        FontSize = 15,
                        FontFamily = new FontFamily("Segoe UI")// Set FontWeight to Bold
                    };

                    TabInput.FontSize = 14;
                    TabInput.FontFamily = new FontFamily("Segoe UI");

                    TabInput.Content = screen;

                    MyTabControl.SelectedItem = TabInput;

                    MyTabControl.Items.Add(TabInput);
                }

            }
          
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }

        private void pnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else this.WindowState = WindowState.Normal;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {

        }

    }
}
