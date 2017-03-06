using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Valutech.UserManagment;
using Valutech.IO;
using Valutech.Electrox.Data;
using Valutech.Electrox;

namespace Valutech.Electrox
{
    public partial class LaserSelection : Form
    {

        public delegate void Invoker();

        private LaserEquipmentListDataTable table = new LaserEquipmentListDataTable();

        private LaserSelectionManager manager = new LaserSelectionManager();

        private LocalSettingsManager localSettingsManager = LocalSettingsManager.GetInstance();

        private const string ALL_OPTION = "[All]";

        private Timer timer = new Timer();

        private LaserEquipmentPropertiesDataTable propertiesDataTable = new LaserEquipmentPropertiesDataTable();

        /// <summary>
        /// Form Constructor
        /// </summary>
        public LaserSelection()
        {
            InitializeComponent();

            this.plantComboBox.DisplayMember = "Facility";
            this.plantComboBox.DataSource = new Electrox.Data.PlantsDataTable();
            this.areaComboBox.DisplayMember = "Area";
            this.areaComboBox.DataSource = new Electrox.Data.AreasDataTable();

            this.plantComboBox.SelectedIndexChanged += new System.EventHandler(this.plantComboBox_SelectedIndexChanged);
            this.areaComboBox.SelectedIndexChanged += new System.EventHandler(this.areaComboBox_SelectedIndexChanged);
            this.laserEquipmentDataGridView.DoubleClick += laserEquipmentDataGridView_DoubleClick;
            this.propertiesDataTable.RowChanged += propertiesDataTable_RowChanged;
            this.Load += LaserSelection_Load;
            this.Resize += LaserSelection_Resize;
            this.timer.Tick += timer_Tick;

            this.laserEquipmentDataGridView.DataSource = table;
            this.SetComboSelection();

            this.table.Plant = (localSettingsManager.Plant == ALL_OPTION) ? String.Empty : localSettingsManager.Plant;
            this.table.Area = (localSettingsManager.Area == ALL_OPTION) ? String.Empty : localSettingsManager.Area;
            this.table.Refresh();

            this.timer.Interval = 500;
            this.timer.Start();

            this.laserEquipmentDataGridView.SelectionChanged += laserEquipmentDataGridView_SelectionChanged;

            SetListenersForLasers();

            this.propertiesDataGridView.DataSource = propertiesDataTable;            
        }

        void LaserSelection_Resize(object sender, EventArgs e)
        {
            tabControl1.Width = this.Width - this.tabControl1.Left - 20;
            propertiesDataGridView.Width = tabControl1.Width - 14;

        }

