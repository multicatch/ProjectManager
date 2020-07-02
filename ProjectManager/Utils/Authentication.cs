using Microsoft.AspNetCore.Http;
using ProjectManager.Database.Models;

namespace ProjectManager.Utils
{
    public static class AuthenticationUtils
    {
        private const string CurrentUserIdKey = "CURRENT_USER_ID";

        public static void SetUser(this ISession session, User user)
        {
            session.SetInt32(CurrentUserIdKey, user.Id);
        }

        public static bool IsAuthenticated(this ISession session)
        {
            var currentUser = session.GetInt32(CurrentUserIdKey);
            if (currentUser != null) return true;
            session.Clear();
            return false;
        }
    }
}