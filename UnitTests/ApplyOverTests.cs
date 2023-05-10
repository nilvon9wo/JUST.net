using NUnit.Framework;

namespace JUST.UnitTests;

[TestFixture]
public class ApplyOverTests
{
	[Test]
	public void ApplyOverInputRetake()
	{
		const string input = "{\"d\": [ \"one\", \"two\", \"three\" ], \"values\": [ \"z\", \"c\", \"n\" ]}";
		const string transformer = "{ \"result\": \"#applyover({ 'condition': { '#loop($.values)': { 'test': '#ifcondition(#stringcontains(#valueof($.d[0]),#currentvalue()),True,yes,no)' } } }, '#exists($.condition[?(@.test=='yes')])')\", \"after_result\": \"#valueof($.d[0])\" }";
		JUSTContext context = new()
		{
			EvaluationMode = EvaluationMode.Strict
		};
		string result = new JsonTransformer(context).Transform(transformer, input);

		Assert.AreEqual("{\"result\":true,\"after_result\":\"one\"}", result);
	}

	[Test]
	public void ObjectTransformationResult()
	{
		const string input = "{ \"data\": [ { \"saleStatus\": 1, \"priority\": \"normal\", \"other\": \"one\" }, { \"saleStatus\": 2, \"priority\": \"high\", \"other\": \"two\" }, { \"saleStatus\": 1, \"priority\": \"normal\", \"other\": \"three\" } ] }";
		const string transformer = "{ \"result\": \"#applyover({ 'temp': '#grouparrayby($.data,saleStatus:priority,all)' }, { '#loop($.temp)': { 'count': '#length($.all)' } })\" }";
		JUSTContext context = new()
		{
			EvaluationMode = EvaluationMode.Strict
		};
		string result = new JsonTransformer(context).Transform(transformer, input);

		Assert.AreEqual("{\"result\":[{\"count\":2},{\"count\":1}]}", result);
	}

	[Test]
	public void ArrayTransformationResult()
	{
		const string input = "{ \"data\": [ { \"saleStatus\": 1, \"priority\": \"normal\", \"other\": \"one\" }, { \"saleStatus\": 2, \"priority\": \"high\", \"other\": \"two\" }, { \"saleStatus\": 1, \"priority\": \"normal\", \"other\": \"three\" } ] }";
		const string transformer = "{ \"result\": \"#applyover({ 'temp': { '#loop($.data)': { 'field': '#currentvalueatpath($.other)' } } }, { '#loop($.temp)': '#currentvalueatpath($.field)' })\" }";
		JUSTContext context = new()
		{
			EvaluationMode = EvaluationMode.Strict
		};
		string result = new JsonTransformer(context).Transform(transformer, input);

		Assert.AreEqual("{\"result\":[\"one\",\"two\",\"three\"]}", result);
	}
}
