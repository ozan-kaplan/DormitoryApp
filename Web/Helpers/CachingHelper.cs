using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Web.Models;

namespace Web.Helpers
{
    public class CachingHelper
    {

        public static string GetCacheKey(string email)
        {
            return "User_" + email;
        }

        public static void AddUserToCache(string email, User user)
        {
            CleanUserFromCache(GetCacheKey(email));
            HttpRuntime.Cache.Insert(GetCacheKey(email), user, null, DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration);
        }


        public static User GetUserFromCache(string email)
        {

            try
            {
                if (HttpRuntime.Cache[GetCacheKey(email)] != null)
                    return (User)HttpRuntime.Cache[GetCacheKey(email)];
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        private static void CleanUserFromCache(string email)
        {
            var cacheKey = GetCacheKey(email);
            HttpRuntime.Cache.Remove(cacheKey);
        }

    }
}