using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceDataAccessLayer;

namespace E_CommerceBussinessLayer
{
    public class clsReview
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public ReviewDTO SDTO
        {
            get { return (new ReviewDTO(this.ReviewID, this.CustomerID, this.ProductID, this.ReviewText, this.Rating, this.ReviewDate)); }

        }

        public clsReview(ReviewDTO SDTO, enMode cMode = enMode.AddNew)
        {
            this.ReviewID = SDTO.ReviewID;
            this.ProductID = SDTO.ProductID;
            this.CustomerID = SDTO.CustomerID;
            this.ReviewText = SDTO.ReviewText;
            this.Rating = SDTO.Rating;
            this.ReviewDate = SDTO.ReviewDate;
            this.customer = clsCustomer.Find(SDTO.CustomerID);
            this.Product = clsProduct.Find(SDTO.ProductID);

            Mode = cMode;
        }

        public int ReviewID { set; get; }

        public int CustomerID { set; get; }

        public int ProductID { set; get; }

        public string ReviewText { set; get; }

        public short Rating { set; get; }

        public DateTime ReviewDate { set; get; }

        public clsCustomer customer { set; get; }

        public clsProduct Product { set; get; }

        public static List<ReviewDTO> GetReviews()
        {
            return clsReviewData.GetReviews();
        }

        public static bool IsReviewExist(int ID)
        {
            return clsReviewData.IsReviewExist(ID);
        }

        public static bool DeleteReview(int studentId)
        {
            return clsReviewData.DeleteReview(studentId);
        }

        private bool _AddNewReview()
        {
            //call DataAccess Layer 

            this.ReviewID = clsReviewData.AddReview(SDTO);

            return (this.ReviewID != -1);
        }

        private bool _UpdateReview()
        {

            return clsReviewData.UpdateReview(SDTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewReview())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateReview();

            }

            return false;
        }

        public static clsReview Find(int ID)
        {

            ReviewDTO SDTO = clsReviewData.GetReviewById(ID);

            if (SDTO != null)
            //we return new object of that student with the right data
            {

                return new clsReview(SDTO, enMode.Update);
            }
            else
                return null;
        }


    }
}
