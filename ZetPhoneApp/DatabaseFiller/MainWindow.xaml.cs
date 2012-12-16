using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DatabaseFiller.Properties;

namespace DatabaseFiller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DAO _dao;

        protected IEnumerable<Vehicle> VehicleSet;
        protected IEnumerable<Station> StationSet;
        protected IEnumerable<StationVehicleContext> StationVehicleSet;
        protected IEnumerable<DepartureTimeContext> DepartureSet;

        public MainWindow()
        {
            var id = (int)Settings.Default["Id"];

            while(id == -1)
            {
                new Dialog().ShowDialog();
                id = (int)Settings.Default["Id"];
            }


            InitializeComponent();
            ConnectToDatabase();
            LoadData();
            InitDataGrids();

        }

        private void ConnectToDatabase()
        {
            const string fileName = "ZetkoDatabase";
            var con = new SqlCeConnection(@"Data Source=|DataDirectory|\" + fileName + ".sdf");
            _dao = new DAO(con);
            _dao.OpenConnection();
        }

        private void LoadData()
        {
            VehicleSet = _dao.FetchVehicles();
            StationSet = _dao.FetchStations();
            StationVehicleSet = _dao.FetchStationVehicleContextList();
            DepartureSet = _dao.FetchDepartureTimeContextList();

            InitDataGrids();

        }

        private void InitDataGrids()
        {
            Vehicle_Grid.ItemsSource = VehicleSet;
            Station_Grid.ItemsSource = StationSet;
            StationVehicle_Grid.ItemsSource = StationVehicleSet;
            DepartureTime_Grid.ItemsSource = DepartureSet;

        }


        #region Events

        private void AddStationVehicleButtonClick(object sender, RoutedEventArgs e)
        {
            int errorNum = 0;
            string errorMessage = String.Empty;


            string lineNumberText = StationVehicle_LineNumber.Text;
            int voziloId = 0;

            try
            {
                int lineNumber = int.Parse(lineNumberText);
                voziloId = _dao.FetchVehicle(lineNumber).Id;
            }
            catch
            {
                errorMessage += ++errorNum + ": Error parsing 'Broj linije' field.\n";
            }

            string indexText = StationVehicle_Index.Text;
            int index = 0;

            try
            {
                index = int.Parse(indexText);
            }
            catch
            {
                errorMessage += ++errorNum + ": Error parsing 'Redni broj' field.\n";
            }

            string stationIdText = StationVehicle_StanicaId.Text;
            int stationId = 0;

            try
            {
                stationId = int.Parse(stationIdText);
                if (!_dao.ContainsStation(stationId))
                {
                    MessageBox.Show("Error. Station with 'Stanica Id' = " + stationId + " doesn't exist in database.");
                    return;
                }
            }
            catch 
            {
                errorMessage += ++errorNum + ": Error parsing 'Stanica Id' field.\n";
            }

            
            string polazisteIdText = StationVehicle_PolazisteId.Text;
            int polazisteId = 0;

            try
            {
                polazisteId = int.Parse(polazisteIdText);
                if (!_dao.ContainsStation(polazisteId))
                {
                    MessageBox.Show("Error. Station with 'Polaziste Id' = " + polazisteId + " doesn't exist in database.");
                    return;
                }
            }
            catch 
            {
                errorMessage += ++errorNum + ": Error parsing 'Polaziste Id' field.\n";
            }

            
            string timeOffsetText = StationVehicle_TimeOffset.Text;
            int timeOffset = 0;

            try
            {
                timeOffset = int.Parse(timeOffsetText);
            }
            catch
            {
                errorMessage += ++errorNum + ": Error parsing 'TimeOffset' field.\n";
            }


            if(errorNum > 0)
            {
                MessageBox.Show(errorMessage);
            }

            else
            {
                _dao.AddStationVehicleContext(new StationVehicleContext(stationId, voziloId, polazisteId, timeOffset, index));
                LoadData();

            }

        }

        private void AddDepartureTimeButtonClick(object sender, RoutedEventArgs e)
        {
            int errorNum = 0;
            string errorMessage = String.Empty;


            string dan = String.Empty;
            if (DepartureTime_Radni.IsChecked == true)
            {
                dan = "Radni";
            }
            else if (DepartureTime_Subota.IsChecked == true)
            {
                dan = "Subota";
            }
            else if (DepartureTime_Nedjelja.IsChecked == true)
            {
                dan = "Nedjelja";
            }
            else
            {
                errorMessage += ++errorNum + ": Error, select vehicle type.\n";
            }

            string lineNumberText = DepartureTime_LineNumber.Text;
            int voziloId = 0;

            try
            {
                int lineNumber = int.Parse(lineNumberText);
                if (!_dao.ContainsVehicle(lineNumber))
                {
                    MessageBox.Show("Error. Database doesn't contain vehicle with line number = " + lineNumber + ".");
                    return;
                }
                voziloId = _dao.FetchVehicle(lineNumber).Id;
            }
            catch 
            {
                errorMessage += ++errorNum + ": Error parsing 'Broj linije' field.\n";
            }

            string polazisteIdText = DepartureTime_PolazisteId.Text;
            int polazisteId = 0;

            try
            {
                polazisteId = int.Parse(polazisteIdText);
                if (!_dao.ContainsStation(polazisteId))
                {
                    MessageBox.Show("Error. Station with 'Polaziste Id' = " + polazisteId + " doesn't exist in database.");
                    return;
                }
            }
            catch 
            {
                errorMessage += ++errorNum + ": Error parsing 'Polaziste Id' field.\n";
            }

            


            string time = DepartureTime_Time.Text;

            if (time == null || time.Trim().Equals(""))
            {
                errorMessage += ++errorNum + ": Error parsing 'Time' field.\n";
            }


            if (errorNum > 0)
            {
                MessageBox.Show(errorMessage);
            }

            else
            {
                _dao.AddDepartureTimeContext(new DepartureTimeContext(voziloId, polazisteId, time, dan));
                LoadData();

            }

        }

        private void AddStationButtonClick(object sender, RoutedEventArgs e)
        {
            int errorNum = 0;
            string errorMessage = String.Empty;


            string altitudeText = Station_Altitude.Text;
            float altitude = 0;

            try
            {
                altitude = (float) Double.Parse(altitudeText);
            }
            catch
            {
                errorMessage += ++errorNum + ": Error parsing 'Altitude' field.\n";
            }

            string longitudeText = Station_Longitude.Text;
            float longitude = 0;

            try
            {
                longitude = (float) Double.Parse(longitudeText);
            }
            catch 
            {
                errorMessage += ++errorNum + ": Error parsing 'Longitude' field.\n";
            }

            string name = Station_Name.Text;

            if (name == null || name.Trim().Equals(""))
            {
                errorMessage += ++errorNum + ": Error parsing 'Name' field.\n";
            }

            string direction = Station_Direction.Text;

            if (direction == null || direction.Trim().Equals(""))
            {
                errorMessage += ++errorNum + ": Error parsing 'Smjer' field.\n";
            }

            if (errorNum > 0)
            {
                MessageBox.Show(errorMessage);
            }

            else
            {
                _dao.AddStation(new Station(altitude, longitude, name, direction));
                LoadData();
            }


        }

        private void AddVehicleButtonClick(object sender, RoutedEventArgs e)
        {
            int errorNum = 0;
            string errorMessage = String.Empty;


            string brojLinijeText = Vehicle_LineNumber.Text;
            int brojLinije = 0;

            try
            {
                brojLinije = int.Parse(brojLinijeText);
                if (_dao.ContainsVehicle(brojLinije))
                {
                    MessageBox.Show("Error. Database already contains vehicle with line number = " + brojLinije + ".");
                    return;
                }
            }
            catch
            {
                errorMessage += ++errorNum + ": Error parsing 'Line number' field.\n";
            }

            string type = String.Empty;
            if(Vehicle_Tramvaj.IsChecked == true)
            {
                type = "Tramvaj";
            }
            else if(Vehicle_Autobus.IsChecked == true)
            {
                type = "Autobus";
            }

            else
            {
                errorMessage += ++errorNum + ": Select vehicle type.\n";
            }


            if (errorNum > 0)
            {
                MessageBox.Show(errorMessage);
            }

            else
            {
                _dao.AddVehicle(new Vehicle(brojLinije, type));
                LoadData();

            }


        }


        private void DeleteStationVehicleButtonClick(object sender, RoutedEventArgs e)
        {
            var item = (StationVehicleContext)StationVehicle_Grid.SelectedItem;
            if (item == null) return;
            _dao.DeleteStationVehicleContext(item.Id);
            LoadData();
        }

        private void DeleteDepartureTimeButtonClick(object sender, RoutedEventArgs e)
        {
            var item = (DepartureTimeContext)DepartureTime_Grid.SelectedItem;
            if (item == null) return;
            _dao.DeleteDepartureTimeContext(item.Id);
            LoadData();
        }

        private void DeleteStationButtonClick(object sender, RoutedEventArgs e)
        {
            var item = (Station)Station_Grid.SelectedItem;
            if (item == null) return;
            _dao.DeleteStation(item.Id);
            LoadData();
        }

        private void DeleteVehicleButtonClick(object sender, RoutedEventArgs e)
        {
            var item = (Vehicle)Vehicle_Grid.SelectedItem;
            if (item == null) return;
            _dao.DeleteVehicle(item.Id);
            LoadData();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            new Dialog().ShowDialog();
        }



        #endregion

        private void DepartureTimeSelected(object sender, MouseButtonEventArgs e)
        {
            var selected = (DepartureTimeContext)DepartureTime_Grid.SelectedItem;
            var firstOrDefault = _dao.FetchVehicles().FirstOrDefault(s => s.Id == selected.VoziloId);
            if (firstOrDefault != null)
                DepartureTime_LineNumber.Text = firstOrDefault.LineNumber + "";
            DepartureTime_Time.Text = selected.Time;
            DepartureTime_PolazisteId.Text = selected.StanicaId + "";
            if (selected.DayStatus.Equals("Radni"))
                DepartureTime_Radni.IsChecked = true;
            else if (selected.DayStatus.Equals("Subota"))
                DepartureTime_Subota.IsChecked = true;
            else
            {
                DepartureTime_Nedjelja.IsChecked = true;
            }
        }

        private void StationVehicleSelected(object sender, MouseButtonEventArgs e)
        {
            var selected = (StationVehicleContext)StationVehicle_Grid.SelectedItem;
            StationVehicle_Index.Text = selected.Index + "";
            var firstOrDefault = _dao.FetchVehicles().FirstOrDefault(s => s.Id == selected.VoziloId);
            if (firstOrDefault != null)
                StationVehicle_LineNumber.Text =firstOrDefault.LineNumber + "";
            StationVehicle_PolazisteId.Text = selected.PolazisnaStanicaId + "";
            StationVehicle_StanicaId.Text = selected.StanicaId + "";
            StationVehicle_TimeOffset.Text = selected.TimeOffset + "";

        }

        private void StationSelected(object sender, MouseButtonEventArgs e)
        {
            var selected = (Station)Station_Grid.SelectedItem;
            Station_Longitude.Text = selected.Longitude + "";
            Station_Altitude.Text = selected.Altitude + "";
            Station_Direction.Text = selected.Direction + "";
            Station_Name.Text = selected.Name;

        }

        private void VehicleItemSelected(object sender, MouseButtonEventArgs e)
        {
            var selected = (Vehicle)Vehicle_Grid.SelectedItem;
            Vehicle_LineNumber.Text = selected.LineNumber + "";
            if (selected.Type.Equals("Tramvaj"))
                Vehicle_Tramvaj.IsChecked = true;
            else
            {
                Vehicle_Autobus.IsChecked = true;
            }
        }

        private void UpdateStationVehicleButtonClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ResetSelection(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
