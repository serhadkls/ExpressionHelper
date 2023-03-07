namespace ExpressionHelper.Helper;

public static class StringExtensions
{
    private static string FirstCharToUpper(this string input)
    {
        return input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => input.First().ToString().ToUpperInvariant() + input.Substring(1)
        };
    }
        
    public static string FirstCharToUpperBySeparator(this string input, char separator = '.')
    {
        var  restOfArray = string.Join(separator, input.Split(separator)
            .Select(s => s.FirstCharToUpper()));
        return restOfArray;

    }
       
    private static string FirstCharToLower(this string input)
    {
        return input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => input.First().ToString().ToLowerInvariant() + input.Substring(1)
        }; 
    }
        
    public static string FirstCharToLowerBySeparator(this string input, char separator = '.')
    {
        var  restOfArray = string.Join(separator, input.Split(separator)
            .Select(s => s.FirstCharToLower()));
        return restOfArray;

    }
    
}