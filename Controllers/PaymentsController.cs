using System.Security.Claims;
using E_CommerceBussinessLayer;
using E_CommerceDataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API_s.Controllers
{
    [Route("api/Payments")]
    [Authorize]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("All", Name = "GetAllPayments")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<PaymentDTO>> GetAllPayments()
        {
            List<PaymentDTO> PaymentsList = clsPayment.GetPayments();

            if (PaymentsList.Count == 0)
            {
                return NotFound("No Payments Found!");
            }
            return Ok(PaymentsList);
        }

        
        [HttpGet("AllPaymentsOfClient", Name = "GetAllPaymentsOfCustomer")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<PaymentDTO>> GetAllPaymentsOfCustomer(int CustomerID)
        {
            List<PaymentDTO> PaymentsList = clsPayment.GetAllPaymentsOfCustomer(CustomerID);

            if (PaymentsList.Count == 0)
            {
                return NotFound("No Payments For This Customer Found!");
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
            return Ok(PaymentsList);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetPaymentByPaymentID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<PaymentDTO> GetPaymentByID(int PaymentID)
        {
            if (PaymentID < 1)
            {
                return BadRequest($"Not accepted ID {PaymentID}");
            }

            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            //if (student == null)
            //{
            //    return NotFound($"Student with ID {id} not found.");
            //}
            clsPayment payment = clsPayment.Find(PaymentID);

            if (payment == null)
            {
                return NotFound($"Payment with ID {PaymentID} not found.");
            }

            //here we get only the DTO object to send it back.
            PaymentDTO SDTO = payment.SDTO;

            //we return the DTO not the student object.
            return Ok(SDTO);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeletePaymentByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult DeletePayment(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            // var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            // StudentDataSimulation.StudentsList.Remove(student);

            if (clsPayment.DeletePaymentByPaymentID(id))

                return Ok($"Payment with ID {id} has been deleted.");
            else
                return NotFound($"Payment with ID {id} not found. no rows deleted!");
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("DeletePaymentsOfCustomerID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult DeletePaymentOfCustomer(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            // var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            // StudentDataSimulation.StudentsList.Remove(student);

            if (clsPayment.DeletePaymentsOfCustomer(id))

                return Ok($"Payment with ID {id} has been deleted.");
            else
                return NotFound($"Payment with ID {id} not found. no rows deleted!");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("PaymentExist/{ID}", Name = "IsPaymentExistByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<bool> IsPaymentExist(int ID)
        {
            if (clsPayment.IsPaymentExist(ID))
            {
                return Ok(true);
            }
            else
            {
                return NotFound(false);
            }
        }

        [Authorize(Roles = "Customer")]
        [HttpPost(Name = "AddPayment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<PaymentDTO> AddPayment(PaymentDTO newPaymentDTO)
        {
            //we validate the data here
            if ((newPaymentDTO == null || string.IsNullOrEmpty(newPaymentDTO.PaymentMethod) || (newPaymentDTO.CustomerID < 1) ||
                (newPaymentDTO.Amount == default(System.Single))))
            {
                return BadRequest("Invalid Payment data.");
            }

            //newStudent.Id = StudentDataSimulation.StudentsList.Count > 0 ? StudentDataSimulation.StudentsList.Max(s => s.Id) + 1 : 1;

            clsPayment Payment = new clsPayment(new PaymentDTO(newPaymentDTO.PaymentID, newPaymentDTO.CustomerID,
                newPaymentDTO.Amount, newPaymentDTO.PaymentMethod, newPaymentDTO.PaymentDate));
            Payment.Save();

            newPaymentDTO.PaymentID = Payment.PaymentID;

            //we return the DTO only not the full student object
            //we dont return Ok here,we return createdAtRoute: this will be status code 201 created.
            return CreatedAtRoute("GetByID", new { id = newPaymentDTO.PaymentID }, newPaymentDTO);

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}", Name = "UpdatePayment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PaymentDTO> UpdatePayment(int id, PaymentDTO newPaymentDTO)
        {
            if ((newPaymentDTO == null || string.IsNullOrEmpty(newPaymentDTO.PaymentMethod) || (newPaymentDTO.CustomerID < 1) || (id < 1) ||
                 (newPaymentDTO.Amount == default(System.Single))))
            {
                return BadRequest("Invalid Payment data.");
            }

            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);

            clsPayment payment = clsPayment.Find(id);


            if (payment == null)
            {
                return NotFound($"Payment with ID {id} not found.");
            }


            payment.CustomerID = newPaymentDTO.CustomerID;
            payment.Amount = newPaymentDTO.Amount;
            payment.PaymentMethod = newPaymentDTO.PaymentMethod;
            payment.PaymentDate = newPaymentDTO.PaymentDate;


            payment.Save();

            //we return the DTO not the full student object.
            return Ok(payment.SDTO);

        }
    }
}
