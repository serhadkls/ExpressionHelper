``
public class TestModel {

public Collection Collections { get; set;}
}

public class Collection{

public Price Prices {get; set;}
}

public class Price{

public int Amount { get; set;}
}
``



#Example 

var queryRuleSets = new List<QueryRuleSet> 
{
  new QueryRuleSet
  {
    Field = "Collections.Prices.Amount",
    Comparison = Comparison.Eq,
    Value  = = new List<QueryRuleValueType>()
                        {
                            new()
                            {
                                Value = 100
                            }
                        }
  }
}

var predicate = new ExpressionCall().ToExpression<TestModel>(queryRuleSets);
//predicate == Param1 => Param1.Collections.Prices.Amount == 100
var predicateResult = CollectionQuery.AsQueryable().Where(predicate).ToList();




