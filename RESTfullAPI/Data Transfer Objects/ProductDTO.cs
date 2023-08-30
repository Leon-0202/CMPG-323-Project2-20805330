namespace RESTfullAPI.Data_Transfer_Objects
{
    public class ProductDTO
    {
        public short ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int? UnitsInStock { get; set; }
    }
}
