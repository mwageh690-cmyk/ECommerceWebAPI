using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace E_CommerceDataAccessLayer
{
    public class ShippingDTO
    {
        public int ShippingID { set; get; }

        public int OrderID { set; get; }

        public string ShippingCarrires { set; get; }

        public DateTime EstimateDeliverdDate { set; get; }

        public DateTime? ActualDeliveredDate { set; get; }

        public string? Notes { set; get; }

        public short Status { set; get; }

        public ShippingDTO(int shippingID, int orderID, string shippingCarrires, DateTime estimateDeliverdDate, DateTime? actualDeliveredDate, string? notes, short status)
        {
            ShippingID = shippingID;
            OrderID = orderID;
            ShippingCarrires = shippingCarrires;
            EstimateDeliverdDate = estimateDeliverdDate;
            ActualDeliveredDate = actualDeliveredDate;
            Notes = notes;
            Status = status;
        }

        public ShippingDTO() { }
    }
    public class clsShippingData
    {
        private static string _connecstring = clsConfing.connectionstring;

        public static int AddShipping(ShippingDTO ShippingDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_AddNewShipping", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@OrderID", ShippingDTO.OrderID);
                    command.Parameters.AddWithValue("@ShippingCarrires", ShippingDTO.ShippingCarrires);
                    command.Parameters.AddWithValue("@EstimateDeliverdDate", ShippingDTO.EstimateDeliverdDate);
                    command.Parameters.AddWithValue("@Status", ShippingDTO.Status);
                    if (string.IsNullOrEmpty(ShippingDTO.Notes))
                    {
                        command.Parameters.AddWithValue("@Notes", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Notes", ShippingDTO.Notes);
                    }

                    if (ShippingDTO.ActualDeliveredDate.HasValue)
                    {
                        command.Parameters.AddWithValue("@ActualDeliveredDate", ShippingDTO.ActualDeliveredDate);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@ActualDeliveredDate", DBNull.Value);
                    }



                        var outputIdParam = new SqlParameter("@NewShippingID", SqlDbType.Int)
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


        public static bool UpdateShipping(ShippingDTO ShippingDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_UpdateShipping", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ShippingID", ShippingDTO.ShippingID);
                    command.Parameters.AddWithValue("@OrderID", ShippingDTO.OrderID);
                    command.Parameters.AddWithValue("@ShippingCarrires", ShippingDTO.ShippingCarrires);
                    command.Parameters.AddWithValue("@EstimateDeliverdDate", ShippingDTO.EstimateDeliverdDate);
                    command.Parameters.AddWithValue("@Status", ShippingDTO.Status);
                    if (string.IsNullOrEmpty(ShippingDTO.Notes))
                    {
                        command.Parameters.AddWithValue("@Notes", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Notes", ShippingDTO.Notes);
                    }

                    if (ShippingDTO.ActualDeliveredDate.HasValue)
                    {
                        command.Parameters.AddWithValue("@ActualDeliveredDate", ShippingDTO.ActualDeliveredDate);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@ActualDeliveredDate", DBNull.Value);
                    }

                    connection.Open();
                    int num = command.ExecuteNonQuery();
                    return (num > 0) ? true : false;

                }
            }
        }


        public static ShippingDTO GetShippingById(int ReviewID)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_GetShippingByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ShippingID", ReviewID);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int notesIndex = reader.GetOrdinal("Notes");
                            int actualDateIndex = reader.GetOrdinal("ActualDeliveredDate");

                            

                           

                            return new ShippingDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("ShippingID")),
                                reader.GetInt32(reader.GetOrdinal("OrderID")),
                                reader.GetString(reader.GetOrdinal("ShippingCarrires")),
                                reader.GetDateTime(reader.GetOrdinal("EstimateDeliverdDate")),
                                reader.IsDBNull(actualDateIndex) ? null : reader.GetDateTime(actualDateIndex),
                                reader.IsDBNull(notesIndex) ? null: reader.GetString(notesIndex),
                                reader.GetInt16(reader.GetOrdinal("Status"))
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

        public static bool DeleteShipping(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_DeleteShipping", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ShippingID", studentId);

                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }


        public static bool IsShippingExist(int ID)
        {
            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_ShippingExists", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ShippingID", ID);


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


        public static List<ShippingDTO> GetShippings()
        {
            List<ShippingDTO> ReviewsList = new List<ShippingDTO>();

            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllShippings", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int notesIndex = reader.GetOrdinal("Notes");
                            int actualDateIndex = reader.GetOrdinal("ActualDeliveredDate");
                            ReviewsList.Add(new ShippingDTO
                            (
                                 reader.GetInt32(reader.GetOrdinal("ShippingID")),
                                 reader.GetInt32(reader.GetOrdinal("OrderID")),
                                 reader.GetString(reader.GetOrdinal("ShippingCarrires")),
                                 reader.GetDateTime(reader.GetOrdinal("EstimateDeliverdDate")),
                                 reader.IsDBNull(actualDateIndex) ? null : reader.GetDateTime(actualDateIndex),
                                 reader.IsDBNull(notesIndex) ? null : reader.GetString(notesIndex),
                                 reader.GetInt16(reader.GetOrdinal("Status"))
                            ));
                           
                        }
                    }
                }


                return ReviewsList;
            }

        }
    }
}
