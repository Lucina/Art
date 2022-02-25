using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace Art.Tests;

public class HttpArtifactToolTests : HttpTestsBase
{
    [Test]
    public async Task GetDeserializedJsonAsync_NullJson_IsNull()
    {
        MockHandler.When("http://localhost/imaginaryfriend.json").Respond("application/json", "null");
        var x = await Tool.GetDeserializedJsonAsync<JsonElement?>("http://localhost/imaginaryfriend.json");
        Assert.That(x, Is.Null);
    }

    [Test]
    public async Task GetDeserializedJsonAsync_NotNullJson_IsNotNull()
    {
        MockHandler.When("http://localhost/watashironri.json").Respond("application/json", @"{""kaf"":""epic""}");
        var x = await Tool.GetDeserializedJsonAsync<JsonElement?>("http://localhost/watashironri.json");
        Assert.That(x, Is.Not.Null);
    }
}
