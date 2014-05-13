using System;
using System.Linq.Expressions;

namespace ManageAzure.Lib
{
    /// <summary>
    /// Utility class that has functions used by all other classes int the library..
    /// </summary>
    public static class MemberUtils
    {

        /// <summary>
        /// Uses Linq based member expression to return the property name requires explicit specification of object type - i.e. we do not need the instance..
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="exp"></param>
        /// <returns>the name of the property as a string</returns>
        public static string GetPropertyName<TObject>(Expression<Func<TObject, object>> exp)
        {
            return (((MemberExpression)(exp.Body)).Member).Name;
        }

    }
}
