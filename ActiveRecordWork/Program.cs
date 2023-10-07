using System;
using System.Data;
using System.Data.SqlClient;

string conStr = @"Server=(localdb)\local; Database=UserDB; Trusted_Connection=True;";
User newUser = new User(conStr);
newUser.UserName = "John";
newUser.Password = "password";

newUser.Save();

User foundUser = User.Find(1, conStr);
if (foundUser != null)
{
    Console.WriteLine(foundUser.UserName);
}

public class User
{
    readonly string conStr = @"Server=(localdb)\local; Database=UserDB; Trusted_Connection=True;";

    public int ID { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    public User(string connectionString)
    {
        conStr = connectionString;
    }

    public void Save()
    {
        using (SqlConnection con = new SqlConnection(conStr))
        {
            con.Open();
            SqlCommand cmd = con.CreateCommand();

            if (ID == 0)
            {
                cmd.CommandText = "INSERT INTO [User] (UserName, Password) VALUES (@UserName, @Password)";
            }
            else
            {
                cmd.CommandText = "UPDATE [User] SET UserName = @UserName, Password = @Password WHERE Id = @Id";
                cmd.Parameters.AddWithValue("@Id", ID);
            }

            cmd.Parameters.AddWithValue("@UserName", UserName);
            cmd.Parameters.AddWithValue("@Password", Password);

            cmd.ExecuteNonQuery();
        }
    }

    public static User Find(int id, string connectionString)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM [User] WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                return new User(connectionString)
                {
                    ID = id,
                    UserName = row.Field<string>("UserName"),
                    Password = row.Field<string>("Password")
                };
            }
            else
            {
                return null;
            }
        }
    }
}
