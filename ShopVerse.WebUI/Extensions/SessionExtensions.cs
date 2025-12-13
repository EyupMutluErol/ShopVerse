using System.Text.Json;

namespace ShopVerse.WebUI.Extensions;

public static class SessionExtensions
{
    // Nesneyi jsona çevirme
    public static void SetJson(this ISession session, string key,object value)
    {
        session.SetString(key,JsonSerializer.Serialize(value));
    }

    // Json'ı nesneye çevirme
    public static T GetJson<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
    }
}
