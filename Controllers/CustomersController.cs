using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using E_CommerceBussinessLayer;
using E_CommerceDataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static E_CommerceDataAccessLayer.clsCustomerData;

namespace E_Commerce_API_s.Controllers
{
    
    [Route("api/Customers")]
    [Authorize]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("All", Name = "GetAllCustomers")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<CustomerDTO>> GetAllCustomers() // Define a method to get all students.
        {


            List<CustomerDTO> CustomersList = clsCustomer.GetAllCustomers();
            if (CustomersList.Count == 0)
            {
                return NotFound("No Customers Found!");
            }
            return Ok(CustomersList); // Returns the list of students.

        }


        [Authorize(Roles = "Admin")]
        [HttpGet("Username/Password", Name = "GetByUsernameAndPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<CustomerDTO> GetCustomerByUsernameAndPassword(string username, string password)
        {
            clsCustomer customer = clsCustomer.Find(username, password);
            if (customer == null)
            {
                return NotFound("InValid Credintails");
            }

            CustomerDTO DTO = customer.SDTO;
            return Ok(DTO);
        }

        
        [HttpGet("ID", Name = "GetByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<CustomerDTO> GetCustomerByID(int CustomerID)
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
            clsCustomer student = clsCustomer.Find(CustomerID);

            if (student == null)
            {
                return NotFound($"Customer with ID {CustomerID} not found.");
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
            if (!isAdmin && authenticatedStudentId != CustomerID)
                return Forbid(); // Returns HTTP 403 Forbidden

            //here we get only the DTO object to send it back.
            CustomerDTO SDTO = student.SDTO;

            //we return the DTO not the student object.
            return Ok(SDTO);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("CustomerExist/{ID}", Name = "IsCustomerExistByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<bool> IsCustomerExist(int ID)
        {
            if (clsCustomer.IsCustomerExist(ID))
            {
                return Ok(true);
            }
            else
            {
                return NotFound(false);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("CustomerExist/username/password", Name = "IsCustomerExistByUsernameAndPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<bool> IsCustomerExist(string UserName, string Password)
        {
            
            if (!clsCustomer.IsCustomerExist(UserName,Password))
            {
                return NotFound(false);
            }
            else
            {
                return Ok(true);
            }
        }



        [AllowAnonymous]
        [HttpPost(Name = "AddCustomer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<CustomerDTO> AddCustomer(CustomerDTO newCustomerDTO)
        {
            //we validate the data here
            if (newCustomerDTO == null || string.IsNullOrEmpty(newCustomerDTO.name) || string.IsNullOrEmpty(newCustomerDTO.Username) ||
                string.IsNullOrEmpty(newCustomerDTO.Password) ||
                string.IsNullOrEmpty(newCustomerDTO.Phone)|| 
                string.IsNullOrEmpty(newCustomerDTO.Address)|| string.IsNullOrEmpty(newCustomerDTO.Role))
                
            {
                return BadRequest("Invalid student data.");
            }

            //newStudent.Id = StudentDataSimulation.StudentsList.Count > 0 ? StudentDataSimulation.StudentsList.Max(s => s.Id) + 1 : 1;

            clsCustomer Customer = new clsCustomer(new CustomerDTO(newCustomerDTO.CustomerID , newCustomerDTO.name , 
                newCustomerDTO.Username , newCustomerDTO.Password ,newCustomerDTO.Phone, newCustomerDTO.Address , newCustomerDTO.Role));
            Customer.Save();

            newCustomerDTO.CustomerID = Customer.CustomerID;

            //we return the DTO only not the full student object
            //we dont return Ok here,we return createdAtRoute: this will be status code 201 created.
            return CreatedAtRoute("GetByID", new { id = newCustomerDTO.CustomerID }, newCustomerDTO);

        }



        
        [HttpPut("{id}", Name = "UpdateCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CustomerDTO> UpdateStudent(int id, CustomerDTO updatedCustomer)
        {
            if (id < 1 || updatedCustomer == null || string.IsNullOrEmpty(updatedCustomer.name) ||
                string.IsNullOrEmpty(updatedCustomer.Username) ||
                string.IsNullOrEmpty(updatedCustomer.Password) ||
                string.IsNullOrEmpty(updatedCustomer.Phone) ||
                string.IsNullOrEmpty(updatedCustomer.Address) || string.IsNullOrEmpty(updatedCustomer.Role))
            {
                return BadRequest("Invalid student data.");
            }

            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);

            clsCustomer customer = clsCustomer.Find(id);


            if (customer == null)
            {
                return NotFound($"Student with ID {id} not found.");
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
            if (!isAdmin && authenticatedStudentId != id)
                return Forbid(); // Returns HTTP 403 Forbidden




            customer.name = updatedCustomer.name;
            customer.Username = updatedCustomer.Username;
            customer.Password = _HashPassword(updatedCustomer.Password);
            customer.Phone = updatedCustomer.Phone;
            customer.Address = updatedCustomer.Address;
            customer.Role = updatedCustomer.Role;

            customer.Save();

            //we return the DTO not the full student object.
            return Ok(customer.SDTO);

        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}", Name = "DeleteCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteCustomer(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            // var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            // StudentDataSimulation.StudentsList.Remove(student);

            if (clsCustomer.DeleteCustomer(id))

                return Ok($"Customer with ID {id} has been deleted.");
            else
                return NotFound($"Customer with ID {id} not found. no rows deleted!");
        }


        private static string _HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();
                foreach (byte b in hash)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }



    }
    
}
