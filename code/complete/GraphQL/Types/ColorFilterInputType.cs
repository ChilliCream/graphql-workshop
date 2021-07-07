using HotChocolate.Data.Filters;
using System.Drawing;

namespace ConferencePlanner.GraphQL.Types
{
    public class ColorFilterInputType : FilterInputType<Color>
    {
        protected override void Configure(IFilterInputTypeDescriptor<Color> descriptor)
        {
        }
    }
}
