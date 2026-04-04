using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceDataAccessLayer;
using Microsoft.Data.SqlClient;

namespace E_CommerceBussinessLayer
{
    public class clsPayment
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int PaymentID { get; set; }
        public int CustomerID { get; set; }

        public double Amount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }

        public clsCustomer customer { set; get; }

        public PaymentDTO SDTO
        {
            get { return (new PaymentDTO(this.PaymentID, this.CustomerID, this.Amount, this.PaymentMethod, this.PaymentDate)); }

        }

        public clsPayment(PaymentDTO SDTO, enMode cMode = enMode.AddNew)

        {
            this.PaymentID = SDTO.PaymentID;
            this.CustomerID = SDTO.CustomerID;
            this.Amount = SDTO.Amount;
            this.PaymentMethod = SDTO.PaymentMethod;
            this.PaymentDate = SDTO.PaymentDate;
            this.customer = clsCustomer.Find(SDTO.CustomerID);
            

            Mode = cMode;
        }

        private bool _AddNewPayment()
        {
            //call DataAccess Layer 

            this.PaymentID = clsPaymentData.AddPayment(SDTO);

            return (this.CustomerID != -1);
        }

        private bool _UpdatePayment()
        {

            return clsPaymentData.UpdatePayment(SDTO);
        }
        public static List<PaymentDTO> GetPayments()
        {
            return clsPaymentData.GetPayments();
        }

        public static List<PaymentDTO> GetAllPaymentsOfCustomer(int CustomerID)
        {
            return clsPaymentData.GetAllPaymentsOfCustomer(CustomerID);
        }


        public static clsPayment Find(int ID)
        {

            PaymentDTO SDTO = clsPaymentData.GetPaymentById(ID);

            if (SDTO != null)
            //we return new object of that student with the right data
            {

                return new clsPayment(SDTO, enMode.Update);
            }
            else
                return null;
        }


        public static bool DeletePaymentByPaymentID(int PaymentID)
        {
            return clsPaymentData.DeletePaymentByPaymentID(PaymentID);
        }


        public static bool DeletePaymentsOfCustomer(int CustomerID)
        {
            return clsPaymentData.DeletePaymentsOfCustomer(CustomerID);
        }

        public static bool IsPaymentExist(int ID)
        {
            return clsPaymentData.IsPaymentExist(ID);
           
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewPayment())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdatePayment();

            }

            return false;
        }


    }
}
