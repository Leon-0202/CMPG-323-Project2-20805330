namespace RESTfullAPI.Data_Transfer_Objects
{
    public class OrderDetailDTO
    {
        public short OrderDetailsId { get; set; }
        public short OrderId { get; set; }
        public short ProductId { get; set; }
        public int Quantity { get; set; }
        public int? Discount { get; set; }
    }
}
