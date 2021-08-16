using System;
using System.Data.SQLite; //TODO: Перекинуть на PostgreSQL

namespace TLGBot.Services
{
    public class Connection: IConnectionDB
    {
        public SQLiteConnection sqliteDB;
        public int UserRequestInDB;

        public void Registration(string chatId, string userName)
        {
            try
            {
                using (sqliteDB = new SQLiteConnection("Data Source=DB.db;"))
                {
                    sqliteDB.Open();
                    var regcmd = sqliteDB.CreateCommand();
                    regcmd.CommandText = "INSERT INTO RegUsers VALUES(@ChatId, @UserName, @ReqUser)";
                    regcmd.Parameters.AddWithValue("@chatId", chatId);
                    regcmd.Parameters.AddWithValue("@username", userName);
                    regcmd.Parameters.AddWithValue("@reqUser", default);
                    regcmd.ExecuteNonQuery();
                    sqliteDB.Close(); 
                } 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine("Что-то случилось с БД, нужно проверить.");
            }
        }
        
        public void Delete(string chatId)
        { 
            using (sqliteDB = new SQLiteConnection("Data Source=DB.db;"))
            {
                sqliteDB.Open();
                var regcmd = sqliteDB.CreateCommand();
                regcmd.CommandText = $"DELETE FROM RegUsers WHERE {chatId}";
                regcmd.ExecuteNonQuery();
                sqliteDB.Close(); 
            }
        }
        public string UserRequest(string chatId)
        {
            using (sqliteDB = new SQLiteConnection("Data Source=DB.db;"))
            {
                sqliteDB.Open();
                var regcmd = sqliteDB.CreateCommand();
                regcmd.CommandText = $"SELECT RequestUsers FROM RegUsers WHERE ChatId = {chatId}";
                var request = regcmd.ExecuteScalar().ToString();
                sqliteDB.Close(); 
                return request;
            }
        }
        public string Read(string chatId)
        {
            using (sqliteDB = new SQLiteConnection("Data Source=DB.db;"))
            {
                sqliteDB.Open();
                var regcmd = sqliteDB.CreateCommand();
                regcmd.CommandText = $"SELECT ChatId FROM RegUsers WHERE ChatId = {chatId}";
                var request = regcmd.ExecuteScalar();
                sqliteDB.Close(); 
                return request.ToString();
            }
        }

        public void Update(string request, string chatId)
        { 
            using (sqliteDB = new SQLiteConnection("Data Source=DB.db;"))
            {
                sqliteDB.Open();
                var regcmd = sqliteDB.CreateCommand();
                regcmd.CommandText = $"UPDATE RegUsers SET RequestUsers = @request WHERE ChatId = {chatId}";
                regcmd.Parameters.AddWithValue("@request", request);
                regcmd.ExecuteNonQuery();
                sqliteDB.Close(); 
            }
        }

    }
}