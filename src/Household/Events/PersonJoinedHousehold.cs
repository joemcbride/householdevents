using System;

namespace Household.Events
{
    public class PersonJoinedHousehold
    {
        public Guid HouseholdId { get; set; }
        public Person Person { get; set; }
    }
}