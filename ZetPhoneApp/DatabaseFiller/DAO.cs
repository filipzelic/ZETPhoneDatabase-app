using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using DatabaseFiller.Properties;

namespace DatabaseFiller
{
    public class DAO
    {
        private readonly SqlCeConnection _connection;

        public DAO(SqlCeConnection connection)
        {
            _connection = connection;
        }

        #region Connection
        public bool OpenConnection()
        {
            try
            {
                _connection.Open();
                return true;
            } catch
            {
                return false;
            }
        }

        public bool CloseConnection()
        {
            try
            {
                _connection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        int CalculateMaxId(string tableName)
        {
            var result = 0;
            using (var com = new SqlCeCommand("SELECT MAX(Id) as MaxId FROM  " + tableName, _connection))
            {
                SqlCeDataReader reader = com.ExecuteReader();
                if (reader.Read())
                {
                        if(reader.IsDBNull(0) )
                        {
                            return 0;
                        }

                }
                result = reader.GetInt32(0);

            }
            return result;
        }

        #region dohvacanjepodataka
        public Vehicle FetchVehicle(int lineNumber)
        {
            using (var com = new SqlCeCommand("SELECT * FROM Vozila WHERE LineNumber = " + lineNumber, _connection))
            {
                SqlCeDataReader reader = com.ExecuteReader();


                if (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var number = reader.GetInt32(1);
                    var type = reader.GetString(2);
                    return new Vehicle(number, type) {Id = id};
                }
                return null;

            }
   
        }

        public Station FetchStation(int stationId)
        {
            using (var com = new SqlCeCommand("SELECT * FROM Stanice WHERE Id = " + stationId, _connection))
            {
                SqlCeDataReader reader = com.ExecuteReader();


                if (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var alt = (double)reader.GetDecimal(1);
                    var lon = (double)reader.GetDecimal(2);
                    var name = reader.GetString(3);
                    var direction = reader.GetString(4);
                    return new Station(alt, lon, name, direction) { Id = id };
                }
                return null;
            }
        }

        public IList<Vehicle> FetchVehicles()
        {
            IList<Vehicle> vozila = new List<Vehicle>();

            using (var com = new SqlCeCommand("SELECT * FROM Vozila", _connection))
            {
                SqlCeDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var lineNumber = reader.GetInt32(1);
                    var type = reader.GetString(2);
                    
                    vozila.Add(new Vehicle(lineNumber, type){ Id = id});

                }
            }

            return vozila;
        }

        public IList<Station> FetchStations()
        {
            IList<Station> stanice = new List<Station>();

            using (var com = new SqlCeCommand("SELECT * FROM Stanice", _connection))
            {
                SqlCeDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var alt = (double)reader.GetDecimal(1);
                    var lon = (double)reader.GetDecimal(2);
                    var name = reader.GetString(3);
                    var direction = reader.GetString(4);
                    stanice.Add(new Station(alt, lon, name, direction) { Id = id });
                }
            }

            return stanice;
        }

        public IList<StationVehicleContext> FetchStationVehicleContextList()
        {
            IList<StationVehicleContext> contextList = new List<StationVehicleContext>();

            using (var com = new SqlCeCommand("SELECT * FROM StanicaVoziloSet", _connection))
            {
                SqlCeDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var stanicaId = reader.GetInt32(1);
                    var voziloId = reader.GetInt32(2);
                    var timeOffset = reader.GetInt32(3);
                    var polaznaStanicaId = reader.GetInt32(4);
                    var index = reader.GetInt32(5);
                    contextList.Add(new StationVehicleContext(stanicaId, voziloId, polaznaStanicaId, timeOffset, index) { Id = id });
                }
            }

            return contextList;
        }

        public IList<DepartureTimeContext> FetchDepartureTimeContextList()
        {
            IList<DepartureTimeContext> contextList = new List<DepartureTimeContext>();

            using (var com = new SqlCeCommand("SELECT * FROM VrijemePolaska", _connection))
            {
                SqlCeDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var stanicaId = reader.GetInt32(1);
                    var voziloId = reader.GetInt32(2);
                    var vrijeme = reader.GetString(3);
                    var dan = reader.GetString(4);
                    contextList.Add(new DepartureTimeContext(voziloId, stanicaId, vrijeme, dan) { Id = id });
                }
            }

