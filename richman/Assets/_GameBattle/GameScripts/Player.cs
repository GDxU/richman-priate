using System;
using System.Collections.Generic;

public class Player
{
    public enum Status
    {
       BuyOK, BuyError, BuyError_NotEnoughGold, BuyError_ItemCanNotBuy,
       SellOK, SellError, SellError_ItemCanNotBeSold,
       DestoryOK, DestoryError, DestoryError_CanNotDestory,
    }
    
    string mName = "";
    int mGold = 0;

#region Exp
    protected int mExp = 0; //experience;
    public int Exp { get { return mExp; } }
    public void EarnExp(int exp)    { mExp += exp; }

    protected static List<int> LevelUpStandard = new List<int>() { 100, 1000, 2000, 5000, 10000, };
    public int Level
    {
        get
        {
            int level = 0;
            for (int i = 0; i < LevelUpStandard.Count; i++)
            {
                if (mExp >= LevelUpStandard[i])
                    level = i;
                else
                    return level;
            }
            return level;
        }
    }
#endregion

#region Inventory
    static readonly int DefaultSlotCount = 9;
    public Inventory mInventory = new Inventory(DefaultSlotCount);
#endregion

    public Player(string name, int gold)
    {
        mName = name;
        mGold = gold;
    }

    public Player(String name, int gold, int slotCount)
    {
        mName = name;
        mGold = gold;
        mInventory = new Inventory(slotCount);
    }

#region Inventory
    /// <summary>
    /// Buy Item
    /// </summary>
    public Status BuyItem(Item item)
    {
        if(item.mType == Item.ItemType.CanNotBuy)
            return Status.BuyError_ItemCanNotBuy;

        if (item.mSellPrize > mGold)
            return Status.BuyError_NotEnoughGold;

        Inventory.Status res = mInventory.AddItem(item);
        if (res == Inventory.Status.AddOK)
        {
            mGold -= item.mBuyPrize;
            return Status.BuyOK;
        }
        else
            return Status.BuyError;
    }

    /// <summary>
    /// SellItem
    /// </summary>
    public Status SellItem(Item item)
    {
        if (item.mType == Item.ItemType.CanNotSell)
            return Status.SellError_ItemCanNotBeSold;

        Status res = RemoveItemFromInventory(item);
        if(res == Status.DestoryOK)
        {
            mGold += item.mSellPrize;
            res = Status.SellOK;
        }
        else
        {
            res = Status.SellError;
        }
        return res;
    }

    /// <summary>
    /// Destory item;
    /// </summary>
    public Status DestoryItem(Item item)
    {
        if (item.mType == Item.ItemType.CanNotDestory)
            return Status.DestoryError_CanNotDestory;

        return RemoveItemFromInventory(item);
    }

    /// <summary>
    /// Remove item from inventory
    /// </summary>
    private Status RemoveItemFromInventory(Item item)
    {
        Inventory.Status res = mInventory.RemoveItem(item);
        if (res == Inventory.Status.RemoveOK)
        {
            return Status.DestoryOK;
        }
        else
            return Status.DestoryError;
    }
#endregion
}
