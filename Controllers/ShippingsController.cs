using System.Security.Claims;
using E_CommerceBussinessLayer;
using E_CommerceDataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API_s.Controllers
{
    [Route("api/Shippings")]
    [Authorize]
    [ApiController]
    public class ShippingsController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("All", Name = "GetAllShippings")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<ShippingDTO>> GetAllShippings() // Define a method to get all students.
        {


            List<ShippingDTO> CustomersList = clsShipping.GetShippings();
            if (CustomersList.Count == 0)
            {
                return NotFound("No Customers Found!");
            }
            return Ok(CustomersList); // Returns the list of students.

        }


        [Authorize(Roles = "Admin")]
        [HttpGet("ShippingExist/{ID}", Name = "IsShippingExistByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<bool> IsShippingExist(int ShippingID)
        {
            if (clsShipping.IsShippingExist(ShippingID))
            {
                return Ok(true);
            }
            else
            {
                return NotFound(false);
            }
        }



        [Authorize(Roles = "Admin")]
        [HttpPost(Name = "AddShipping")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<ShippingDTO> AddShipping(ShippingDTO newCustomerDTO)
        {
            //we validate the data here
            if (newCustomerDTO == null || string.IsNullOrEmpty(newCustomerDTO.ShippingCarrires) || (newCustomerDTO.OrderID < 1) ||
                (newCustomerDTO.Status < 1) || (newCustomerDTO.EstimateDeliverdDate == null))
               
            {
                return BadRequest("Invalid Shipping data.");
            }

            //newStudent.Id = StudentDataSimulation.StudentsList.Count > 0 ? StudentDataSimulation.StudentsList.Max(s => s.Id) + 1 : 1;

            clsShipping Customer = new clsShipping(new ShippingDTO(newCustomerDTO.ShippingID, newCustomerDTO.OrderID,
                newCustomerDTO.ShippingCarrires, newCustomerDTO.EstimateDeliverdDate, newCustomerDTO.ActualDeliveredDate, newCustomerDTO.Notes , newCustomerDTO.Status));
            Customer.Save();

            newCustomerDTO.ShippingID = Customer.ShippingID;

            //we return the DTO only not the full student object
            //we dont return Ok here,we return createdAtRoute: this will be status code 201 created.
            return CreatedAtRoute("GetByID", new { id = newCustomerDTO.ShippingID }, newCustomerDTO);

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}", Name = "UpdateShipping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ShippingDTO> UpdateShipping(int id, ShippingDTO newCustomerDTO)
        {
            if (newCustomerDTO == null || string.IsNullOrEmpty(newCustomerDTO.ShippingCarrires) || (newCustomerDTO.OrderID < 1) ||
                (newCustomerDTO.Status < 1) || (newCustomerDTO.EstimateDeliverdDate == null))

            {
                return BadRequest("Invalid Shipping data.");
            }


            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);

            clsShipping shipping = clsShipping.Find(id);


            if (shipping == null)
            {
                return NotFound($"Shipping with ID {id} not found.");
            }


            shipping.OrderID = newCustomerDTO.OrderID;
            shipping.ShippingCarrires = newCustomerDTO.ShippingCarrires;
            shipping.EstimateDeliverdDate = newCustomerDTO.EstimateDeliverdDate;
            shipping.ActualDeliveredDate = newCustomerDTO.ActualDeliveredDate;
            shipping.Notes = newCustomerDTO.Notes;
            shipping.Status = newCustomerDTO.Status;


            shipping.Save();

            //we return the DTO not the full student object.
            return Ok(shipping.SDTO);

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}", Name = "DeleteShipping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteShipping(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            // var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            // StudentDataSimulation.StudentsList.Remove(student);

            if (clsShipping.DeleteShipping(id))

                return Ok($"Shipping with ID {id} has been deleted.");
            else
                return NotFound($"Shipping with ID {id} not found. no rows deleted!");
        }


       
        [HttpGet("ID", Name = "GetShippingByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<ShippingDTO> GetShippingByID(int CustomerID)
        {
            if (CustomerID < 1)
            {
                return BadRequest($"Not accepted ID {CustomerID}");
            }

            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            //if (student == null)
            //{
            //    return NotFound($"Student with ID {id} not found.");
            //}
            clsShipping student = clsShipping.Find(CustomerID);

            if (student == null)
            {
                return NotFound($"Shipping with ID {CustomerID} not found.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            // Extract the authenticated user's role from the JWT.
            // Typical values are "Student" or "Admin".
            var userRole = User.FindFirstValue(ClaimTypes.Role);


            // Convert the authenticated user ID from string to integer.
            // This represents the identity of the caller.
            int authenticatedStudentId = int.Parse(userId);


            // Determine whether the current user is an Admin.
            // Admins are allowed to access any student record.
            bool isAdmin = userRole == "Admin";


            // Ownership check:
            // If the user is NOT an admin and is trying to access
            // a student record that does not belong to them,
            // the request is forbidden.
            if (!isAdmin && authenticatedStudentId != student.Order.CustomerID)
                return Forbid(); // Returns HTTP 403 Forbidden

            //here we get only the DTO object to send it back.
            ShippingDTO SDTO = student.SDTO;

            //we return the DTO not the student object.
            return Ok(SDTO);
        }




    }
}
