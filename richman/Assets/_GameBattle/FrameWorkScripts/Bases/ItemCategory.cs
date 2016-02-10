/// <summary>
/// ItemCategory base class
/// </summary>
public abstract class ItemCategory
{
    public readonly string mName = "";
    public readonly string mID = "";

    public ItemCategory(string name)
    {
        mName = name;
        mID = MD5Algorithm.GetMd5Hash(name);
    }
}