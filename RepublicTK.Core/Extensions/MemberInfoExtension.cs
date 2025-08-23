using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace RepublicTK.Core.Extensions
{
    public static class MemberInfoExtension
    {
        public static bool TryGetCustomAttribute<T>(this MemberInfo member, [MaybeNullWhen(false)] out T attribute)
            where T : Attribute
        {
            return (attribute = member.GetCustomAttribute<T>()!) is not null;
        }

        public static bool HasCustomAttribute<T>(this MemberInfo member)
            where T : Attribute
        {
            return member.GetCustomAttribute<T>() is not null;
        }
    }
}
