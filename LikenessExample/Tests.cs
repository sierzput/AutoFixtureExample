using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SemanticComparison;
using SemanticComparison.Fluent;
using Xunit;

namespace LikenessExample
{
    public class Tests
    {
        private readonly Person _person;
        private readonly Person _person2;
        private readonly Employee _employee;
        private readonly Dog _dog;
        private readonly Citizen _citizen;
        private readonly Citizen _citizen2;

        public Tests()
        {
            _person = new Person
            {
                FirstName = "Jan",
                LastName = "Kowalski",
                Age = 30,
            };

            _person2 = new Person
            {
                FirstName = "Adam",
                LastName = "Nowak",
                Age = 35,
            };

            _employee = new Employee
            {
                FirstName = "Jan",
                LastName = "Kowalski",
                Age = 30,
            };

            _dog = new Dog
            {
                Name = "Burek",
                Age = 30,
            };

            _citizen = new Citizen
            {
                FirstName = "Jan",
                LastName = "Kowalski",
                Address = new Address
                {
                    City = "Gdańsk",
                    Street = "Marynarki Polskiej 163"
                }
            };

            _citizen2 = new Citizen
            {
                FirstName = "Adam",
                LastName = "Kowalski",
                Address = new Address
                {
                    City = "Gdańsk",
                    Street = "Marynarki Polskiej 163"
                }
            };
        }

        [Fact]
        public void EmployeePersonCompareTest()
        {
            var likeness = _employee.AsSource().OfLikeness<Person>();

            likeness.ShouldEqual(_person);
        }

        [Fact]
        public void EmployeePersonCompare2Test()
        {
            var likeness = _employee.AsSource().OfLikeness<Person>();

            likeness.ShouldEqual(_person2);
        }

        [Fact]
        public void PersonDogCompareTest()
        {
            var likeness = _person.AsSource().OfLikeness<Dog>().Without(dog => dog.Name);

            likeness.ShouldEqual(_dog);
        }

        [Fact]
        public void PersonEmployeeCompare3Test()
        {
            var likeness = _person.AsSource().OfLikeness<Employee>().OmitAutoComparison()
                .WithDefaultEquality(employee => employee.FirstName)
                .WithDefaultEquality(employee => employee.LastName);

            likeness.ShouldEqual(_employee);
        }

        [Fact]
        public void CitizenCompareTest()
        {
            var likeness = _citizen.AsSource().OfLikeness<Citizen>()
                .Without(citizen => citizen.FirstName)
                .With(c => c.Address).EqualsWhen((citizen1, citizen2) => citizen1.Address.AsSource().OfLikeness<Address>().Equals(citizen2.Address));

            likeness.ShouldEqual(_citizen2);
        }

        [Fact]
        public void CompareWithProxyTest()
        {
            Person proxy = _employee.AsSource().OfLikeness<Person>().CreateProxy();

            proxy.Should().BeEquivalentTo(_person);
        }

        [Fact]
        public void EmployeePersonCollectionCompareTest()
        {
            var employees = new List<Employee> { _employee, _employee, _employee };
            var persons = new List<Person> { _person, _person, _person };
            var likenesses = employees.AsSourceOfCollectionLikeness<Employee, Person>();

            likenesses.ShouldEqual(persons);
        }
    }

    public static class LikenessExtensions
    {
        public static void ShouldEqual<TSource, TDestination>(this IEnumerable<Likeness<TSource, TDestination>> likenesses, IEnumerable<TDestination> destinations)
        {
            likenesses.Count().Should().Be(destinations.Count());

            using (var likenessEnumerator = likenesses.GetEnumerator())
            using (var destinationEnumerator = destinations.GetEnumerator())
            {
                while (likenessEnumerator.MoveNext() && destinationEnumerator.MoveNext())
                {
                    likenessEnumerator.Current.ShouldEqual(destinationEnumerator.Current);
                }
            }
        }

        public static IEnumerable<Likeness<TSource, TDestination>> AsSourceOfCollectionLikeness<TSource, TDestination>(this IEnumerable<TSource> sources)
        {
            return sources.Select(source => new LikenessSource<TSource>(source))
                .Select(source => source.OfLikeness<TDestination>());
        }
    }
}
