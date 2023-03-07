
namespace ExpressionHelper.Attribute;

public class RuleAttribute: System.Attribute
{
    public string Name { get; }
    public RuleAttribute(string name)
    {
        Name = name;
    }
}