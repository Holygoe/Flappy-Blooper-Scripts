namespace FlappyBlooper
{
    public interface IOffer
    {
        ItemTarget ItemContainer { get; }
        Product Product { get; }
        bool AvailableInStock { get; }
        void Purchase(PurchaseOfferCallback callback);
        ProductCard GetCardPrefab();
    }
    
    public delegate void PurchaseOfferCallback(bool success);
}