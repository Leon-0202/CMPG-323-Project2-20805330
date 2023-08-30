namespace RESTfullAPI.Data_Transfer_Objects
{
    public class OrderDTO
    {
        public short OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public short CustomerId { get; set; }
        public string DeliveryAddress { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}
