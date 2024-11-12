using System.Reflection;
using HotChocolate.Types.Descriptors;

namespace ConferencePlanner.GraphQL.Extensions;

public sealed class UseUpperCaseAttribute : ObjectFieldDescriptorAttribute
{
    protected override void OnConfigure(
        IDescriptorContext context,
        IObjectFieldDescriptor descriptor,
        MemberInfo member)
    {
        descriptor.UseUpperCase();
    }
}
