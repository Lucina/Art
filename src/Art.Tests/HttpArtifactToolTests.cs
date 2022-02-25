using System;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace Art.Tests;

public class HttpArtifactToolTests : HttpTestsBase
{
    [Test]
    public async Task GetDeserializedJsonAsync_NullJson_Null()
    {
        MockHandler.When("http://localhost/imaginaryfriend.json").Respond("application/json", "null");
        var x = await Tool.GetDeserializedJsonAsync<JsonElement?>("http://localhost/imaginaryfriend.json");
        Assert.That(x, Is.Null);
    }

    [Test]
    public async Task GetDeserializedJsonAsync_NotNullJson_NotNull()
    {
        MockHandler.When("http://localhost/watashironri.json").Respond("application/json", @"{""kaf"":""epic""}");
        var x = await Tool.GetDeserializedJsonAsync<JsonElement?>("http://localhost/watashironri.json");
        Assert.That(x, Is.Not.Null);
    }

    [Test]
    public async Task GetDeserializedRequiredJsonAsync_NullJsonToJsonElement_NullValue()
    {
        MockHandler.When("http://localhost/imaginaryfriend.json").Respond("application/json", "null");
        JsonElement res = await Tool.GetDeserializedRequiredJsonAsync<JsonElement>("http://localhost/imaginaryfriend.json");
        Assert.That(res.ValueKind, Is.EqualTo(JsonValueKind.Null));
    }

    [Test]
    public void GetDeserializedRequiredJsonAsync_NullJsonToNullableJsonElement_Throws()
    {
        MockHandler.When("http://localhost/imaginaryfriend.json").Respond("application/json", "null");
        Assert.That(() => Tool.GetDeserializedRequiredJsonAsync<JsonElement?>("http://localhost/imaginaryfriend.json").Wait(),
            Throws.InstanceOf<AggregateException>().With.InnerException.InstanceOf<NullJsonDataException>());
    }
}
