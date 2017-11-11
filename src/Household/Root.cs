using System;
using System.Collections.Generic;
using Household.Events;

namespace Household
{
    public class Household
    {
        public Guid Id { get; set; }
        public List<Person> Members { get; set; } = new List<Person>();

        public void Apply(PersonJoinedHousehold joined)
        {
            Members.Add(joined.Person);
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public string SSN { get; set; }
        public bool Deceased { get; set; }
    }
}