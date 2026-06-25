using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    internal class LapTests: BaseEntity
    {
       
        public string TestName { get; private set; }

        public string Description { get; private set; }

        private LabTest()
        {

        }

        public LabTest(string testName, string description)
        {
            TestName = testName;
            Description = description;
        }

        public void Update(string testName, string description)
        {
            TestName = testName;
            Description = description;
        }
    }
}
