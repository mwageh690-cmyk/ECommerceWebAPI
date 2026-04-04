using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceDataAccessLayer;

namespace E_CommerceBussinessLayer
{
    public class clsOrderItem
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int OrderItemID { set; get; }

        public int OrderID { set; get; }

        public int ProductID { set; get; }

        public int Quantity { set; get; }

        public decimal UnitPrice { set; get; }

        public clsOrder Order { set; get; }

        public clsProduct Product {  set; get; }

        public OrderItemDTO SDTO
        {
            get { return (new OrderItemDTO(this.OrderItemID, this.OrderID, this.ProductID, this.Quantity, this.UnitPrice)); }

        }

        public clsOrderItem(OrderItemDTO SDTO, enMode cMode = enMode.AddNew)
        {
            this.OrderItemID = SDTO.OrderItemID;
            this.OrderID = SDTO.OrderID;
            this.ProductID = SDTO.ProductID;
            this.Quantity = SDTO.Quantity;
            this.UnitPrice = SDTO.UnitPrice;
            this.Order = clsOrder.Find(OrderID);
            this.Product = clsProduct.Find(ProductID);

            Mode = cMode;

        }

        private bool _AddNewOrderItem()
        {
            //call DataAccess Layer 

            this.OrderItemID = clsOrderItemsData.AddOrderItem(SDTO);

            return (this.OrderItemID != -1);
        }

        private bool _UpdateOrderItem()
        {

            return clsOrderItemsData.UpdateOrderItem(SDTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewOrderItem())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateOrderItem();

            }

            return false;
        }

        public static List<OrderItemDTO> GetOrderItems()
        {
            return clsOrderItemsData.GetOrderItems();
        }

        public static clsOrderItem Find(int ID)
        {

            OrderItemDTO SDTO = clsOrderItemsData.GetOrderItemByID(ID);

            if (SDTO != null)
            //we return new object of that student with the right data
            {

                return new clsOrderItem(SDTO, enMode.Update);
            }
            else
                return null;
        }

        public static bool DeleteOrderItemByID(int OrderItemID)
        {
            return clsOrderItemsData.DeleteOrderItemByOrderItemID(OrderItemID);
        }

        public static bool DeleteOrderItemByOrderID(int OrderID)
        {
            return clsOrderItemsData.DeleteOrderItemByOrderID(OrderID);
        }

        public static bool IsOrderItemExist(int ID)
        {
            return clsOrderItemsData.IsOrderItemExist(ID);

        }
    }
}
