using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceDataAccessLayer;

namespace E_CommerceBussinessLayer
{
    public class clsShipping
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int ShippingID { set; get; }

        public int OrderID { set; get; }

        public string ShippingCarrires { set; get; }

        public DateTime EstimateDeliverdDate { set; get; }

        public DateTime? ActualDeliveredDate { set; get; }

        public string? Notes { set; get; }

        public short Status { set; get; }

        public clsOrder Order { set; get; }

        public ShippingDTO SDTO
        {
            get { return (new ShippingDTO(this.ShippingID, this.OrderID, this.ShippingCarrires, this.EstimateDeliverdDate, this.ActualDeliveredDate, this.Notes , this.Status)); }

        }

        public clsShipping(ShippingDTO SDTO , enMode cMode = enMode.AddNew)
        {
            this.ShippingID = SDTO.ShippingID;
            this.OrderID = SDTO.OrderID;
            this.ShippingCarrires = SDTO.ShippingCarrires;
            this.EstimateDeliverdDate = SDTO.EstimateDeliverdDate;
            this.ActualDeliveredDate = SDTO.ActualDeliveredDate;
            this.Notes = SDTO.Notes;
            this.Status = SDTO.Status;
            this.Order = clsOrder.Find(SDTO.OrderID);

            Mode = cMode;
        }


        public static List<ShippingDTO> GetShippings()
        {
            return clsShippingData.GetShippings();
        }

        public static bool IsShippingExist(int ID)
        {
            return clsShippingData.IsShippingExist(ID);
        }

        public static bool DeleteShipping(int studentId)
        {
            return clsShippingData.DeleteShipping(studentId);
        }

        private bool _AddNewShipping()
        {
            //call DataAccess Layer 

            this.ShippingID = clsShippingData.AddShipping(SDTO);

            return (this.ShippingID != -1);
        }

        private bool _UpdateShipping()
        {

            return clsShippingData.UpdateShipping(SDTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewShipping())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateShipping();

            }

            return false;
        }

        public static clsShipping Find(int ID)
        {

            ShippingDTO SDTO = clsShippingData.GetShippingById(ID);

            if (SDTO != null)
            //we return new object of that student with the right data
            {

                return new clsShipping(SDTO, enMode.Update);
            }
            else
                return null;
        }




    }
}
