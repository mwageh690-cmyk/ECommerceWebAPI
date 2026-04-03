using System.Security.Claims;
using E_CommerceBussinessLayer;
using E_CommerceDataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API_s.Controllers
{
    [Route("api/Orders")]
    [Authorize]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("All", Name = "GetAllOrders")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<OrderDTO>> GetAllOrders()
        {
            List<OrderDTO> OrdersList = clsOrder.GetOrders();

            if (OrdersList.Count == 0)
            {
                return NotFound("No Orders Found!");
            }
            return Ok(OrdersList);
        }

       
        [HttpGet("GetOrderByOrderID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<OrderDTO> GetOrderByID(int OrderID)
        {
            if (OrderID < 1)
            {
                return BadRequest($"Not accepted ID {OrderID}");
            }

            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            //if (student == null)
            //{
            //    return NotFound($"Student with ID {id} not found.");
            //}
            clsOrder order = clsOrder.Find(OrderID);

            if (order == null)
            {
                return NotFound($"Order with ID {OrderID} not found.");
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
            if (!isAdmin && authenticatedStudentId != order.CustomerID)
                return Forbid(); // Returns HTTP 403 Forbidden

            //here we get only the DTO object to send it back.
            OrderDTO SDTO = order.SDTO;

            //we return the DTO not the student object.
            return Ok(SDTO);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteOrderByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult DeleteOrder(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            // var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            // StudentDataSimulation.StudentsList.Remove(student);

            if (clsOrder.DeleteOrderByID(id))

                return Ok($"Order with ID {id} has been deleted.");
            else
                return NotFound($"Order with ID {id} not found. no rows deleted!");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("OrderExist/{ID}", Name = "IsOrderExistByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<bool> IsOrderExist(int ID)
        {
            if (clsOrder.IsOrderExist(ID))
            {
                return Ok(true);
            }
            else
            {
                return NotFound(false);
            }
        }

        [Authorize(Roles = "Customer")]
        [HttpPost(Name = "AddOrder")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<OrderDTO> AddOrder(OrderDTO newOrderDTO)
        {
            //we validate the data here
            if ((newOrderDTO == null || string.IsNullOrEmpty(newOrderDTO.ShippingMethod) || (newOrderDTO.CustomerID < 1) ||
                (newOrderDTO.Status < 1) ||
                (newOrderDTO.TotalPrice == default(System.Single))))
            {
                return BadRequest("Invalid Order data.");
            }

            //newStudent.Id = StudentDataSimulation.StudentsList.Count > 0 ? StudentDataSimulation.StudentsList.Max(s => s.Id) + 1 : 1;

            clsOrder Order = new clsOrder(new OrderDTO(newOrderDTO.OrderID, newOrderDTO.OrderDate,
                newOrderDTO.TotalPrice, newOrderDTO.ShippingMethod, newOrderDTO.Status , newOrderDTO.CustomerID));
            Order.Save();

            newOrderDTO.OrderID = Order.OrderID;

            //we return the DTO only not the full student object
            //we dont return Ok here,we return createdAtRoute: this will be status code 201 created.
            return CreatedAtRoute("GetByID", new { id = newOrderDTO.OrderID }, newOrderDTO);

        }


        
        [HttpPut("{id}", Name = "UpdateOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<OrderDTO> UpdateOrder(int id, OrderDTO newOrderDTO)
        {
            if ((newOrderDTO == null || string.IsNullOrEmpty(newOrderDTO.ShippingMethod) || (newOrderDTO.CustomerID < 1) ||
                (newOrderDTO.Status < 1) ||
                (newOrderDTO.TotalPrice == default(System.Single))))
            {
                return BadRequest("Invalid Order data.");
            }

            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);

            clsOrder order = clsOrder.Find(id);


            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
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
            if (!isAdmin && authenticatedStudentId != order.CustomerID)
                return Forbid(); // Returns HTTP 403 Forbidden


            order.CustomerID = newOrderDTO.CustomerID;
            order.OrderDate = newOrderDTO.OrderDate;
            order.Status = newOrderDTO.Status;
            order.ShippingMethod = newOrderDTO.ShippingMethod;


            order.Save();

            //we return the DTO not the full student object.
            return Ok(order.SDTO);

        }
    }
}
