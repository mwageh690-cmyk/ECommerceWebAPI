using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace E_CommerceDataAccessLayer
{
    public class ProductDTO
    {
        public int ProductID { get; set; }
        public string name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int QuantityInStock { get; set; }
        public string notes { get; set; }
        public string ImageURL { set; get; }
        public ProductDTO(int ProductID, string name, string Description, double Price, int QuantityInStock, string notes,string ImageURL)
        {
            this.ProductID = ProductID;
            this.name = name;
            this.Description = Description;
            this.Price = Price;
            this.QuantityInStock = QuantityInStock;
            this.notes = notes;
            this.ImageURL = ImageURL;
        }


        public ProductDTO() { }
    }
    public class clsProductData
    {
        private static string _connecstring = clsConfing.connectionstring;

        public static List<ProductDTO> GetProducts()
        {
            List<ProductDTO> ProductList = new List<ProductDTO>();

            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllProducts", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int notesIndex = reader.GetOrdinal("notes");
                            ProductList.Add(new ProductDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("ProductID")),
                                reader.GetString(reader.GetOrdinal("name")),
                                reader.GetString(reader.GetOrdinal("Description")),
                                reader.GetDouble(reader.GetOrdinal("Price")),
                                reader.GetInt32(reader.GetOrdinal("QuantityInStock")),
                                reader.IsDBNull(notesIndex)? null : reader.GetString(notesIndex),
                                reader.GetString(reader.GetOrdinal("Image"))

                            ));
                        }
                    }
                }


                return ProductList;
            }

        }

        public static ProductDTO GetProductById(int ProductID)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_GetProductByID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductID", ProductID);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int notesIndex = reader.GetOrdinal("notes");
                            return new ProductDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("ProductID")),
                                reader.GetString(reader.GetOrdinal("name")),
                                reader.GetString(reader.GetOrdinal("Description")),
                                reader.GetDouble(reader.GetOrdinal("Price")),
                                reader.GetInt32(reader.GetOrdinal("QuantityInStock")),
                                reader.IsDBNull(notesIndex) ? null : reader.GetString(notesIndex),
                                reader.GetString(reader.GetOrdinal("Image"))
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


        public static bool IsProductExist(int ID)
        {
            using (SqlConnection conn = new SqlConnection(_connecstring))
            {
                using (SqlCommand cmd = new SqlCommand("SP_ProductExist", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductID", ID);


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

        public static int AddProduct(ProductDTO ProductDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_AddNewProduct", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@name", ProductDTO.name);
                    command.Parameters.AddWithValue("@Description", ProductDTO.Description);
                    command.Parameters.AddWithValue("@Price", ProductDTO.Price);
                    command.Parameters.AddWithValue("@QuantityInStock", ProductDTO.QuantityInStock);
                    if (string.IsNullOrEmpty(ProductDTO.notes))
                    {
                        command.Parameters.AddWithValue("@Notes", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Notes", ProductDTO.notes);
                    }
                    command.Parameters.AddWithValue("@Image", ProductDTO.ImageURL);


                    var outputIdParam = new SqlParameter("@NewProductID", SqlDbType.Int)
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


        public static bool UpdateProduct(ProductDTO ProductDTO)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_UpdateProduct", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@name", ProductDTO.name);
                    command.Parameters.AddWithValue("@Description", ProductDTO.Description);
                    command.Parameters.AddWithValue("@Price", ProductDTO.Price);
                    command.Parameters.AddWithValue("@QuantityInStock", ProductDTO.QuantityInStock);
                    command.Parameters.AddWithValue("@ProductID", ProductDTO.ProductID);
                    if (string.IsNullOrEmpty(ProductDTO.notes))
                    {
                        command.Parameters.AddWithValue("@Notes", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Notes", ProductDTO.notes);
                    }
                    command.Parameters.AddWithValue("@Image", ProductDTO.ImageURL);


                    connection.Open();
                    int num = command.ExecuteNonQuery();
                    return (num > 0) ? true : false;

                }
            }
        }

        public static bool DeleteProduct(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connecstring))
            {
                using (SqlCommand command = new SqlCommand("SP_DeleteProduct", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductID", studentId);

                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }


    }
}
