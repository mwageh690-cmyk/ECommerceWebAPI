using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace E_CommerceDataAccessLayer
{
    public class OrderDTO
    {
        public int OrderID { set; get; }
        public DateTime OrderDate {  set; get; }

        public double TotalPrice { get; }

        public string ShippingMethod {  set; get; }

        public short Status {  set; get; }

        public int CustomerID {  set; get; }

        public OrderDTO(int orderID, DateTime orderDate, double totalPrice, string shippingMethod, short status, int customerID)
        {
            OrderID = orderID;
            OrderDate = orderDate;
            TotalPrice = totalPrice;
            ShippingMethod = shippingMethod;
            Status = status;
            CustomerID = customerID;
        }

        public OrderDTO() { }
    }
    public class clsOrderData
    {
        private static string _connecstring = clsConfing.connectionstring;

        public static List<OrderDTO> GetOrders()
        {
            List<OrderDTO> CustomerList = new List<OrderDTO>();

            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllOrders", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CustomerList.Add(new OrderDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("OrderID")),
                                reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                                reader.GetDouble(reader.GetOrdinal("TotalPrice")),
                                reader.GetString(reader.GetOrdinal("ShippingMethod")),
                                reader.GetInt16(reader.GetOrdinal("Status")),
                                reader.GetInt32(reader.GetOrdinal("CustomerID"))

                            ));
                        }
                    }
                }


                return CustomerList;
            }

        }

        public static OrderDTO GetOrderByID(int OrderID)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_GetOrderByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OrderID", OrderID);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new OrderDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("OrderID")),
                                reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                                reader.GetDouble(reader.GetOrdinal("TotalPrice")),
                                reader.GetString(reader.GetOrdinal("ShippingMethod")),
                                reader.GetInt16(reader.GetOrdinal("Status")),
                                reader.GetInt32(reader.GetOrdinal("CustomerID"))
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


        public static bool DeleteOrderByOrderID(int OrderID)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_DeleteOrder", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OrderID", OrderID);

                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool IsOrderExist(int ID)
        {
            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_OrderExists", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@OrderID", ID);


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

        public static short AddOrder(OrderDTO OrderDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_AddNewOrder", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CustomerID", OrderDTO.CustomerID);
                    command.Parameters.AddWithValue("@OrderDate", OrderDTO.OrderDate);
                    command.Parameters.AddWithValue("@ShippingMethod", OrderDTO.ShippingMethod);
                    command.Parameters.AddWithValue("@Status", OrderDTO.Status);


                    var outputIdParam = new SqlParameter("@NewOrderID", SqlDbType.SmallInt)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputIdParam);

                    connection.Open();
                    command.ExecuteNonQuery();

                    return (short)outputIdParam.Value;
                }
            }
        }

        public static bool UpdateOrder(OrderDTO OrderDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_UpdateOrder", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@OrderID", OrderDTO.OrderID);
                    command.Parameters.AddWithValue("@CustomerID", OrderDTO.CustomerID);
                    command.Parameters.AddWithValue("@OrderDate", OrderDTO.OrderDate);
                    command.Parameters.AddWithValue("@ShippingMethod", OrderDTO.ShippingMethod);
                    command.Parameters.AddWithValue("@Status", OrderDTO.Status);



                    connection.Open();
                    int num = command.ExecuteNonQuery();
                    return (num > 0) ? true : false;

                }
            }
        }

    }
}
