using Microsoft.AspNetCore.Http;

namespace InternetCafeManagementSystem.Extensions
{
    public static class SessionExtensions
    {
        public static void SetBoolean(this ISession session, string key, bool value)
        {
            session.SetInt32(key, value ? 1 : 0);
        }

        public static bool GetBoolean(this ISession session, string key)
        {
            var value = session.GetInt32(key);
            return value == 1;
        }
    }
}