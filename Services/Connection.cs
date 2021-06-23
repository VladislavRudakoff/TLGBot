using System;
using System.Data.SQLite;

namespace TLGBot.Services
{
    public static class Connection
    {
        public static SQLiteConnection sqliteDB;
        public static bool ContainedUserInDB;
        public static int UserRequestInDB;
        public static void Registration(string chatId, string userName)
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
                ContainedUserInDB = true;
            }
        }
        //If HTTP status code error 403 (403: Forbidden: bot was blocked by the user) - Remove a user from the DB 
        public static void Delete(string chatId)
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
        public static string UserRequest(string chatId)
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
        public static string Read(string chatId)
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

        public static void Update(string request, string chatId)
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