using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceDataAccessLayer;

namespace E_CommerceBussinessLayer
{
    public class clsOrder
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int OrderID { set; get; }
        public DateTime OrderDate { set; get; }

        public double TotalPrice {  get; }

        public string ShippingMethod { set; get; }

        public short Status { set; get; }

        public int CustomerID { set; get; }

        public clsCustomer Customer { set; get; }

        public OrderDTO SDTO
        {
            get { return (new OrderDTO(this.OrderID, this.OrderDate, this.TotalPrice, this.ShippingMethod, this.Status,this.CustomerID)); }

        }

        public clsOrder(OrderDTO SDTO, enMode cMode = enMode.AddNew)

        {
            this.OrderID = SDTO.OrderID;
            this.CustomerID = SDTO.CustomerID;
            this.OrderDate = SDTO.OrderDate;
            this.TotalPrice = SDTO.TotalPrice;
            this.ShippingMethod = SDTO.ShippingMethod;
            this.Status = SDTO.Status;
            this.Customer = clsCustomer.Find(SDTO.CustomerID);


            Mode = cMode;
        }

        private bool _AddNewOrder()
        {
            //call DataAccess Layer 

            this.OrderID = clsOrderData.AddOrder(SDTO);

            return (this.CustomerID != -1);
        }

        private bool _UpdateOrder()
        {

            return clsOrderData.UpdateOrder(SDTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewOrder())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateOrder();

            }

            return false;
        }

        public static List<OrderDTO> GetOrders()
        {
            return clsOrderData.GetOrders();
        }

        public static clsOrder Find(int ID)
        {

            OrderDTO SDTO = clsOrderData.GetOrderByID(ID);

            if (SDTO != null)
            //we return new object of that student with the right data
            {

                return new clsOrder(SDTO, enMode.Update);
            }
            else
                return null;
        }

        public static bool DeleteOrderByID(int PaymentID)
        {
            return clsOrderData.DeleteOrderByOrderID(PaymentID);
        }

        public static bool IsOrderExist(int ID)
        {
            return clsOrderData.IsOrderExist(ID);

        }


    }
}
