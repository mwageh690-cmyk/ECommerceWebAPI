using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace E_CommerceDataAccessLayer
{
    public class PaymentDTO
    {
        public int PaymentID { get; set; }
        public int CustomerID { get; set; }
       
        public double Amount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
       
        public PaymentDTO(int PaymentID, int CustomerID, double Amount, string PaymentMethod, DateTime PaymentDate)
        {
            this.PaymentID = PaymentID;
            this.CustomerID = CustomerID;   
            this.Amount = Amount;
            this.PaymentMethod = PaymentMethod;
            this.PaymentDate = PaymentDate;
           
        }

        public PaymentDTO() { }

    }
    public class clsPaymentData
    {
        private static string _connecstring = clsConfing.connectionstring;

        public static List<PaymentDTO> GetPayments()
        {
            List<PaymentDTO> PaymentList = new List<PaymentDTO>();

            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllPayments", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PaymentList.Add(new PaymentDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("PaymentID")),
                                reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                reader.GetDouble(reader.GetOrdinal("Amount")),
                                reader.GetString(reader.GetOrdinal("PaymentMethod")),
                                reader.GetDateTime(reader.GetOrdinal("Date"))


                            ));
                        }
                    }
                }


                return PaymentList;
            }
        }


        public static List<PaymentDTO>GetAllPaymentsOfCustomer(int CustomerID)
        {
            List<PaymentDTO> PaymentList = new List<PaymentDTO>();

            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetPaymentByCustomerID", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CustomerID", CustomerID);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PaymentList.Add(new PaymentDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("PaymentID")),
                                reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                reader.GetDouble(reader.GetOrdinal("Amount")),
                                reader.GetString(reader.GetOrdinal("PaymentMethod")),
                                reader.GetDateTime(reader.GetOrdinal("Date"))


                            ));
                        }
                    }
                }


                return PaymentList;
            }
        }



        public static bool DeletePaymentByPaymentID(int PaymentID)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_DeletePayment", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PaymentID", PaymentID);

                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }


        public static bool DeletePaymentsOfCustomer(int CustomerID)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_DeletePaymentByCustomerID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CustomerID", CustomerID);

                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }


        public static PaymentDTO GetPaymentById(int PaymentID)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_GetPaymentByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PaymentID", PaymentID);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new PaymentDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("PaymentID")),
                                reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                reader.GetDouble(reader.GetOrdinal("Amount")),
                                reader.GetString(reader.GetOrdinal("PaymentMethod")),
                                reader.GetDateTime(reader.GetOrdinal("Date"))
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

        public static bool IsPaymentExist(int ID)
        {
            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_PaymentExist", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PaymentID", ID);


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


        public static int AddPayment(PaymentDTO PaymentDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_AddNewPayment", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CustomerID", PaymentDTO.CustomerID);
                    command.Parameters.AddWithValue("@Amount", PaymentDTO.Amount);
                    command.Parameters.AddWithValue("@PaymentMethod", PaymentDTO.PaymentMethod);
                    command.Parameters.AddWithValue("@PaymentDate", PaymentDTO.PaymentDate);
                 

                    var outputIdParam = new SqlParameter("@NewPaymentID", SqlDbType.Int)
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


        public static bool UpdatePayment(PaymentDTO PaymentDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_UpdatePayment", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@PaymentID", PaymentDTO.PaymentID);
                    command.Parameters.AddWithValue("@CustomerID", PaymentDTO.CustomerID);
                    command.Parameters.AddWithValue("@Amount", PaymentDTO.Amount);
                    command.Parameters.AddWithValue("@PaymentMethod", PaymentDTO.PaymentMethod);
                    command.Parameters.AddWithValue("@PaymentDate", PaymentDTO.PaymentDate);
                 


                    connection.Open();
                    int num = command.ExecuteNonQuery();
                    return (num > 0) ? true : false;

                }
            }
        }




    }
}
