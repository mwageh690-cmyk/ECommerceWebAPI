using System.Security.Claims;
using E_CommerceBussinessLayer;
using E_CommerceDataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API_s.Controllers
{
    [Route("api/OrderItems")]
    [Authorize]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("All", Name = "GetAllOrderItems")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<OrderItemDTO>> GetAllOrderItems()
        {
            List<OrderItemDTO> OrderItemsList = clsOrderItem.GetOrderItems();

            if (OrderItemsList.Count == 0)
            {
                return NotFound("No OrderItems Found!");
            }
            return Ok(OrderItemsList);
        }


       
        [HttpGet("GetOrderItemByOrderItemID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<OrderItemDTO> GetOrderItemByID(int OrderItemID)
        {
            if (OrderItemID < 1)
            {
                return BadRequest($"Not accepted ID {OrderItemID}");
            }

            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            //if (student == null)
            //{
            //    return NotFound($"Student with ID {id} not found.");
            //}
            clsOrderItem orderitem = clsOrderItem.Find(OrderItemID);

            if (orderitem == null)
            {
                return NotFound($"OrderItem with ID {OrderItemID} not found.");
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
            if (!isAdmin && authenticatedStudentId != orderitem.Order.CustomerID)
                return Forbid(); // Returns HTTP 403 Forbidden

            //here we get only the DTO object to send it back.
            OrderItemDTO SDTO = orderitem.SDTO;

            //we return the DTO not the student object.
            return Ok(SDTO);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteOrderItemByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult DeleteOrderItem(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            // var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            // StudentDataSimulation.StudentsList.Remove(student);

            if (clsOrderItem.DeleteOrderItemByID(id))

                return Ok($"OrderItem with ID {id} has been deleted.");
            else
                return NotFound($"OrderItem with ID {id} not found. no rows deleted!");
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteOrderItemByOrderID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult DeleteOrderItemByOrderID(int orderid)
        {
            if (orderid < 1)
            {
                return BadRequest($"Not accepted ID {orderid}");
            }

            // var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);
            // StudentDataSimulation.StudentsList.Remove(student);

            if (clsOrderItem.DeleteOrderItemByOrderID(orderid))

                return Ok($"OrderItem with OrderID  {orderid} has been deleted.");
            else
                return NotFound($"OrderItem with OrderID {orderid} not found. no rows deleted!");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("OrderItemExist/{ID}", Name = "IsOrderItemExistByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<bool> IsOrderExist(int OrderItemID)
        {
            if (clsOrderItem.IsOrderItemExist(OrderItemID))
            {
                return Ok(true);
            }
            else
            {
                return NotFound(false);
            }
        }


        [Authorize(Roles = "Customer")]
        [HttpPost(Name = "AddOrderItem")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<OrderItemDTO> AddOrderItem(OrderItemDTO newOrderItemDTO)
        {
            //we validate the data here
            if ((newOrderItemDTO == null) || (newOrderItemDTO.OrderID < 1) || (newOrderItemDTO.ProductID < 1) || (newOrderItemDTO.Quantity < 0)
                || (newOrderItemDTO.UnitPrice == default(System.Decimal))) 
            {
                return BadRequest("Invalid Order data.");
            }

            //newStudent.Id = StudentDataSimulation.StudentsList.Count > 0 ? StudentDataSimulation.StudentsList.Max(s => s.Id) + 1 : 1;

            clsOrderItem OrderItem = new clsOrderItem(new OrderItemDTO(newOrderItemDTO.OrderItemID, newOrderItemDTO.OrderID,
                newOrderItemDTO.ProductID, newOrderItemDTO.Quantity, newOrderItemDTO.UnitPrice));
            OrderItem.Save();

            newOrderItemDTO.OrderItemID = OrderItem.OrderItemID;

            //we return the DTO only not the full student object
            //we dont return Ok here,we return createdAtRoute: this will be status code 201 created.
            return CreatedAtRoute("GetByID", new { id = newOrderItemDTO.OrderItemID }, newOrderItemDTO);

        }

       
        [HttpPut("{id}", Name = "UpdateOrderItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<OrderItemDTO> UpdateOrderItem(int id, OrderItemDTO newOrderItemDTO)
        {
            if ((newOrderItemDTO == null) || (newOrderItemDTO.OrderID < 1) || (newOrderItemDTO.ProductID < 1) || (newOrderItemDTO.Quantity < 0)
               || (newOrderItemDTO.UnitPrice == default(System.Decimal)))
            {
                return BadRequest("Invalid Order data.");
            }

            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);

            clsOrderItem orderitem = clsOrderItem.Find(id);


            if (orderitem == null)
            {
                return NotFound($"OrderItem with ID {id} not found.");
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
            if (!isAdmin && authenticatedStudentId != orderitem.Order.CustomerID)
                return Forbid(); // Returns HTTP 403 Forbidden


            orderitem.OrderID = newOrderItemDTO.OrderID;
            orderitem.ProductID = newOrderItemDTO.ProductID;
            orderitem.Quantity = newOrderItemDTO.Quantity;
            orderitem.UnitPrice = newOrderItemDTO.UnitPrice;


            orderitem.Save();

            //we return the DTO not the full student object.
            return Ok(orderitem.SDTO);

        }


    }
}
