/// <summary>
/// Item base class
/// </summary>
public abstract class Item
{
    public enum ItemType { Normal, CanNotSell, CanNotBuy, CanNotDestory }
    public readonly string mName = "";
    public readonly int mBuyPrize = 0;
    public readonly int mSellPrize = 0;
    public readonly ItemCategory mCategory = null;
    public ItemType mType = Item.ItemType.Normal;
    
    public Item(string name, int buyPrize, int sellPrize, ItemCategory itemCategory)
    {
        mName = name;
        mBuyPrize = buyPrize;
        mSellPrize = sellPrize;
        mCategory = itemCategory;
    }
}


