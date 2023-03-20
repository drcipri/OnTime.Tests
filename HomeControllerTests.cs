using Microsoft.AspNetCore.Mvc;
using Moq;
using OnTime.Controllers;
using OnTime.Models;
using OnTime.Models.Repository;
using OnTime.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnTime.Tests
{
    [TestFixture]
    internal class HomeControllerTests
    {
        [Test]
        public void Index_GetAppointemtnsFromDataBase_ViewContainDatabaseObjectsInDescendingOrder()
        {
            //arrange
            var mockRepository = new Mock<IRepositoryAppointment>();
            mockRepository.Setup(c => c.FilterAppointments(ClassificationTypes.Awaiting)).Returns((new Appointment[]
            {
                new Appointment
                {
                    Id = 1,
                    AppointmentDateTime = DateTime.Now,
                    PostDateTime = new DateTime(2020,1,1),
                    Objective = "O1",
                    Reason = "R1",
                    AdditionalInfo = "AF1",
                    Classification = new Classification { Id = 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 2,
                    AppointmentDateTime = DateTime.Now,
                    PostDateTime = new DateTime(2020,2,1),
                    Objective = "O2",
                    Reason = "R2",
                    AdditionalInfo = "AF2",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                }
            }).AsEnumerable());

            var controller = new HomeController(mockRepository.Object);

            //act
            var result = (controller.Index(1,ClassificationTypes.Awaiting) as ViewResult)?.ViewData.Model as AppointmentsListViewModel ?? new();

            //assert
            Appointment[] appointments = result.Appointments.ToArray();
            Assert.Multiple(() =>
            {
                Assert.That(appointments, Has.Length.EqualTo(2));
                Assert.That(appointments[0].Objective, Is.EqualTo("O2"));
                Assert.That(appointments[0].Id, Is.EqualTo(2));
                Assert.That(appointments[1].Objective, Is.EqualTo("O1"));
                Assert.That(appointments[1].Id, Is.EqualTo(1));
            });
        }
        [Test]
        public void Index_CanPaginate_ReturnSecondPage()
        {
            //arrange
            var mockRepository = new Mock<IRepositoryAppointment>();
            mockRepository.Setup(c => c.FilterAppointments(ClassificationTypes.Awaiting)).Returns((new Appointment[]
            {
                new Appointment
                {
                    Id = 1,
                    AppointmentDateTime = DateTime.Now,
                    Objective = "O1",
                    Reason = "R1",
                    AdditionalInfo = "AF1",
                    Classification = new Classification { Id = 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 2,
                    AppointmentDateTime = DateTime.Now,
                    Objective = "O2",
                    Reason = "R2",
                    AdditionalInfo = "AF2",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 3,
                    AppointmentDateTime = DateTime.Now,
                    Objective = "O3",
                    Reason = "R3",
                    AdditionalInfo = "AF3",
                    Classification = new Classification { Id = 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 4,
                    AppointmentDateTime = DateTime.Now,
                    Objective = "O4",
                    Reason = "R4",
                    AdditionalInfo = "AF4",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 5,
                    AppointmentDateTime = DateTime.Now,
                    Objective = "O4",
                    Reason = "R4",
                    AdditionalInfo = "AF4",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                }
            }).AsEnumerable());

            var controller = new HomeController(mockRepository.Object);
            controller.PageSize = 2;

            //act
            var result = (controller.Index(2,ClassificationTypes.Awaiting) as ViewResult)?.ViewData.Model as AppointmentsListViewModel ?? new();

            //assert
            var appointments = result.Appointments.ToArray();
            Assert.Multiple(() =>
            {
                Assert.That(appointments, Has.Length.EqualTo(2));
                Assert.That(appointments[0].Id, Is.EqualTo(3));
                Assert.That(appointments[1].Id , Is.EqualTo(4));
                Assert.That(appointments[0].Objective, Is.EqualTo("O3"));
                Assert.That(appointments[1].Objective, Is.EqualTo("O4"));
            });
        }
        [Test]
        public void Index_PaginationInfoObjectHasAccurateData_PaginationInfoIsAccurate()
        {
            //arrange
            var mockRepository = new Mock<IRepositoryAppointment>();
            mockRepository.Setup(c => c.FilterAppointments(ClassificationTypes.Awaiting)).Returns((new Appointment[]
            {
                new Appointment
                {
                    Id = 1,
                    AppointmentDateTime = DateTime.Now,
                    Objective = "O1",
                    Reason = "R1",
                    AdditionalInfo = "AF1",
                    Classification = new Classification { Id = 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 2,
                    AppointmentDateTime = DateTime.Now,
                    Objective = "O2",
                    Reason = "R2",
                    AdditionalInfo = "AF2",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 3,
                    AppointmentDateTime = DateTime.Now,
                    Objective = "O3",
                    Reason = "R3",
                    AdditionalInfo = "AF3",
                    Classification = new Classification { Id = 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 4,
                    AppointmentDateTime = DateTime.Now,
                    Objective = "O4",
                    Reason = "R4",
                    AdditionalInfo = "AF4",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 5,
                    AppointmentDateTime = DateTime.Now,
                    Objective = "O4",
                    Reason = "R4",
                    AdditionalInfo = "AF4",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                }
            }).AsEnumerable());

            var controller = new HomeController(mockRepository.Object);
            controller.PageSize = 2;

            //act
            var result = (controller.Index(2,ClassificationTypes.Awaiting) as ViewResult)?.ViewData.Model as AppointmentsListViewModel ?? new();

            //assert
            var pagingInfo = result.PaginationInfo;
            Assert.Multiple(() =>
            {
                Assert.That(pagingInfo.ItemsPerPage, Is.EqualTo(2));
                Assert.That(pagingInfo.CurrentPage, Is.EqualTo(2));
                Assert.That(pagingInfo.TotalItems, Is.EqualTo(5));
                Assert.That(pagingInfo.TotalPages, Is.EqualTo(3));
            });
        }
        [Test]
        public void Index_CanFilterByClassification_ReturnSuccesfullAppointments()
        {
            //arrange
            var mockRepository = new Mock<IRepositoryAppointment>();
            mockRepository.Setup(c => c.FilterAppointments(ClassificationTypes.Succesfull)).Returns((new Appointment[]
            {
                new Appointment
                {
                    Id = 1,
                    AppointmentDateTime = DateTime.Now,
                    Objective = "O1",
                    Reason = "R1",
                    AdditionalInfo = "AF1",
                    Classification = new Classification { Id = 1, Name = "Succesfull"}
                },
                new Appointment
                {
                    Id = 2,
                    AppointmentDateTime = DateTime.Now,
                    Objective = "O2",
                    Reason = "R2",
                    AdditionalInfo = "AF2",
                    Classification = new Classification { Id= 2, Name = "Succesfull"}
                },
                new Appointment
                {
                    Id = 3,
                    AppointmentDateTime = DateTime.Now,
                    Objective = "O3",
                    Reason = "R3",
                    AdditionalInfo = "AF3",
                    Classification = new Classification { Id = 2, Name = "Succesfull"}
                },
            }).AsEnumerable());

            var controller = new HomeController(mockRepository.Object);
            controller.PageSize = 2;

            //act
            var result = (controller.Index(1, ClassificationTypes.Succesfull) as ViewResult)?.ViewData.Model as AppointmentsListViewModel ?? new();

            //assert
            //since i implemented a method for Filtering appointments i can only test that method with integration tests.
            //i simply checked if the Classification is the same as the one passed through the Index.
            Assert.That(result.Classification, Is.EqualTo("Succesfull"));
        }
    }
    
}
