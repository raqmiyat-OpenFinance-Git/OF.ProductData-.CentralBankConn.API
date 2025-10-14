namespace OF.ProductData.Common.Custom;

[AttributeUsage(AttributeTargets.Property)]
public class RequestOptionsAttribute : Attribute
{
    public int SortingOrder = 0;
    public string AttrFieldType = "C";
    public RequestOptionsAttribute() { }
    public RequestOptionsAttribute(int sortingOrder, string attrFieldType)
    {
        AttrFieldType = attrFieldType;
        SortingOrder = sortingOrder;
    }
}
