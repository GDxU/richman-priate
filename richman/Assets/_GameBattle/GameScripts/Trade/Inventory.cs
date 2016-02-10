using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Inventory
/// </summary>
public class Inventory
{
    public enum Status
    {
        AddOK, AddError, RemoveOK, RemoveError, 
    }

    public int mLimitSlotSize = 0;  // limit slot size;
    protected Dictionary<int, Slot> mSlotList = new Dictionary<int, Slot>();   //slot list;

    public Inventory(int limitSlotSize)
    {
        AddSlots(limitSlotSize);
    }

    /// <summary>
    /// Add slots in this inventory;
    /// </summary>
    public void AddSlots(int extraSlotCount)
    {
        int id = mSlotList.Count;
        for (int i = 0; i < extraSlotCount; i++)
        {
            int slotID = id + i;
            Slot slot = new Slot(slotID);
            mSlotList.Add(slotID, slot);
        }
    }

    /// <summary>
    /// Add item into inventory
    /// </summary>
    public Status AddItem(Item item)
    {
        foreach (KeyValuePair<int, Slot> keyValuePair in mSlotList)
        {
            Slot slot = keyValuePair.Value;
            Slot.Status slotStatus = slot.AddItem(item);

            switch (slotStatus)
            {
                case Slot.Status.AddOK:
                    return Status.AddOK;

                case Slot.Status.AddError:
                case Slot.Status.AddError_FullSlot:
                case Slot.Status.AddError_NotSameCategory:
                    break;
            }
        }
        return Status.AddError;
    }

    /// <summary>
    /// remove Item;
    /// </summary>
    public Status RemoveItem(Item item)
    {
        foreach (KeyValuePair<int, Slot> keyValuePair in mSlotList)
        {
            Slot slot = keyValuePair.Value;
            Slot.Status slotStatus = slot.RemoveItem(item);

            switch (slotStatus)
            {
                case Slot.Status.RemoveOK:
                    return Status.RemoveOK;

                case Slot.Status.RemoveError:
                case Slot.Status.RemoveError_EmptySlot:
                case Slot.Status.RemoveError_DifferentCategory:
                    break;
            }
        }
        return Status.RemoveError;
    }
    
    /// <summary>
    /// Slot class;
    /// </summary>
    protected class Slot
    {
        public enum Status
        {
            AddOK, AddError, AddError_FullSlot, AddError_NotSameCategory,
            RemoveOK, RemoveError, RemoveError_EmptySlot, RemoveError_DifferentCategory,
        }
        public static readonly int DefaultLimitSize = 99;
        public readonly int ID;
        public List<Item> mItems = new List<Item>();
        public readonly int mLimitSize;

        public Slot(int id)
        {
            ID = id;
            mLimitSize = DefaultLimitSize;
        }
        public Slot(int id, int size)
        {
            ID = id;
            mLimitSize = size;
        }

        /// <summary>
        /// Add item into this slot;
        /// </summary>
        public Status AddItem(Item item)
        {
            if (mItems.Count == 0)
            {
                mItems.Add(item);
                return Status.AddOK;
            }
            else if (item == mItems[0])//(item.mCategory != item.mCategory)
            {
                return Status.AddError_NotSameCategory;
            }
            else
            {
                if (mLimitSize >= mItems.Count)
                {
                    return Status.AddError_FullSlot;
                }

                mItems.Add(item);
                return Status.AddOK;
            }
        }

        /// <summary>
        /// remove item from slot
        /// </summary>
        public Status RemoveItem(Item item)
        {
            if (mItems.Count == 0)
                return Status.RemoveError_EmptySlot;

            if (item == mItems[0])//(item.mCategory != item.mCategory)
                return Status.RemoveError_DifferentCategory;

            mItems.Remove(item);
            return Status.RemoveOK;
        }
    }
}
