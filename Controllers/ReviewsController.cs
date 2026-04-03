using System.Security.Claims;
using E_CommerceBussinessLayer;
using E_CommerceDataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_API_s.Controllers
{
    [Route("api/Reviews")]
    [Authorize]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("All", Name = "GetAllReviews")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<IEnumerable<ReviewDTO>> GetAllReviews()
        {
            List<ReviewDTO> OrderItemsList = clsReview.GetReviews();

            if (OrderItemsList.Count == 0)
            {
                return NotFound("No Reviews Found!");
            }
            return Ok(OrderItemsList);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ReviewExist/{ID}", Name = "IsReviewExistByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<bool> IsOrderExist(int ReviewID)
        {
            if (clsReview.IsReviewExist(ReviewID))
            {
                return Ok(true);
            }
            else
            {
                return NotFound(false);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteReviewByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult DeleteReviewByID(int orderid)
        {
            if (orderid < 1)
            {
                return BadRequest($"Not accepted ID {orderid}");
            }



            if (clsReview.DeleteReview(orderid))

                return Ok($"Review with ID  {orderid} has been deleted.");
            else
                return NotFound($"Review with ID {orderid} not found. no rows deleted!");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetReviewByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<ReviewDTO> GetReviewByID(int OrderItemID)
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
            clsReview orderitem = clsReview.Find(OrderItemID);

            if (orderitem == null)
            {
                return NotFound($"Review with ID {OrderItemID} not found.");
            }

            //here we get only the DTO object to send it back.
            ReviewDTO SDTO = orderitem.SDTO;

            //we return the DTO not the student object.
            return Ok(SDTO);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost(Name = "AddReview")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<ReviewDTO> AddReview(ReviewDTO newOrderItemDTO)
        {
            //we validate the data here
            if ((newOrderItemDTO == null) || (newOrderItemDTO.CustomerID < 1) || (newOrderItemDTO.ProductID < 1)
                || (newOrderItemDTO.Rating < 0))
            {
                return BadRequest("Invalid Review data.");
            }

            //newStudent.Id = StudentDataSimulation.StudentsList.Count > 0 ? StudentDataSimulation.StudentsList.Max(s => s.Id) + 1 : 1;

            clsReview OrderItem = new clsReview(new ReviewDTO(newOrderItemDTO.ReviewID, newOrderItemDTO.CustomerID,
                newOrderItemDTO.ProductID, newOrderItemDTO.ReviewText, newOrderItemDTO.Rating, newOrderItemDTO.ReviewDate));
            OrderItem.Save();

            newOrderItemDTO.ReviewID = OrderItem.ReviewID;

            //we return the DTO only not the full student object
            //we dont return Ok here,we return createdAtRoute: this will be status code 201 created.
            return CreatedAtRoute("GetByID", new { id = newOrderItemDTO.ReviewID }, newOrderItemDTO);

        }

       
        [HttpPut("{id}", Name = "UpdateReview")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ReviewDTO> UpdateReview(int id, ReviewDTO newOrderItemDTO)
        {
            if ((newOrderItemDTO == null) || (newOrderItemDTO.CustomerID < 1) || (newOrderItemDTO.ProductID < 1)
                || (newOrderItemDTO.Rating < 0))
            {
                return BadRequest("Invalid Review data.");
            }

            //var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == id);

            clsReview orderitem = clsReview.Find(id);


            if (orderitem == null)
            {
                return NotFound($"Review with ID {id} not found.");
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
            if (!isAdmin && authenticatedStudentId != orderitem.CustomerID)
                return Forbid(); // Returns HTTP 403 Forbidden


            orderitem.CustomerID = newOrderItemDTO.CustomerID;
            orderitem.ProductID = newOrderItemDTO.ProductID;
            orderitem.Rating = newOrderItemDTO.Rating;
            orderitem.ReviewText = newOrderItemDTO.ReviewText;
            orderitem.ReviewDate = newOrderItemDTO.ReviewDate;


            orderitem.Save();

            //we return the DTO not the full student object.
            return Ok(orderitem.SDTO);

        }
    }
}
