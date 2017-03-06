using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;

namespace Valutech.Electrox
{
    class LaserEquipmentPropertiesDataTable:DataTable
    {
        #region Constants

        public const string PROPERTY_COLUMN = "Property";
        public const string VALUE_COLUMN = "Value";

        #endregion

        public DataRow nameDataRow;
        public DataRow plantDataRow;
        public DataRow areaDataRow;
        public DataRow ipDataRow;
        public DataRow dspNameDataRow;
        public DataRow dspFileDataRow;
        public DataRow lensDataRow;
        public DataRow lensAmpDataRow;
        public DataRow xCompensationDataRow;
        public DataRow yCompensationDataRow;
        public DataRow mru1DataRow;
        public DataRow mru2DataRow;
        public DataRow mru3DataRow;
        public DataRow mru4DataRow;
        public DataRow mru5DataRow;
        public DataRow mru6DataRow;
        public DataRow mru7DataRow;
        public DataRow mru8DataRow;

        private LaserEquipment _laserEquipment;
        private bool loading = false;

        public LaserEquipmentPropertiesDataTable()
        {            
            this.Columns.Add(PROPERTY_COLUMN, typeof(string));
            this.Columns.Add(VALUE_COLUMN, typeof(string));

            nameDataRow = this.NewRow();
            nameDataRow[this.Columns[PROPERTY_COLUMN]] = "Name";

            plantDataRow = this.NewRow();
            plantDataRow[this.Columns[PROPERTY_COLUMN]] = "Plant";
            

            areaDataRow = this.NewRow();
            areaDataRow[this.Columns[PROPERTY_COLUMN]] = "Area";

            ipDataRow = this.NewRow();
            ipDataRow[this.Columns[PROPERTY_COLUMN]] = "Ip";

            dspNameDataRow = this.NewRow();
            dspNameDataRow[this.Columns[PROPERTY_COLUMN]] = "Dsp Name";

            dspFileDataRow = this.NewRow();
            dspFileDataRow[this.Columns[PROPERTY_COLUMN]] = "Dsp File";

            lensDataRow = this.NewRow();
            lensDataRow[this.Columns[PROPERTY_COLUMN]] = "Lens";

            lensAmpDataRow = this.NewRow();
            lensAmpDataRow[this.Columns[PROPERTY_COLUMN]] = "Lens Amp";

            xCompensationDataRow = this.NewRow();
            xCompensationDataRow[this.Columns[PROPERTY_COLUMN]] = "X Comp";

            yCompensationDataRow = this.NewRow();
            yCompensationDataRow[this.Columns[PROPERTY_COLUMN]] = "Y Comp";

            mru1DataRow = this.NewRow();
            mru1DataRow[this.Columns[PROPERTY_COLUMN]] = "Mru 1";            

            mru2DataRow = this.NewRow();
            mru2DataRow[this.Columns[PROPERTY_COLUMN]] = "Mru 2";

            mru3DataRow = this.NewRow();
            mru3DataRow[this.Columns[PROPERTY_COLUMN]] = "Mru 3";            

            mru4DataRow = this.NewRow();
            mru4DataRow[this.Columns[PROPERTY_COLUMN]] = "Mru 4";
            
            mru5DataRow = this.NewRow();
            mru5DataRow[this.Columns[PROPERTY_COLUMN]] = "Mru 5";
                        
            mru6DataRow = this.NewRow();
            mru6DataRow[this.Columns[PROPERTY_COLUMN]] = "Mru 6";
            
            mru7DataRow = this.NewRow();
            mru7DataRow[this.Columns[PROPERTY_COLUMN]] = "Mru 7";
            
            mru8DataRow = this.NewRow();
            mru8DataRow[this.Columns[PROPERTY_COLUMN]] = "Mru 8";
            
            this.Rows.Add(nameDataRow);
            this.Rows.Add(plantDataRow);
            this.Rows.Add(areaDataRow);
            this.Rows.Add(ipDataRow);
            this.Rows.Add(dspNameDataRow);
            this.Rows.Add(dspFileDataRow);
            this.Rows.Add(lensDataRow);
            this.Rows.Add(lensAmpDataRow);
            this.Rows.Add(xCompensationDataRow);
            this.Rows.Add(yCompensationDataRow);
            this.Rows.Add(mru1DataRow);
            this.Rows.Add(mru2DataRow);
            this.Rows.Add(mru3DataRow);
            this.Rows.Add(mru4DataRow);
            this.Rows.Add(mru5DataRow);
            this.Rows.Add(mru6DataRow);
            this.Rows.Add(mru7DataRow);
            this.Rows.Add(mru8DataRow);

            this.RowChanged += LaserEquipmentPropertiesDataTable_RowChanged;
        }

        void LaserEquipmentPropertiesDataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (!loading)
            {
                _laserEquipment.Name = this.nameDataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.Plant = this.plantDataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.Area = this.areaDataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.Ip = this.ipDataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.Dsp = this.dspNameDataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.DspFile = this.dspFileDataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.Lens = this.lensDataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.LensAmp = this.lensAmpDataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.XCompensation = this.xCompensationDataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.YCompensation = this.yCompensationDataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.Mru1= this.mru1DataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.Mru2 = this.mru2DataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.Mru3 = this.mru3DataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.Mru4 = this.mru4DataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.Mru5 = this.mru5DataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.Mru6 = this.mru6DataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.Mru7 = this.mru7DataRow[this.Columns[VALUE_COLUMN]].ToString();
                _laserEquipment.Mru8 = this.mru8DataRow[this.Columns[VALUE_COLUMN]].ToString();
                LaserEquipmentListManager.GetInstance().SaveLaserSelectionData(this._laserEquipment);
            }
        }

        public string Mru1
        {
            set
            {
                _laserEquipment.Mru1 = value;
                LaserEquipmentListManager.GetInstance().SaveLaserSelectionData(this._laserEquipment);
            }
        }

        public LaserEquipment laserEquipment
        {
            set
            {
                this._laserEquipment = value;
                this.loading = true;
                this.nameDataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.Name;
                this.plantDataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.Plant;
                this.areaDataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.Area;
                this.ipDataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.Ip;
                this.dspNameDataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.Dsp;
                this.dspFileDataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.DspFile;
                this.lensDataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.Lens;
                this.lensAmpDataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.LensAmp;
                this.xCompensationDataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.XCompensation;
                this.yCompensationDataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.YCompensation;
                this.mru1DataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.Mru1;
                this.mru2DataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.Mru2;
                this.mru3DataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.Mru3;
                this.mru4DataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.Mru4;
                this.mru5DataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.Mru5;
                this.mru6DataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.Mru6;
                this.mru7DataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.Mru7;
                this.mru8DataRow[this.Columns[VALUE_COLUMN]] = _laserEquipment.Mru8;
                this.loading = false;
            }
            get
            {
                return this._laserEquipment;
            }
        }

        public bool Loading
        {
            get
            {
                return this.loading;
            }
        }

    }
}
