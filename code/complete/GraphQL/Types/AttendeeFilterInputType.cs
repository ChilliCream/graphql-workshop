using ConferencePlanner.GraphQL.Data;
using HotChocolate.Data.Filters;

namespace ConferencePlanner.GraphQL.Types
{
    public class AttendeeFilterInputType : FilterInputType<Attendee>
    {
        protected override void Configure(IFilterInputTypeDescriptor<Attendee> descriptor)
        {
            descriptor.Field(l => l.Color).Type<ColorFilterInputType>();
        }
    }
}
