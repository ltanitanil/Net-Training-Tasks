using System;
using System.Linq;
using Reflection.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Reflection.Tests
{
    [TestClass]
    public class CommonTests
    {
        #region GetPublicObsoleteClasses tests
        [TestMethod]
        [TestCategory("GetPublicObsoleteClasses method")]
        public void GetPublicObsoleteClasses_Should_Return_Right_List()
        {
            var expected = "CaseInsensitiveHashCodeProvider, ContractHelper, ExecutionEngineException, "+
                           "FirstMatchCodeGroup, IDispatchImplAttribute, PermissionRequestEvidence, "+
                           "SecurityTreatAsSafeAttribute, SetWin32ContextInIDispatchAttribute, "+
                           "UnionCodeGroup, UnmanagedMarshal";

            var obsoleteMembers = CommonTasks.GetPublicObsoleteClasses("mscorlib, Version=4.0.0.0").OrderBy(x=>x);
            var actual = string.Join(", ", obsoleteMembers);
            Assert.AreEqual(expected, actual);
        }
        #endregion GetPublicObsoleteClasses tests

        #region Test Classes Declaration
        class Employee {
            public string FirstName { get; set; }
            public string LastName  { get; private set; }

            public Employee(string firstName, string lastName)  {
                this.FirstName = firstName;
                this.LastName = lastName;
            }
        }

        class Manager : Employee {
            public Manager(string firstName, string lastName) :  base(firstName, lastName) {}
        }

        class Worker : Employee {
            public Manager Manager { get; private set; }
            public Worker(string firstName, string lastName, Manager manager) : base(firstName, lastName) {
                this.Manager = manager;
            }
        }
        #endregion Test Classes Declaration

        #region GetProperty tests
        [TestMethod]
        [TestCategory("GetProperty method")]
        public void GetProperty_Should_Return_Property_Value_For_Complex_Path()
        {
            var manager = new Manager("Joe", "Smith");
            var worker = new Worker("Willy", "Brown", manager);
            var expected = worker.Manager.FirstName;
            var actual = worker.GetPropertyValue<string>("Manager.FirstName");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("GetProperty method")]
        public void GetProperty_Should_Return_Property_Value_For_Single_Path()
        {
            var manager = new Manager("Joe", "Smith");
            var expected = manager.FirstName;
            var actual = manager.GetPropertyValue<string>("FirstName");
            Assert.AreEqual(expected, actual);
        }

        
        #endregion GetProperty tests

        #region SetProperty tests
        [TestMethod]
        [TestCategory("SetProperty method")]
        public void SetProperty_Should_Assign_Value_For_Single_Public_Path()
        {
            var manager = new Manager("Joe", "Smith");
            var expected = "Alex";

            manager.SetPropertyValue("FirstName", expected);
            
            var actual = manager.FirstName;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("SetProperty method")]
        public void GetProperty_Should_Assign_Value_For_Complex_Path()
        {
            var manager = new Manager("Joe", "Smith");
            var worker = new Worker("Willy", "Brown", manager);
            var expected = "Alex";

            worker.SetPropertyValue("Manager.FirstName", expected);

            var actual = worker.Manager.FirstName;
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [TestCategory("SetProperty method")]
        public void SetProperty_Should_Assign_Value_For_Single_Private_Path()
        {
            var manager = new Manager("Joe", "Smith");
            var expected = "Johnson";

            manager.SetPropertyValue("LastName", expected);

            var actual = manager.LastName;
            Assert.AreEqual(expected, actual);
        }
        #endregion SetProperty tests
    }
}
