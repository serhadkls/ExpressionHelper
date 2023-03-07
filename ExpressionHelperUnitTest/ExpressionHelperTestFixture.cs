using System.Collections;
using ExpressionHelper.Attribute;
using ExpressionHelper.Model;
using ExpressionHelper.Operation;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace ExpressionHelperUnitTest;

[TestFixture]
public class ExpressionHelperTestFixture
{
    [Test]
    [TestCaseSource(typeof(TestClass), nameof(TestClass.GetExpressionWhenEnumValueData))]
    public void GetExpressionWhenEnumValue(List<QueryRuleSet> queryRuleSets)
    {
        var predicate = new ExpressionCall().ToExpression<TestModel>(queryRuleSets);
        var expectedResult = TestClass.GetExpressionWhenEnum.AsQueryable().Where(predicate).ToList();
        var actualResult = TestClass.GetExpressionWhenEnum.Where(q =>
                q.NullableSegment == Segments.VC && q.Segment == Segments.VC )
            .ToList();
        Assert.Equals(expectedResult.Count, actualResult.Count);
    }
}
