using Microsoft.Data.SqlClient;
using Roommates.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
namespace Roommates.Repositories
{
    public class RoommateRepository : BaseRepository
    {
        public RoommateRepository(string connectionString) : base(connectionString) { }
        public Roommate GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT FirstName, LastName, RentPortion, MoveInDate, room.Name FROM Roommate JOIN Room on Roommate.roomId = room.Id WHERE Roommate.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Roommate roommate = null;
                        Room room = null;
                        if (reader.Read())
                        {
                            room = new Room
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };
                            roommate = new Roommate
                            {
                                Id = id,
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                                MovedInDate = reader.GetDateTime(reader.GetOrdinal("MoveInDate")),
                                Room = room
                            };
                        }
                        return roommate;
                    }
                }
            }
        }
        public List<Roommate> GetAll()
        {
            //  We must "use" the database connection.
            //  Because a database is a shared resource (other applications may be using it too) we must
            //  be careful about how we interact with it. Specifically, we Open() connections when we need to
            //  interact with the database and we Close() them when we're finished.
            //  In C#, a "using" block ensures we correctly disconnect from a resource even if there is an error.
            //  For database connections, this means the connection will be properly closed.
            using (SqlConnection conn = Connection)
            {
                // Note, we must Open() the connection, the "using" block doesn't do that for us.
                conn.Open();
                // We must "use" commands too.
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here we setup the command with the SQL we want to execute before we execute it.
                    cmd.CommandText = @"SELECT *
                                        FROM Roommate;";
                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // A list to hold the roommates we retrieve from the database.
                        List<Roommate> roommates = new List<Roommate>();
                        // Read() will return true if there's more data to read
                        while (reader.Read())
                        {
                            // The "ordinal" is the numeric position of the column in the query results.
                            //  For our query, "Id" has an ordinal value of 0 and "Name" is 1.
                            int idColumnPosition = reader.GetOrdinal("Id");
                            // We user the reader's GetXXX methods to get the value for a particular ordinal.
                            int idValue = reader.GetInt32(idColumnPosition);
                            int nameColumnPosition = reader.GetOrdinal("FirstName");
                            string nameValue = reader.GetString(nameColumnPosition);
                            // Now let's create a new room object using the data from the database.
                            Roommate roommateNew = new Roommate
                            {
                                Id = idValue,
                                FirstName = nameValue
                            };
                            // ...and add that room object to our list.
                            roommates.Add(roommateNew);
                        }
                        // Return the list of rooms who whomever called this method.
                        return roommates;
                    }
                }
            }
        }

        public void Delete (int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

            }
        }












    }
}



















