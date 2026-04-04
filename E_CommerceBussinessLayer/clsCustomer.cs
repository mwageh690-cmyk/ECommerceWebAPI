using System.Data;
using System.Security.Cryptography;
using System.Text;
using E_CommerceDataAccessLayer;


namespace E_CommerceBussinessLayer
{
    public class clsCustomer
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public CustomerDTO SDTO
        {
            get { return (new CustomerDTO(this.CustomerID, this.name, this.Username, this.Password , this.Phone , this.Address , this.Role)); }

        }

        public clsCustomer(CustomerDTO SDTO, enMode cMode = enMode.AddNew)

        {
            this.CustomerID = SDTO.CustomerID;
            this.name = SDTO.name;
            this.Username = SDTO.Username;
            this.Password = _HashPassword(SDTO.Password);
            this.Phone = SDTO.Phone;
            this.Address = SDTO.Address;
            this.Role = SDTO.Role;

            Mode = cMode;
        }

        public int CustomerID { get; set; }
        public string name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Phone {  get; set; }
        public string Address { get; set; }

        public string Role { set; get; }


        private bool _AddNewCustomer()
        {
            //call DataAccess Layer 

            this.CustomerID = clsCustomerData.AddCustomer(SDTO);

            return (this.CustomerID != -1);
        }

        private bool _UpdateCustomer()
        {
            
            return clsCustomerData.UpdateCustomer(SDTO);
        }

        public static List<CustomerDTO> GetAllCustomers()
        {
            return clsCustomerData.GetCustomers();
        }

        public static clsCustomer Find(int ID)
        {

           CustomerDTO SDTO = clsCustomerData.GetCustomerById(ID);

            if (SDTO != null)
            //we return new object of that student with the right data
            {

                return new clsCustomer(SDTO, enMode.Update);
            }
            else
                return null;
        }

        public static clsCustomer Find(string Username, string Password)
        {
            string hashed = _HashPassword(Password);
            CustomerDTO SDTO = clsCustomerData.GetCustomerByUsernameAndPass(Username, hashed);

            if (SDTO != null)
                return new clsCustomer(SDTO, enMode.Update);
            else
                return null;
        }

        public static bool IsCustomerExist(string Username, string Password)
        {
            string hashed = _HashPassword(Password);
            return clsCustomerData.IsCustomerExist(Username, hashed);
        }

        public static bool IsCustomerExist(int ID)
        {
            return clsCustomerData.IsCustomerExist(ID);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewCustomer())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateCustomer();

            }

            return false;
        }

        public static bool DeleteCustomer(int ID)
        {
            return clsCustomerData.DeleteCustomer(ID);
        }



        private static string _HashPassword( string password)
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
