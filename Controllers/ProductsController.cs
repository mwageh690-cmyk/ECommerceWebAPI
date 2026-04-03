using E_CommerceBussinessLayer;
using E_CommerceDataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API_s.Controllers
{
    [Route("api/Products")]
    [Authorize]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("All", Name = "GetAllProducts")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<ProductDTO>> GetAllProducts() // Define a method to get all students.
        {


            List<ProductDTO> ProductsList = clsProduct.GetProducts();
            if (ProductsList.Count == 0)
            {
                return NotFound("No Customers Found!");
            }
            return Ok(ProductsList); // Returns the list of students.

        }

        [AllowAnonymous]
        [HttpGet("ID", Name = "GetProductByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<ProductDTO> GetProductByID(int ProductID)
        {
            if (ProductID < 1)
            {
                return BadRequest($"Not accepted ID {ProductID}");
            }

            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            //if (student == null)
            //{
            //    return NotFound($"Student with ID {id} not found.");
            //}
            clsProduct product = clsProduct.Find(ProductID);

            if (product == null)
            {
                return NotFound($"Product with ID {ProductID} not found.");
            }

            //here we get only the DTO object to send it back.
            ProductDTO SDTO = product.SDTO;

            //we return the DTO not the student object.
            return Ok(SDTO);
        }


        [HttpGet("ProductID", Name = "IsProductExist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<bool> IsProductExist(int ProductID)
        {
            if(ProductID < 1)
            {
                return BadRequest($"ProductID Must Be Positive");
            }
            else
            {
                if (clsProduct.IsProductExist(ProductID))
                {
                    return Ok(true);
                }
                else
                {
                    return NotFound(false);
                }
            }
           
        }

        [Authorize(Roles = "Admin")]
        [HttpPost(Name = "AddProduct")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<ProductDTO> AddProduct(ProductDTO product)
        {
            //we validate the data here
            if (product == null || string.IsNullOrEmpty(product.name) || string.IsNullOrEmpty(product.Description) ||
               (product.Price == 0) || (product.Price < 1) ||
               (product.QuantityInStock < 1) ||
                string.IsNullOrEmpty(product.ImageURL))
            {
                return BadRequest("Invalid Product data.");
            }

            //newStudent.Id = StudentDataSimulation.StudentsList.Count > 0 ? StudentDataSimulation.StudentsList.Max(s => s.Id) + 1 : 1;

            clsProduct Product = new clsProduct(new ProductDTO(product.ProductID, product.name,
                product.Description, product.Price, product.QuantityInStock, product.notes , product.ImageURL));
            Product.Save();

            product.ProductID = Product.ProductID;

            //we return the DTO only not the full student object
            //we dont return Ok here,we return createdAtRoute: this will be status code 201 created.
            return CreatedAtRoute("GetByID", new { id = product.ProductID }, product);

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteProduct(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            // var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            // StudentDataSimulation.StudentsList.Remove(student);

            if (clsProduct.DeleteProduct(id))

                return Ok($"Product with ID {id} has been deleted.");
            else
                return NotFound($"Product with ID {id} not found. no rows deleted!");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CustomerDTO> UpdateProduct(int id, ProductDTO product)
        {
            if (product == null || string.IsNullOrEmpty(product.name) || string.IsNullOrEmpty(product.Description) ||
              (product.Price == 0) || (product.Price < 1) ||
              (product.QuantityInStock < 1) ||
               string.IsNullOrEmpty(product.ImageURL))
            {
                return BadRequest("Invalid Product data.");
            }

            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);

            clsProduct product2 = clsProduct.Find(id);


            if (product2 == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }


            product2.name = product.name;
            product2.Description = product.Description;
            product2.Price = product.Price;
            product2.QuantityInStock = product.QuantityInStock;
            product2.notes = product.notes;
            product2.ImageURL = product.ImageURL;

            product2.Save();

            //we return the DTO not the full student object.
            return Ok(product2.SDTO);

        }





    }
}
