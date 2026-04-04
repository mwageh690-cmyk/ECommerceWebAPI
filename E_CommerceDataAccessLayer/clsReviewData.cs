using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace E_CommerceDataAccessLayer
{
    public class ReviewDTO
    {
        public int ReviewID { set; get; }

        public int CustomerID { set; get; }

        public int ProductID { set; get; }

        public string ReviewText { set; get; }

        public short Rating { set; get; }

        public DateTime ReviewDate { set; get; }

        public ReviewDTO(int reviewID, int customerID, int productID, string reviewText, short rating, DateTime reviewDate)
        {
            ReviewID = reviewID;
            CustomerID = customerID;
            ProductID = productID;
            ReviewText = reviewText;
            Rating = rating;
            ReviewDate = reviewDate;
        }
        public ReviewDTO() { }
    }
    public class clsReviewData
    {
        private static string _connecstring = clsConfing.connectionstring;

        public static List<ReviewDTO> GetReviews()
        {
            List<ReviewDTO> ReviewsList = new List<ReviewDTO>();

            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllReviews", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int notesIndex = reader.GetOrdinal("ReviewText");
                            ReviewsList.Add(new ReviewDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("ReviewID")),
                                reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                reader.GetInt32(reader.GetOrdinal("ProductID")),
                                 reader.IsDBNull(notesIndex) ? null : reader.GetString(notesIndex),
                                reader.GetInt16(reader.GetOrdinal("Rating")),
                                reader.GetDateTime(reader.GetOrdinal("ReviewDate"))
                               
                              
                                


                            ));
                        }
                    }
                }


                return ReviewsList;
            }

        }

        public static ReviewDTO GetReviewById(int ReviewID)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_GetReviewByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ReviewID", ReviewID);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int notesIndex = reader.GetOrdinal("notes");
                            return new ReviewDTO
                            (
                               reader.GetInt32(reader.GetOrdinal("ReviewID")),
                                reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                reader.GetInt32(reader.GetOrdinal("ProductID")),
                                 reader.IsDBNull(notesIndex) ? null : reader.GetString(notesIndex),
                                reader.GetInt16(reader.GetOrdinal("Rating")),
                                reader.GetDateTime(reader.GetOrdinal("ReviewDate"))
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

        public static bool IsReviewExist(int ID)
        {
            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_ReviewExist", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ReviewID", ID);


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

        public static int AddReview(ReviewDTO ReviewDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_AddNewReview", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@ReviewDate", ReviewDTO.ReviewDate);
                    command.Parameters.AddWithValue("@Rating", ReviewDTO.Rating);
                    command.Parameters.AddWithValue("@CustomerID", ReviewDTO.CustomerID);
                    command.Parameters.AddWithValue("@ProductID", ReviewDTO.ProductID);
                    if (string.IsNullOrEmpty(ReviewDTO.ReviewText))
                    {
                        command.Parameters.AddWithValue("@ReviewText", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@ReviewText", ReviewDTO.ReviewText);
                    }
                   


                    var outputIdParam = new SqlParameter("@NewReviewID", SqlDbType.Int)
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

        public static bool UpdateReview(ReviewDTO ReviewDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_UpdateReview", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@ReviewDate", ReviewDTO.ReviewDate);
                    command.Parameters.AddWithValue("@Rating", ReviewDTO.Rating);
                    command.Parameters.AddWithValue("@CustomerID", ReviewDTO.CustomerID);
                    command.Parameters.AddWithValue("@ProductID", ReviewDTO.ProductID);
                    command.Parameters.AddWithValue("@ReviewID", ReviewDTO.ReviewID);
                    if (string.IsNullOrEmpty(ReviewDTO.ReviewText))
                    {
                        command.Parameters.AddWithValue("@ReviewText", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@ReviewText", ReviewDTO.ReviewText);
                    }


                    connection.Open();
                    int num = command.ExecuteNonQuery();
                    return (num > 0) ? true : false;

                }
            }
        }

        public static bool DeleteReview(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_DeleteReview", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ReviewID", studentId);

                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }


    }
}
