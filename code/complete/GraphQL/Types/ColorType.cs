using HotChocolate.Types;
using System.Drawing;

namespace ConferencePlanner.GraphQL.Types
{
    public class ColorType : ObjectType<Color>
    {
        protected override void Configure(IObjectTypeDescriptor<Color> descriptor)
        {
            descriptor
                .Name("DummyColor");
        }
    }
}
