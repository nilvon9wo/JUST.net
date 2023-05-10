using NUnit.Framework;

namespace JUST.UnitTests;

[TestFixture]
public class SimpleTransformTests
{
	[Test]
	public void String()
	{
		const string input = "{\"Food\": {\"Desserts\": {\"item\": [{\"name\": \"carrot cake\",\"price\": 5},{\"name\": \"ice cream\",\"price\": 10}]}}}";
		const string transformer = "\"abc\"";
		JUSTContext context = new()
		{
			EvaluationMode = EvaluationMode.Strict
		};
		string result = new JsonTransformer(context).Transform(transformer, input);

		Assert.AreEqual("\"abc\"", result);
	}

	[Test]
	public void StringWithFunction()
	{
		const string input = "{\"Food\": {\"Desserts\": {\"item\": [{\"name\": \"carrot cake\",\"price\": 5},{\"name\": \"ice cream\",\"price\": 10}]}}}";
		const string transformer = "\"#valueof($.Food.Desserts.item)\"";
		JUSTContext context = new()
		{
			EvaluationMode = EvaluationMode.Strict
		};
		string result = new JsonTransformer(context).Transform(transformer, input);

		Assert.AreEqual("[{\"name\":\"carrot cake\",\"price\":5},{\"name\":\"ice cream\",\"price\":10}]", result);
	}

	[Test]
	public void ReturnInput()
	{
		const string input = "{\"Food\": {\"Desserts\": {\"item\": [{\"name\": \"carrot cake\",\"price\": 5},{\"name\": \"ice cream\",\"price\": 10}]}}}";
		const string transformer = "\"#valueof($)\"";
		JUSTContext context = new()
		{
			EvaluationMode = EvaluationMode.Strict
		};
		string result = new JsonTransformer(context).Transform(transformer, input);

		Assert.AreEqual("{\"Food\":{\"Desserts\":{\"item\":[{\"name\":\"carrot cake\",\"price\":5},{\"name\":\"ice cream\",\"price\":10}]}}}", result);
	}

	[Test]
	public void SimpleArrayElement()
	{
		const string input = "{\"Food\": {\"Desserts\": {\"item\": [{\"name\": \"carrot cake\",\"price\": 5},{\"name\": \"ice cream\",\"price\": 10}]}}}";
		const string transformer = "[ \"#valueof($.Food.Desserts.item[*].name)\" ]";
		JUSTContext context = new()
		{
			EvaluationMode = EvaluationMode.Strict
		};
		string result = new JsonTransformer(context).Transform(transformer, input);

		Assert.AreEqual("[\"carrot cake\",\"ice cream\"]", result);
	}
}
