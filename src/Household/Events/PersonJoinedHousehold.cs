using System;
using System.Collections.Generic;
using System.Linq;
using Marten.Events;

namespace Household.Events
{
    public class PersonJoinedHousehold
    {
        public Person Person { get; set; }
    }

    public class SSNChange
    {
        public Guid PersonId { get; set; }
        public string SSN { get; set; }
    }

    public class PersonDeceased
    {
        public Guid PersonId { get; set; }
    }

    public class Household
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public List<Person> Members { get; set; } = new List<Person>();

        public void Apply(Event<PersonJoinedHousehold> joinedEvent)
        {
            WithMeta(joinedEvent);

            Members.Add(joinedEvent.Data.Person);
        }

        public void Apply(Event<SSNChange> ssnChangeEvent)
        {
            WithMeta(ssnChangeEvent);

            var person = Members.FirstOrDefault(x => x.Id == ssnChangeEvent.Data.PersonId);
            if(person != null)
            {
                person.SSN = ssnChangeEvent.Data.SSN;
            }
        }

        public void Apply(Event<PersonDeceased> deceasedEvent)
        {
            WithMeta(deceasedEvent);

            var person = Members.FirstOrDefault(x => x.Id == deceasedEvent.Data.PersonId);
            if(person != null)
            {
                person.Deceased = true;
            }
        }

        private void WithMeta<T>(Event<T> evt)
        {
            Version = evt.Version;
        }
    }

    public class Person
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string SSN { get; set; }
        public bool Deceased { get; set; }
    }
}