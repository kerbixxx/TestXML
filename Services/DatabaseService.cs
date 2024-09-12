using System.Data;
using System.Data.SqlClient;
using TestXML.Models;

namespace TestXML.Services
{
    public static class DatabaseService
    {
        private const string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=5051;Database=Market";

        public static void SaveDataToDatabase(List<Order> orders)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var order in orders)
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            InsertUsers(order, connection, transaction);

                            InsertProducts(order, connection, transaction);

                            InsertOrder(order, connection, transaction);

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
        }

        private static void InsertUsers(Order order, SqlConnection connection, SqlTransaction transaction)
        {
            if (!IsUserExists(connection, order.User.Email))
            {
                using (var command = new SqlCommand("INSERT INTO users (Fio, Email) VALUES (@Fio, @Email)", connection))
                {
                    command.Parameters.Add("@Fio", SqlDbType.Text).Value = order.User.Fio;
                    command.Parameters.Add("@Email", SqlDbType.Text).Value = order.User.Email;
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void InsertProducts(Order order, SqlConnection connection, SqlTransaction transaction)
        {
            foreach (var product in order.Products)
            {
                if (!IsProductExists(connection, product.Name))
                {
                    using (var command = new SqlCommand("INSERT INTO products (Name, Price) VALUES (@Name, @Price)", connection))
                    {
                        command.Parameters.Add("@Name", SqlDbType.Text).Value = product.Name;
                        command.Parameters.Add("@Price", SqlDbType.Decimal).Value = product.Price;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        private static void InsertOrder(Order order, SqlConnection connection, SqlTransaction transaction)
        {
            using (var command = new SqlCommand("INSERT INTO orders (No, Reg_Date, User_Id, Sum) VALUES (@No, @Reg_Date,@User_Id, @Sum)", connection))
            {
                command.Parameters.Add("@No", SqlDbType.Int).Value = order.No;

                string dateString = order.Reg_Date;
                DateTime dateTime = new DateTime(int.Parse(dateString.Split('.')[0]), int.Parse(dateString.Split('.')[1]), int.Parse(dateString.Split('.')[2]));

                command.Parameters.Add("@Reg_Date", SqlDbType.Date).Value = dateTime;
                var id = GetUserIdByEmail(connection, order.User.Email);
                command.Parameters.Add("@User_Id", SqlDbType.Int).Value = id;
                command.Parameters.Add("@Sum", SqlDbType.Decimal).Value = order.Sum;
                command.ExecuteNonQuery();
            }

            foreach (var product in order.Products)
            {
                using (var command = new SqlCommand("INSERT INTO order_products (Order_Id, Product_Id, Quantity) VALUES (@OrderId, @ProductId, @Quantity)", connection))
                {
                    command.Parameters.Add("@OrderId", SqlDbType.Int).Value = order.No;
                    command.Parameters.Add("@ProductId", SqlDbType.Int).Value = GetProductIdByName(connection, product.Name);
                    command.Parameters.Add("@Quantity", SqlDbType.Int).Value = product.Quantity;
                    command.ExecuteNonQuery();
                }
            }
        }

        private static bool IsProductExists(SqlConnection connection, string productName)
        {
            using (var command = new SqlCommand($"SELECT EXISTS (SELECT 1 FROM products WHERE Name = @ProductName)", connection))
            {
                command.Parameters.Add("@ProductName", SqlDbType.Text).Value = productName;
                return Convert.ToBoolean(command.ExecuteScalar());
            }
        }

        private static bool IsUserExists(SqlConnection connection, string email)
        {
            using (var command = new SqlCommand($"SELECT EXISTS (SELECT 1 FROM users WHERE email = @UserEmail)", connection))
            {
                command.Parameters.Add("@UserEmail", SqlDbType.Text).Value = email;
                return Convert.ToBoolean (command.ExecuteScalar());
            }
        }

        private static int GetProductIdByName(SqlConnection connection, string productName)
        {
            using (var command = new SqlCommand($"SELECT Id FROM products WHERE Name = '{productName}'", connection))
            {
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        private static int GetUserIdByEmail(SqlConnection connection, string userEmail)
        {
            using (var command = new SqlCommand($"SELECT Id FROM users WHERE email = '{userEmail}'", connection))
            {
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }
    }
}
