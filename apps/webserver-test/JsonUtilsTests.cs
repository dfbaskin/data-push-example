using System.Text.Json;

namespace DataPushExample.Webserver.Test;

public class JsonUtilsTests
{
    [Theory]
    [InlineData("abc", "ABC")]
    [InlineData("ABC", "A_B_C")]
    [InlineData("SomeValue", "SOME_VALUE")]
    public void ShouldCreateUpperSnakeCase(string text, string expected)
    {
        Assert.Equal(expected, text.ToUpperSnakeCase());
    }

    [Theory]
    [InlineData(TestEnum.Some, "SOME")]
    [InlineData(TestEnum.SomeEnum, "SOME_ENUM")]
    [InlineData(TestEnum.SomeOtherEnum, "SOME_OTHER_ENUM")]
    public void ShouldConvertEnumToSnakeCase(TestEnum testEnum, string enumText)
    {
        var text = JsonSerializer.Serialize(new TestRecord(testEnum), JsonUtils.SerializerOptions);
        var result = JsonSerializer.Deserialize<TestRecord>(text, JsonUtils.SerializerOptions);
        Assert.Equal($"{{\"value\":\"{enumText}\"}}", text);
        Assert.Equal(testEnum, result?.Value);
    }

    [Fact]
    public void ShouldHandleNewtonsoftJsonIgnore()
    {
        var text = JsonSerializer.Serialize(new LegacyClass(), JsonUtils.SerializerOptions);
        Assert.Equal("{\"a\":\"aaa\"}", text);
    }

    public enum TestEnum
    {
        Some,
        SomeEnum,
        SomeOtherEnum
    };

    public record TestRecord(
        TestEnum Value
    );

    public class LegacyClass
    {
        public string A { get; set; } = "aaa";
        [Newtonsoft.Json.JsonIgnore]
        public string B { get; set; } = "bbb";
    }
}
