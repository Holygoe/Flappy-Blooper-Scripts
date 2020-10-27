using System;
using FlappyBlooper;

[Serializable]
public class LimitedOffer : IOffer
{
    private IOffer _offer;
    private LimitedOfferMaster _offerMaster;
    private int _offerIndex;

    public LimitedOffer(IOffer offer, LimitedOfferMaster offerMaster, int offerIndex)
    {
        _offer = offer;
        _offerMaster = offerMaster;
        _offerIndex = offerIndex;
    }

    public LimitedOfferData Data => _offerMaster.GetOfferData(_offerIndex);
    public int Stock => Data.stock;
    Product IOffer.Product => _offer.Product;
    bool IOffer.AvailableInStock => Stock > 0 && ItemContainer.AvailableForPurchase;

    public ItemTarget ItemContainer => _offer.ItemContainer;

    void IOffer.Purchase(PurchaseOfferCallback callback)
    {
        var data = Data;

        if (data.stock <= 0)
        {
            callback.Invoke(false);
            return;
        }
        
        _offer.Purchase((success) =>
        {
            if (success)
            {
                data.stock--;
                DataManager.SaveData();
            }
            
            callback.Invoke(success);
        });
    }

    ProductCard IOffer.GetCardPrefab()
    {
        return _offer.GetCardPrefab();
    }
}