            return contextList;
        }
        #endregion
        #region dodavanjepodataka
        public bool AddStation(Station s)
        {
            int rows;
            using (var com = new SqlCeCommand("INSERT INTO Stanice (Id, Altitude, Longitude, Name, Direction) VALUES(@id, @Altitude, @Longitude, @Name, @Direction)", _connection))
            {
                int nextId = CalculateMaxId("Stanice") + 1;
                s.Id = nextId < (int) Settings.Default["Id"] ? (int) Settings.Default["Id"] : nextId;
                com.Parameters.AddWithValue("@Id", s.Id);
                com.Parameters.AddWithValue("@Altitude", s.Altitude);
                com.Parameters.AddWithValue("@Longitude", s.Longitude);
                com.Parameters.AddWithValue("@Name", s.Name);
                com.Parameters.AddWithValue("@Direction", s.Direction);
                rows = com.ExecuteNonQuery();
            }

            return rows > 0;
        }


        public bool AddVehicle(Vehicle v)
        {
            int rows;
            using (var com = new SqlCeCommand("INSERT INTO Vozila (Id, LineNumber, Type) VALUES(@Id, @LineNumber, @Type)", _connection))
            {
                int nextId = CalculateMaxId("Vozila") + 1;
                v.Id = nextId < (int)Settings.Default["Id"] ? (int)Settings.Default["Id"] : nextId;

                com.Parameters.AddWithValue("@Id", v.Id);
                com.Parameters.AddWithValue("@LineNumber", v.LineNumber);
                com.Parameters.AddWithValue("@Type", v.Type);
                rows = com.ExecuteNonQuery();
            }
            return rows > 0;
        }

        
        public bool AddStationVehicleContext(StationVehicleContext sv)
        {
            int rows;
            using (var com = new SqlCeCommand("INSERT INTO StanicaVoziloSet (Id, StanicaId, VoziloId, TimeOffset, PolazisnaStanicaId, Indeks)" +
                                              " VALUES(@Id, @StanicaId, @VoziloId, @TimeOffset, @PolazisnaStanicaId, @Indeks)", _connection))
            {
                int nextId = CalculateMaxId("StanicaVoziloSet") + 1;
                sv.Id = nextId < (int)Settings.Default["Id"] ? (int)Settings.Default["Id"] : nextId;

                com.Parameters.AddWithValue("@Id", sv.Id);
                com.Parameters.AddWithValue("@StanicaId", sv.StanicaId);
                com.Parameters.AddWithValue("@VoziloId", sv.VoziloId);
                com.Parameters.AddWithValue("@TimeOffset", sv.TimeOffset);
                com.Parameters.AddWithValue("@PolazisnaStanicaId", sv.PolazisnaStanicaId);
                com.Parameters.AddWithValue("@Indeks", sv.Index);
                rows = com.ExecuteNonQuery();
            }

            return rows > 0;
        }

        public bool AddDepartureTimeContext(DepartureTimeContext vp)
        {
            int rows;
            using (var com = new SqlCeCommand("INSERT INTO VrijemePolaska (Id, StanicaId, VoziloId, VrijemePolaska, Dan) " +
                                              "VALUES(@Id, @StanicaId, @VoziloId, @VrijemePolaska, @Dan)", _connection))
            {
                int nextId = CalculateMaxId("VrijemePolaska") + 1;
                vp.Id = nextId < (int)Settings.Default["Id"] ? (int)Settings.Default["Id"] : nextId;

                com.Parameters.AddWithValue("@Id", vp.Id);
                com.Parameters.AddWithValue("@StanicaId", vp.StanicaId);
                com.Parameters.AddWithValue("@VoziloId", vp.VoziloId);
                com.Parameters.AddWithValue("@VrijemePolaska", vp.Time);
                com.Parameters.AddWithValue("@Dan", vp.DayStatus);
                rows = com.ExecuteNonQuery();
            }

            return rows > 0;
        }
        #endregion

        public bool DeleteStation(int stanicaId)
        {
            int rows;
            using (var com = new SqlCeCommand("DELETE FROM Stanice WHERE Id = @Id", _connection))
            {
                com.Parameters.AddWithValue("@Id", stanicaId);
                rows = com.ExecuteNonQuery();
            }

            return rows > 0;
        }

        public bool DeleteVehicle(int voziloId)
        {
            int rows;
            using (var com = new SqlCeCommand("DELETE FROM Vozila WHERE Id = @Id", _connection))
            {
                com.Parameters.AddWithValue("@Id", voziloId);
                rows = com.ExecuteNonQuery();
            }

            return rows > 0;
        }

        public bool DeleteDepartureTimeContext(int id)
        {
            int rows;
            using (var com = new SqlCeCommand("DELETE FROM VrijemePolaska WHERE Id = @Id", _connection))
            {
                com.Parameters.AddWithValue("@Id", id);
                rows = com.ExecuteNonQuery();
            }

            return rows > 0;
        }

        public bool DeleteStationVehicleContext(int id)
        {
            int rows;
            using (var com = new SqlCeCommand("DELETE FROM StanicaVoziloSet WHERE Id = @Id", _connection))
            {
                com.Parameters.AddWithValue("@Id", id);
                rows = com.ExecuteNonQuery();
            }

            return rows > 0;
        }

        public bool ContainsVehicle(int lineNumber)
        {
            return FetchVehicle(lineNumber) != null;
        }

        public bool ContainsStation(int stationId)
        {
            return FetchStation(stationId) != null;
        }
    }
}
