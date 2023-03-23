namespace DecisionTree.Extensions;

public static class ArrayExtensions
{
    public static bool IsNotNullAndAny<T>(this IEnumerable<T>? listObject) where T:class
    {
        if (listObject != null && listObject.Any())
        {
            return true;
        }
        
        return false;
    }
    
    public static bool IsNullOrNotAny<T>(this IEnumerable<T>? listObject) where T:class
    {
        if (listObject == null || !listObject.Any())
        {
            return true;
        }
        
        return false;
    }
}