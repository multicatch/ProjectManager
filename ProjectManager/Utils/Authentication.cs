using System;
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
            var currentUser = GetCurrentUserId(session);
            if (currentUser != null) return true;
            session.Clear();
            return false;
        }

        public static int? GetCurrentUserId(this ISession session)
        {
            return session.GetInt32(CurrentUserIdKey);
        }
    }
}