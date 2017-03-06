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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ElectroxFixtureSelectionLib;
using System.Windows.Threading;
using Valutech.Electrox;
using Valutech.Electrox.Data;
using ElectroxProgramsManagmentLib;

namespace ElectroxFixtureSelectionUI
{
    public delegate void SetContentCallback(string content);
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ElectroxFixtureSelection selection = new ElectroxFixtureSelection();

        private LaserEquipmentListManager listManager = LaserEquipmentListManager.GetInstance();

        private ElectroxFixtureSelectionLocalSettings settings = new ElectroxFixtureSelectionLocalSettings();

        private LaserProgramsDataTable laserProgramsTable = new LaserProgramsDataTable();

        private LaserPropertiesDataTable laserPropertiesTable = new LaserPropertiesDataTable();

        private LaserFixtureDataTable laserFixtureDataTable = new LaserFixtureDataTable();

        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;
            this.selection.CommPortChanged += selection_CommPortChanged;
            this.selection.FixtureChanged += selection_FixtureChanged;
            this.selection.ConnectionStatusChanged += selection_ConnectionStatusChanged;
            this.selection.FixtureSelectionChanged += selection_FixtureSelectionChanged;
            this.selection.NoFixtureFound += selection_NoFixtureFound;
            this.selection.InvalidFixture += selection_InvalidFixture;
            this.selection.NoProgramFound += selection_NoProgramFound;
            this.selection.NoPartFound += selection_NoPartFound;
            comPortLabel.Content = selection.CommPort;
            listManager.LaserEquipmentList.ForEach(delegate(LaserEquipment laser) { if (laser.Area != AreasDataTable.IMEI_AREA) { laserComboBox.Items.Add(laser); } });
            listManager.LaserEquipmentList.ForEach(delegate(LaserEquipment laser) { laser.InfoChanged += laser_InfoChanged; laser.PortBusy += laser_PortBusy; });
            laserComboBox.SelectionChanged += laserComboBox_SelectionChanged;
            listManager.LaserEquipmentList.ForEach(delegate(LaserEquipment laser) { if (laser.Id == settings.SelectedLaser) { laserComboBox.SelectedItem = laser; } });
            laserProgramsDataGrid.ItemsSource = laserProgramsTable.DefaultView;
            laserInfoDataGrid.ItemsSource = laserPropertiesTable.DefaultView;
            fixtureInfoDataGrid.ItemsSource = laserFixtureDataTable.DefaultView;
        }

        #region UI Event Handlers

        void laserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateLaserStatus("Connecting");
            LaserEquipment laser = (LaserEquipment) laserComboBox.SelectedItem;
            settings.SelectedLaser = laser.Id;            
            laser.InfoChanged +=laser_InfoChanged;
            laser.LoadPrograms();
            laserPropertiesTable.Laser = laser;
            selection.Laser = laser;            
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            selection.Stop();
        }

        #endregion

        #region Another Thread Event Handlers

        /// <summary>
        /// Handles comm port change
        /// </summary>
        /// <param name="commPort"></param>
        void selection_CommPortChanged(string commPort)
        {            
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(()=>updateCommPort(commPort)));
        }

        void selection_NoFixtureFound()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => setNoFixtureFound()));
        }

        void selection_NoProgramFound()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => setNoProgramFound()));
        }

        void selection_InvalidFixture()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => setInvalidFixture()));
        }

        void selection_NoPartFound()
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => setNoPartFound()));
        }
        
        void selection_FixtureSelectionChanged(ElectroxProgramInfo programInfo)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => updateFixtureSelection(programInfo)));
        }

        void selection_FixtureChanged(int fixture)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => updateFixture(fixture)));
        }

        void selection_ConnectionStatusChanged(bool connected)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => updateConnectionStatus(connected)));
        }

        void laser_InfoChanged(LaserEquipment laser)
        {
            laserProgramsTable.Programs = laser.Programs;
            string status = (laser.Online) ? "Online" : "Offline";
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => updateLaserStatus(status)));
        }

        void laser_PortBusy(LaserEquipment laser)
        {
            laserProgramsTable.Clear();
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => updateLaserStatus("Busy")));
        }

        #endregion

        void updateCommPort(string commPort)
        {
            bool commPortFound = (commPort != string.Empty);
            comPortLabel.Content = (commPortFound)? commPort:"None";
            laserFixtureDataTable.CommPort = (commPortFound) ? commPort : "None";
        }

        void setNoFixtureFound()
        {
            selectedModelLabel.Content = "No Fixture Found";
        }

        void setNoPartFound()
        {
            selectedModelLabel.Content = "No Part Found";
        }

        void setNoProgramFound()
        {
            selectedModelLabel.Content = "No program found on the machine";
        }

        void setInvalidFixture()
        {
            selectedModelLabel.Content = "Invalid Fixture";
        }

        void updateFixture(int fixture)
        {
            laserFixtureDataTable.Fixture = fixture;            
        }

        void updateFixtureSelection(ElectroxProgramInfo program)
        {
            selectedModelLabel.Content = program.FriendlyName;
        }

        void updateConnectionStatus(bool connected)
        {
            laserFixtureDataTable.Connected = connected;
        }

        void updateLaserStatus(string status)
        {
            laserStatusLabel.Text = status;
            laserProgramsDataGrid.Items.Refresh();                     
        }
    }
}
