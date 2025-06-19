namespace BTL.ViewModel
{
    public class CartModel
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public int Quantity { get; set; }
        public double Total { get; set; }
    }
}