        void propertiesDataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!propertiesDataTable.Loading)
            {
                table.Refresh();
                this.laserEquipmentDataGridView.Focus();
            }
        }

        #region UI Event Handlers

        /// <summary>
        /// Handles the form load event to set the initial size of the columns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaserSelection_Load(object sender, EventArgs e)
        {
            laserEquipmentDataGridView.Columns[table.NameColumn.Ordinal].Width = 200;
            laserEquipmentDataGridView.Columns[table.PlantColumn.Ordinal].Width = 80;
            propertiesDataGridView.Columns[LaserEquipmentPropertiesDataTable.PROPERTY_COLUMN].Width = 70;
            
            DataGridViewComboBoxCell plantCombo = new DataGridViewComboBoxCell();
            plantCombo.DataSource = new Valutech.Electrox.Data.PlantsDataTable();
            plantCombo.DisplayMember = Valutech.Electrox.Data.PlantsDataTable.FACILITY;

            DataGridViewComboBoxCell areaCombo = new DataGridViewComboBoxCell();
            areaCombo.DataSource = new Valutech.Electrox.Data.AreasDataTable();
            areaCombo.DisplayMember = Valutech.Electrox.Data.AreasDataTable.AREA_COLUMN;

            DataGridViewComboBoxCell lensCombo = new DataGridViewComboBoxCell();
            lensCombo.DataSource = new Valutech.Electrox.Data.LensDataTable();
            lensCombo.DisplayMember = Valutech.Electrox.Data.LensDataTable.LENS_COLUMN;

            propertiesDataGridView.Rows[1].Cells[1] = plantCombo;
            propertiesDataGridView.Rows[2].Cells[1] = areaCombo;
            propertiesDataGridView.Rows[6].Cells[1] = lensCombo;
            propertiesDataGridView.Columns[LaserEquipmentPropertiesDataTable.PROPERTY_COLUMN].ReadOnly = true;
            propertiesDataGridView.DoubleClick += propertiesDataGridView_DoubleClick;
        }

        #region Handle double click for mru edit

        private void propertiesDataGridView_DoubleClick(object sender, EventArgs e)
        {
            int currentIndex = propertiesDataGridView.Rows[propertiesDataGridView.CurrentCell.RowIndex].Index;
            if (currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru1DataRow) ||
                currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru2DataRow) ||
                currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru3DataRow) ||
                currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru4DataRow) ||
                currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru5DataRow) ||
                currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru6DataRow) ||
                currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru7DataRow) ||
                currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru8DataRow) 
                )
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = (propertiesDataGridView.CurrentCell.Value.ToString() != String.Empty) ? Path.GetDirectoryName(propertiesDataGridView.CurrentCell.Value.ToString()).ToString() : string.Empty;
                dialog.Filter = "BPT files (*.bpt)|*.bpt";
                DialogResult result = dialog.ShowDialog();
                if (currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru1DataRow))
                {
                    propertiesDataTable.mru1DataRow[propertiesDataTable.Columns[LaserEquipmentPropertiesDataTable.VALUE_COLUMN]] = (result == System.Windows.Forms.DialogResult.Cancel) ? String.Empty : dialog.FileName;
                   
                }
                if (currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru2DataRow))
                {
                    propertiesDataTable.mru2DataRow[propertiesDataTable.Columns[LaserEquipmentPropertiesDataTable.VALUE_COLUMN]] = (result == System.Windows.Forms.DialogResult.Cancel) ? String.Empty : dialog.FileName;

                }
                if (currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru3DataRow))
                {
                    propertiesDataTable.mru3DataRow[propertiesDataTable.Columns[LaserEquipmentPropertiesDataTable.VALUE_COLUMN]] = (result == System.Windows.Forms.DialogResult.Cancel) ? String.Empty : dialog.FileName;

                }
                if (currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru4DataRow))
                {
                    propertiesDataTable.mru4DataRow[propertiesDataTable.Columns[LaserEquipmentPropertiesDataTable.VALUE_COLUMN]] = (result == System.Windows.Forms.DialogResult.Cancel) ? String.Empty : dialog.FileName;

                }
                if (currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru5DataRow))
                {
                    propertiesDataTable.mru5DataRow[propertiesDataTable.Columns[LaserEquipmentPropertiesDataTable.VALUE_COLUMN]] = (result == System.Windows.Forms.DialogResult.Cancel) ? String.Empty : dialog.FileName;

                }
                if (currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru6DataRow))
                {
                    propertiesDataTable.mru6DataRow[propertiesDataTable.Columns[LaserEquipmentPropertiesDataTable.VALUE_COLUMN]] = (result == System.Windows.Forms.DialogResult.Cancel) ? String.Empty : dialog.FileName;

                }
                if (currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru7DataRow))
                {
                    propertiesDataTable.mru7DataRow[propertiesDataTable.Columns[LaserEquipmentPropertiesDataTable.VALUE_COLUMN]] = (result == System.Windows.Forms.DialogResult.Cancel) ? String.Empty : dialog.FileName;

                }
                if (currentIndex == propertiesDataTable.Rows.IndexOf(propertiesDataTable.mru8DataRow))
                {
                    propertiesDataTable.mru8DataRow[propertiesDataTable.Columns[LaserEquipmentPropertiesDataTable.VALUE_COLUMN]] = (result == System.Windows.Forms.DialogResult.Cancel) ? String.Empty : dialog.FileName;

                }
            }            
        }

        #endregion

        /// <summary>
        /// Handles the tick of the timer that refresh the table data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            this.laserEquipmentDataGridView.Refresh();            
        }

        /// <summary>
        /// Handle the datagridview double click to select equipment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void laserEquipmentDataGridView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                manager.SelectLaserEquipment(laserEquipmentDataGridView.SelectedRows[0].Cells[0].Value.ToString());
            }
            catch (System.Security.SecurityException ex)
            {
                MessageBox.Show("No se cuenta con privilegios de administrador");
            }
        }

        /// <summary>
        /// Handles the datagridview selection change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void laserEquipmentDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            loadSelectedRowInfo();
        }

        /// <summary>
        /// Handles the button click of the laser selection button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void laserSelectionButton_Click(object sender, EventArgs e)
        {
            manager.SelectLaserEquipment(laserEquipmentDataGridView.SelectedRows[0].Cells[0].Value.ToString());
        }

        /// <summary>
        /// Handles the plant combobox selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void plantComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            localSettingsManager.Plant = plantComboBox.Text;
            table.Plant = (localSettingsManager.Plant == ALL_OPTION) ? String.Empty : localSettingsManager.Plant;
            table.Refresh();
            loadSelectedRowInfo();
        }

        /// <summary>
        /// Handles the area combobox selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void areaComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            localSettingsManager.Area = areaComboBox.Text;
            table.Area = (localSettingsManager.Area == ALL_OPTION) ? String.Empty : localSettingsManager.Area;
            table.Refresh();
            loadSelectedRowInfo();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        private void SetListenersForLasers() {
            foreach (LaserEquipment le in LaserEquipmentListManager.GetInstance().LaserEquipmentList)
            {
                le.InfoChanged += le_LaserEquipmentInfoChanged;
                le.PortBusy += le_PortBusy;
            }
        }

        void le_PortBusy(LaserEquipment laser)
        {
            if (InvokeRequired)
            {
                this.Invoke(new LaserEquipmentInfoChangedHandler(UpdateUIForPortBusy), laser);
            }
        }

        void le_LaserEquipmentInfoChanged(LaserEquipment laser)
        {
            if (InvokeRequired)
            {
                this.Invoke(new LaserEquipmentInfoChangedHandler(UpdateSelectedLaserInfo), laser);
            }
        }

        private void UpdateUIForPortBusy(LaserEquipment laser)
        {
            statusLabel.Text = "Port Busy";
            connectButton.Enabled = true;
            connectButton.Visible = true;
        }

        /// <summary>
        /// Loads the selected row laser data
        /// </summary>
        private void loadSelectedRowInfo()
        {
            try
            {                
                string name = this.laserEquipmentDataGridView.Rows[this.laserEquipmentDataGridView.CurrentCell.RowIndex].Cells[0].Value.ToString();
                LaserEquipment laser = null;
                foreach (LaserEquipment laserEquipment in LaserEquipmentListManager.GetInstance().LaserEquipmentList)
                {
                    if (name == laserEquipment.Name)
                    {
                        laser = laserEquipment;
                    }
                }
                if (laser != null)
                {
                    ConnectToLaser(laser);                    
                }                
            }
            catch
            {
            }
        }

        private void ConnectToLaser(LaserEquipment laser)
        {
            connectButton.Enabled = false;
            connectButton.Visible = false;
            laserNameLabel.Text = laser.Name;
            //laser.Connect();
            statusLabel.Text = "Loading...";
            programsTextBox.Text = String.Empty;
            propertiesDataTable.laserEquipment = laser;
        }

        private void UpdateSelectedLaserInfo(LaserEquipment laser)
        {            
            statusLabel.Text = (laser.Online) ? "Online" : "Offline";
            hardwareInfoLabel.Text = laser.Hardware;
            programsTextBox.Text = String.Empty;
            foreach (LaserProgram program in laser.Programs) programsTextBox.Text += program + Environment.NewLine;            
        }

        /// <summary>
        /// Set the combo selection based on the local settings value
        /// </summary>
        private void SetComboSelection()
        {
            plantComboBox.Text = localSettingsManager.Plant;
            areaComboBox.Text = localSettingsManager.Area;
        }

        #endregion

        private void connectButton_Click(object sender, EventArgs e)
        {
            ConnectToLaser(propertiesDataTable.laserEquipment);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to copy settings from current selected laser profile?", "Add Laser Equipment", MessageBoxButtons.YesNo);            
            LaserEquipment laser = new LaserEquipment();
            laser.Plant = (localSettingsManager.Plant == ALL_OPTION) ? String.Empty : localSettingsManager.Plant;
            laser.Area = (localSettingsManager.Area == ALL_OPTION) ? String.Empty : localSettingsManager.Area;
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                laser.Ip = propertiesDataTable.laserEquipment.Ip;
                laser.Dsp = propertiesDataTable.laserEquipment.Dsp;
                laser.DspFile = propertiesDataTable.laserEquipment.DspFile;
                laser.Lens = propertiesDataTable.laserEquipment.Lens;
                laser.LensAmp = propertiesDataTable.laserEquipment.LensAmp;
                laser.XCompensation = propertiesDataTable.laserEquipment.XCompensation;
                laser.YCompensation = propertiesDataTable.laserEquipment.YCompensation;
            }
            LaserEquipmentListManager.GetInstance().AddLaserEquipment(laser);
            table.Refresh();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete this registry?", "Delete Registry", MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                LaserEquipmentListManager.GetInstance().DeleteLaserEquipment(propertiesDataTable.laserEquipment);
                table.Refresh();                
            }
        }



    }
}
