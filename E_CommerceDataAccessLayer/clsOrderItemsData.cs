using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace E_CommerceDataAccessLayer
{
    public class OrderItemDTO
    {
        public int OrderItemID { set; get; }

        public int OrderID { set; get; }

        public int ProductID { set; get; }

        public int Quantity { set; get; }

        public decimal UnitPrice { set; get; }

        public OrderItemDTO(int orderItemID, int orderID, int productID, int quantity, decimal unitPrice)
        {
            OrderItemID = orderItemID;
            OrderID = orderID;
            ProductID = productID;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
    public class clsOrderItemsData
    {
        private static string _connecstring = clsConfing.connectionstring;

        public static int AddOrderItem(OrderItemDTO OrderItemDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_AddNewOrderItem", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@OrderID", OrderItemDTO.OrderID);
                    command.Parameters.AddWithValue("@ProductID", OrderItemDTO.ProductID);
                    command.Parameters.AddWithValue("@Quantity", OrderItemDTO.Quantity);
                    command.Parameters.AddWithValue("@UnitPrice", OrderItemDTO.UnitPrice);


                    var outputIdParam = new SqlParameter("@NewOrderItemID", SqlDbType.SmallInt)
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

        public static bool DeleteOrderItemByOrderItemID(int OrderItemID)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_DeleteOrderItem", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OrderItemID", OrderItemID);

                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public static bool DeleteOrderItemByOrderID(int OrderID)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_DeleteOrderItemByOrderID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OrderID", OrderID);

                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }


        public static List<OrderItemDTO> GetOrderItems()
        {
            List<OrderItemDTO> OrderItemsList = new List<OrderItemDTO>();

            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllOrderItems", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            OrderItemsList.Add(new OrderItemDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("OrderItemID")),
                                reader.GetInt32(reader.GetOrdinal("OrderID")),
                                reader.GetInt32(reader.GetOrdinal("ProductID")),
                                reader.GetInt32(reader.GetOrdinal("Quantity")),
                                reader.GetDecimal(reader.GetOrdinal("UnitPrice"))
                                

                            ));
                        }
                    }
                }


                return OrderItemsList;
            }

        }


        public static OrderItemDTO GetOrderItemByID(int OrderItemID)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_GetOrderItemByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OrderItemID", OrderItemID);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new OrderItemDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("OrderItemID")),
                                reader.GetInt32(reader.GetOrdinal("OrderID")),
                                reader.GetInt32(reader.GetOrdinal("ProductID")),
                                reader.GetInt32(reader.GetOrdinal("Quantity")),
                                reader.GetDecimal(reader.GetOrdinal("UnitPrice"))
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

        public static bool IsOrderItemExist(int ID)
        {
            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_OrderItemExist", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@OrderItemID", ID);


                    SqlParameter outputParam = new SqlParameter("@ReturnValue", SqlDbType.Int)
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

        public static bool UpdateOrderItem(OrderItemDTO OrderItemDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_UpdateOrderItem", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@OrderID", OrderItemDTO.OrderID);
                    command.Parameters.AddWithValue("@ProductID", OrderItemDTO.ProductID);
                    command.Parameters.AddWithValue("@Quantity", OrderItemDTO.Quantity);
                    command.Parameters.AddWithValue("@UnitPrice", OrderItemDTO.UnitPrice);
                    command.Parameters.AddWithValue("@OrderItemID", OrderItemDTO.OrderItemID);




                    connection.Open();
                    int num = command.ExecuteNonQuery();
                    return (num > 0) ? true : false;

                }
            }
        }






    }
}
