using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceDataAccessLayer;

namespace E_CommerceBussinessLayer
{
    public class clsProduct
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public ProductDTO SDTO
        {
            get { return (new ProductDTO(this.ProductID, this.name, this.Description, this.Price, this.QuantityInStock, this.notes, this.ImageURL)); }

        }

        public clsProduct(ProductDTO SDTO, enMode cMode = enMode.AddNew)

        {
            this.ProductID = SDTO.ProductID;
            this.name = SDTO.name;
            this.Description = SDTO.Description;
            this.Price = SDTO.Price;
            this.QuantityInStock = SDTO.QuantityInStock;
            this.notes = SDTO.notes;
            this.ImageURL = SDTO.ImageURL;
            Mode = cMode;
        }

        public int ProductID { get; set; }
        public string name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int QuantityInStock { get; set; }
        public string notes { get; set; }
        public string ImageURL { set; get; }

        public static List<ProductDTO> GetProducts()
        {
            return clsProductData.GetProducts();
        }

        public static clsProduct Find(int ID)
        {

            ProductDTO SDTO = clsProductData.GetProductById(ID);

            if (SDTO != null)
            //we return new object of that student with the right data
            {

                return new clsProduct(SDTO, enMode.Update);
            }
            else
                return null;
        }

        public static bool IsProductExist(int ProductID)
        {
            return clsProductData.IsProductExist(ProductID);
        }

        private bool _AddNewProduct()
        {
            //call DataAccess Layer 

            this.ProductID = clsProductData.AddProduct(SDTO);

            return (this.ProductID != -1);
        }

        private bool _UpdateProduct()
        {

            return clsProductData.UpdateProduct(SDTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewProduct())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateProduct();

            }

            return false;
        }

        public static bool DeleteProduct(int ProductID)
        {
            return clsProductData.DeleteProduct(ProductID);
        }
    }
}
