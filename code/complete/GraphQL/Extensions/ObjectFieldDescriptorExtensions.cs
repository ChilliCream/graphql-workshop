using HotChocolate.Types;

namespace ConferencePlanner.GraphQL
{
    public static class ObjectFieldDescriptorExtensions
    {
        public static IObjectFieldDescriptor UseUpperCase(
            this IObjectFieldDescriptor descriptor)
        {
            // TODO : we need a better API for the user.
            descriptor.Extend().Definition.ResultConverters.Add(
                new((_, result) =>
                {
                    if (result is string s)
                    {
                        return s.ToUpperInvariant();
                    }
                    return result;
                }));

            return descriptor;
        }
    }
}