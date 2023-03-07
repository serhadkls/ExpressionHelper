using System.Collections;
using ExpressionHelper.Attribute;
using ExpressionHelper.Model;
using ExpressionHelper.Operation;

namespace ExpressionHelperUnitTest;


public class TestClass
{
    public static IEnumerable GetExpressionWhenEnumValueData
    {
        get
        {
            yield return new TestCaseData
            (
                new List<QueryRuleSet>()
                {
                    new()
                    {
                        Field = FieldEnums.Segment.GetAttribute<RuleAttribute>()?.Name,
                        Comparison = Comparison.Eq,
                        Values = new List<QueryRuleValueType>()
                        {
                            new()
                            {
                                Value = "VC"
                            }
                        }
                    }
                }
            );
        }
    }
    
    public static IEnumerable<TestModel> GetExpressionWhenEnum =>
        new List<TestModel>()
        {
            new()
            {
                NullableSegment = Segments.VC,
                Segment = Segments.VC
            } 
        };
}
public enum FieldEnums
{
    #region ForUnitTest

    [Rule("StringId")] StringId,
    [Rule("NullableSegment")] NullableSegment,
    [Rule("Segment")] Segment,

    [Rule("IntegerId")] IntegerId,
    [Rule("DecimalId")] DecimalId,

    [Rule("Price.Amount")] PriceAmount,

    [Rule("Prices.Amount")] PricesAmount,

    [Rule("Collection.Price.Amount")] CollectionPriceAmount,

    [Rule("Collection.Prices.Amount")] CollectionPricesAmount,

    [Rule("Collections.Price.Amount")] CollectionsPriceAmount,

    [Rule("Collections.Prices.Amount")]
    CollectionsPricesAmount,

    [Rule("SitePolicy.Collection.Price.Amount")]
    SitePolicyCollectionPriceAmount,

    [Rule("SitePolicy.Collections.Price.Amount")]
    SitePolicyCollectionsPriceAmount,

    [Rule("SitePolicy.Collection.Prices.Amount")]
    SitePolicyCollectionPricesAmount,

    [Rule("SitePolicy.Collections.Prices.Amount")]
    SitePolicyCollectionsPricesAmount,

    [Rule("SitePolicies.Collection.Price.Amount")]
    SitePoliciesCollectionPriceAmount,

    [Rule("SitePolicies.Collections.Price.Amount")]
    SitePoliciesCollectionsPriceAmount,

    [Rule("SitePolicies.Collection.Prices.Amount")]
    SitePoliciesCollectionPricesAmount,

    [Rule("SitePolicies.Collections.Prices.Amount")]
    SitePoliciesCollectionsPricesAmount,

    [Rule("Currency.tr")] CurrencyTr,

    [Rule("Price.Currency.tr")] PriceCurrencyTr,

    [Rule("Prices.Currency.tr")] PricesCurrencyTr,

    [Rule("Collection.Price.Currency.tr")]
    CollectionPriceCurrencyTr,

    [Rule("Collection.Prices.Currency.tr")]
    CollectionPricesCurrencyTr,

    [Rule("Collections.Prices.Currency.tr")]
    CollectionsPricesCurrencyTr,

    [Rule("UnitPrice.tr.Amount")] UnitPriceTrAmount,

    [Rule("UnitPrices.tr.Prices.Amount")]
    UnitPriceTrPricesAmount,

    [Rule("SitePolicies.CollectionDict.tr.Prices.Amount")]
    SitePoliciesCollectionDictTrPricesAmount,

    [Rule("SitePolicy.Collections.Price.Units.Value")]
    SitePolicyCollectionsPriceUnitsValue,

    [Rule("SitePolicy.Prices.UnitAmount")]
    SitePolicyPricesUnitAmountValue,
    
    [Rule("IsInCampaign")]
    IsInCampaign

    #endregion
}

public class Unit
{
    public int Value { get; set; }
}

public class SitePolicy
{
    public Collection Collection { get; set; }
    public List<Collection> Collections { get; set; }
    public Dictionary<string, Collection> CollectionDict { get; set; }
    public List<Price> Prices { get; set; }
}

public class Collection
{
    public Price Price { get; set; }
    public List<Price> Prices { get; set; }
}

public class Price
{
    public int Amount { get; set; }
    public List<Unit> Units { get; set; }
    public decimal? UnitAmount { get; set; }
}

public enum Segments
{
    VC
}

public class TestModel
{
    public string StringId { get; set; }
    public int IntegerId { get; set; }

    public decimal? DecimalId { get; set; }
    public Price Price { get; set; }
    public Collection Collection { get; set; }
    public SitePolicy SitePolicy { get; set; }
    public List<Price> Prices { get; set; }
    public List<Collection> Collections { get; set; }
    public List<SitePolicy> SitePolicies { get; set; }

    public Dictionary<string, Price> UnitPrice { get; set; }
    public Dictionary<string, Collection> UnitPrices { get; set; }
    public Segments? NullableSegment { get; set; }
    public Segments Segment { get; set; }

    public bool? IsInCampaign { get; set; }
}