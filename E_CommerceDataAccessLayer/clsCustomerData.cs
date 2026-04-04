using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace E_CommerceDataAccessLayer
{
    public class CustomerDTO
    {
        public int CustomerID { get; set; }
        public string name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public string Role { set; get; }
        public CustomerDTO(int CustomerID, string name, string Username, string Password, string Phone, string Address , string Role)
        {
            this.CustomerID = CustomerID;
            this.name = name;
            this.Username = Username;
            this.Password = Password;
            this.Phone = Phone;
            this.Address = Address;
            this.Role = Role;
        }

        public CustomerDTO() { }

    }
    public class clsCustomerData
    {
       

        private static string _connecstring = clsConfing.connectionstring;

        public static List<CustomerDTO> GetCustomers()
        {
            List<CustomerDTO> CustomerList = new List<CustomerDTO>();

            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllCustomers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CustomerList.Add(new CustomerDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                reader.GetString(reader.GetOrdinal("name")),
                                reader.GetString(reader.GetOrdinal("Username")),
                                reader.GetString(reader.GetOrdinal("Password")),
                                reader.GetString(reader.GetOrdinal("Phone")),
                                reader.GetString(reader.GetOrdinal("Address")),
                                 reader.GetString(reader.GetOrdinal("Role"))

                            ));
                        }
                    }
                }


                return CustomerList;
            }

        }

        public static CustomerDTO GetCustomerById(int customerid)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_GetCustomerByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CustomerID", customerid);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new CustomerDTO
                            (
                                    reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                    reader.GetString(reader.GetOrdinal("name")),
                                    reader.GetString(reader.GetOrdinal("Username")),
                                    reader.GetString(reader.GetOrdinal("Password")),
                                    reader.GetString(reader.GetOrdinal("Phone")),
                                    reader.GetString(reader.GetOrdinal("Address")),
                                    reader.GetString(reader.GetOrdinal("Role"))

                            );
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static CustomerDTO GetCustomerByUsernameAndPass(string Username,string Password)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_GetCustomerByUsernameAndPassword", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Username", Username);
                    command.Parameters.AddWithValue("@Password", Password);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new CustomerDTO
                            (
                                    reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                    reader.GetString(reader.GetOrdinal("name")),
                                    reader.GetString(reader.GetOrdinal("Username")),
                                    reader.GetString(reader.GetOrdinal("Password")),
                                    reader.GetString(reader.GetOrdinal("Phone")),
                                    reader.GetString(reader.GetOrdinal("Address")),
                                    reader.GetString(reader.GetOrdinal("Role"))


                            );
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public static bool IsCustomerExist(string Username , string Password)
        {
            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CustomerExistByUsernameAndPassword", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Username", Username);
                    cmd.Parameters.AddWithValue("@Password", Password);

                    SqlParameter outputParam = new SqlParameter("@Result", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(outputParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    int result = (int)outputParam.Value;


                    return (result == 1)?true : false;
                }
            }
        }

        public static bool IsCustomerExist(int ID)
        {
            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_CustomerExist", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CustomerID", ID);
                    

                    SqlParameter outputParam = new SqlParameter("@Result", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };

                    cmd.Parameters.Add(outputParam);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    int result = (int)outputParam.Value;

                    return (result == 1);
                }
            }
        }

        public static int AddCustomer(CustomerDTO CustomerDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_AddNewCustomer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@name", CustomerDTO.name);
                    command.Parameters.AddWithValue("@Username", CustomerDTO.Username);
                    command.Parameters.AddWithValue("@Passsword", CustomerDTO.Password);
                    command.Parameters.AddWithValue("@Phone", CustomerDTO.Phone);
                    command.Parameters.AddWithValue("@Address", CustomerDTO.Address);
                    command.Parameters.AddWithValue("@Role", CustomerDTO.Role);

                    var outputIdParam = new SqlParameter("@NewCustomerID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputIdParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    return (int)outputIdParam.Value;
                }
            }
        }

        public static bool UpdateCustomer(CustomerDTO CustomerDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_UpdateCustomer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@name", CustomerDTO.name);
                    command.Parameters.AddWithValue("@Username", CustomerDTO.Username);
                    command.Parameters.AddWithValue("@Passsword", CustomerDTO.Password);
                    command.Parameters.AddWithValue("@Phone", CustomerDTO.Phone);
                    command.Parameters.AddWithValue("@Address", CustomerDTO.Address);
                    command.Parameters.AddWithValue("@CustomerID", CustomerDTO.CustomerID);
                    command.Parameters.AddWithValue("@Role", CustomerDTO.Role);


                    connection.Open();
                    int num = command.ExecuteNonQuery();
                     return (num > 0)? true: false;

                }
            }
        }

        public static bool DeleteCustomer(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_DeleteCustomer", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CustomerID", studentId);

                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }




    }
}

