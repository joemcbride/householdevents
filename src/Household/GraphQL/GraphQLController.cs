using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using Household.Events;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Household.GraphQL
{
    public class Query
    {
        private readonly IDocumentStore _store;

        public Query(IDocumentStore store)
        {
            _store = store;
        }

        public async Task<Household.Events.Household> Household(Guid id)
        {
            using(var session = _store.QuerySession())
            {
              var result = await session.LoadAsync<Household.Events.Household>(id);
              return result;
            }
        }

        public async Task<IEnumerable<Household.Events.Household>> Households()
        {
            using(var session = _store.QuerySession())
            {
              var result = await session.Query<Household.Events.Household>().ToListAsync();
              return result.AsEnumerable();
            }
        }
    }

    public class PersonJoinedHouseholdInput
    {
        public Guid HouseholdId { get; set; }
        public string Name { get; set; }
    }

    public class Mutation
    {
        private readonly IDocumentStore _store;

        public Mutation(IDocumentStore store)
        {
            _store = store;
        }

        public Person JoinHousehold(PersonJoinedHouseholdInput joined)
        {
            using(var session = _store.LightweightSession())
            {
                // var household = new Guid("9ed0b4f7-c406-4915-be71-28044785d5a4");

                var evt = new PersonJoinedHousehold
                {
                    Person = new Person
                    {
                        Name = joined.Name
                    }
                };

                session.Events.Append(joined.HouseholdId, evt);

                session.SaveChanges();

                return evt.Person;
            }
        }
    }

    public class GraphQLController : Controller
    {
        private readonly ISchema _schema;

        public GraphQLController(ISchema schema)
        {
            _schema = schema;
        }

        [HttpPost]
        public string Index([FromBody]GraphQLRequest request)
        {
            var result = _schema.Execute(_ =>
            {
                _.Query = request.Query;
                _.OperationName = request.OperationName;
                _.Inputs = request.Variables.ToInputs();
                _.ExposeExceptions = true;
            });

            return result;
        }

        public static ISchema BuildSchema(IServiceProvider provider)
        {
            var schema = Schema.For(@"
              type Household {
                id: ID!
                version: ID!
                members: [Person!]!
              }

              type Person {
                id: ID!
                name: String!
                ssn: String
                deceased: Boolean
              }

              input PersonJoinedHouseholdInput {
                householdId: ID!
                name: String!
              }

              type Query {
                household(id: ID!): Household
                households: [Household!]!
              }

              type Mutation {
                joinHousehold(joined: PersonJoinedHouseholdInput!): Person!
              }
            ", _ =>
            {
                _.DependencyResolver = new FuncDependencyResolver(provider.GetService);
                _.Types.Include<Query>();
                _.Types.Include<Mutation>();
            });

            return schema;
        }
    }

    public class GraphQLRequest
    {
        public string OperationName { get; set; }
        public string Query { get; set; }
        public JObject Variables { get; set; }
    }
}