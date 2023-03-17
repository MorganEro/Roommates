using Microsoft.Data.SqlClient;
using Roommates.Models;
using System.Collections.Generic;

namespace Roommates.Repositories
{
    public class ChoreRepository : BaseRepository
    {
        public List<Chore> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Chore";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Chore> chores = new List<Chore>();
                        while (reader.Read())
                        {
                            int idColumnPosition = reader.GetOrdinal("Id");
                            int idValue = reader.GetInt32(idColumnPosition);
                            int nameColumnPosition = reader.GetOrdinal("Name");
                            string nameValue = reader.GetString(nameColumnPosition);
                            Chore chore = new Chore()
                            {
                                Id = idValue,
                                Name = nameValue
                            };
                            chores.Add(chore);
                        }
                        return chores;
                    }
                }
            }
        }
        public ChoreRepository(string connectionString) : base(connectionString) { }
        public Chore GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Chore chore = null;
                        // If we only expect a single row back from the database, we don't need a while loop.
                        if (reader.Read())
                        {
                            chore = new Chore
                            {
                                Id = id,
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };
                        }
                        return chore;
                    }
                }
            }
        }
        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Chore (Name)
                                         OUTPUT INSERTED.Id
                                         VALUES (@name, @maxOccupancy)";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    int id = (int)cmd.ExecuteScalar();
                    chore.Id = id;
                }
            }
        }
        public List<Chore> GetUnassignedChores ()
        {
            using (SqlConnection conn = Connection)
            {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Select Chore.Name From Chore\r\nLeft Join RoommateChore\r\non Chore.id = RoommateChore.ChoreId\r\nWhere RoommateId is NULL;";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Chore> unassignedChores = new List<Chore>();
                        {
                            while (reader.Read())
                            {
                                int nameColumnPosition = reader.GetOrdinal("Name");
                                string nameValue = reader.GetString(nameColumnPosition);
                                Chore chore = new Chore()
                                {
                                    Name = nameValue
                                };
                                unassignedChores.Add(chore);
                            }
                            return unassignedChores;
                        } 
                    }
                }
            }
        }
        public void AssignChore(int roommateId, int choreId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand()) //execute against database and make queries 
                {
                    cmd.CommandText = @"INSERT INTO RoommateChore(RoommateId, ChoreId) 
                                        VALUES (@roommateId, @choreId)";
                    cmd.Parameters.AddWithValue("@roommateId", roommateId);
                    cmd.Parameters.AddWithValue("@choreId", choreId);
                    cmd.ExecuteNonQuery();

                }
            }

        }
    }

}















