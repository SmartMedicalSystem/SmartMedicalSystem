using System;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Testing.Domain
{
    public class SessionTests
    {
        [Fact]
        public void Constructor_Valid_SetsProperties()
        {
            var date = DateTime.UtcNow;
            var s = new Session(1,2,3,date,"note");
            s.PatientId.Should().Be(1);
            s.DoctorId.Should().Be(2);
            s.DeptId.Should().Be(3);
            s.SessionDate.Should().Be(date);
            s.Notes.Should().Be("note");
        }

        [Fact]
        public void Constructor_InvalidIds_Throws()
        {
            Action a = () => new Session(0,1,1,DateTime.UtcNow);
            a.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Reschedule_DefaultDate_Throws()
        {
            var s = new Session(1,1,1,DateTime.UtcNow);
            Action a = () => s.Reschedule(default);
            a.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateNotes_TrimsOrNull()
        {
            var s = new Session(1,1,1,DateTime.UtcNow);
            s.UpdateNotes("  hello  ");
            s.Notes.Should().Be("hello");
            s.UpdateNotes(null);
            s.Notes.Should().BeNull();
        }
    }
}
