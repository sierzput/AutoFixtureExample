using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace LikenessExample
{
    public class AutoFixtureNSubstituteExampleTests
    {
        private readonly ITestOutputHelper _helper;
        private readonly Fixture _fixture;

        public AutoFixtureNSubstituteExampleTests(ITestOutputHelper helper)
        {
            _helper = helper;
            _fixture = new Fixture();
        }

        [Fact]
        public void WhenMembersAreNotConfigured()
        {
            _fixture.Customize(new AutoNSubstituteCustomization());
            var person = _fixture.Create<IPerson>(); // IPersonProxy {Name:"", Age:0}

            _helper.WriteLine($"{person.GetType().Name} Name:{person.Name}, Age:{person.Age}");
            person.Name.Should().BeNullOrEmpty();
            person.Age.Should().Be(0);
        }

        [Fact]
        public void WhenMembersAreConfigured()
        {
            _fixture.Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });
            var person = _fixture.Create<IPerson>(); // IPersonProxy {Name:"Name74930fb9-a8fd-4ebd-9fa7-6355dcb2544c", Age:28}

            _helper.WriteLine($"{person.GetType().Name} Name:{person.Name}, Age:{person.Age}");
            person.Name.Should().NotBeNullOrEmpty();
            person.Age.Should().NotBe(0);
        }
    }

    public interface IPerson
    {
        string Name { get; set; }
        int Age { get; set; }
    }
}