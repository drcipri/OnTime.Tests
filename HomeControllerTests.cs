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
        public void Index_GetAppointemtnsFromDataBase_ViewContainDatabaseObjects()
        {
            //arrange
            var mockRepository = new Mock<IRepositoryAppointment>();
            mockRepository.Setup(c => c.Appointments).Returns((new Appointment[]
            {
                new Appointment
                {
                    Id = 1,
                    DateTime = DateTime.Now,
                    Objective = "O1",
                    Reason = "R1",
                    AdditionalInfo = "AF1",
                    Classification = new Classification { Id = 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 2,
                    DateTime = DateTime.Now,
                    Objective = "O2",
                    Reason = "R2",
                    AdditionalInfo = "AF2",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                }
            }).AsQueryable());

            var controller = new HomeController(mockRepository.Object);

            //act
            var result = (controller.Index() as ViewResult)?.ViewData.Model as AppointmetnsListViewModel ?? new();

            //assert
            Appointment[] appointments = result.Appointments.ToArray();
            Assert.Multiple(() =>
            {
                Assert.That(appointments, Has.Length.EqualTo(2));
                Assert.That(appointments[0].Objective, Is.EqualTo("O1"));
                Assert.That(appointments[0].Id, Is.EqualTo(1));
                Assert.That(appointments[1].Objective, Is.EqualTo("O2"));
                Assert.That(appointments[1].Id, Is.EqualTo(2));
            });
        }
        [Test]
        public void Index_CanPaginate_ReturnSecondPage()
        {
            //arrange
            var mockRepository = new Mock<IRepositoryAppointment>();
            mockRepository.Setup(c => c.Appointments).Returns((new Appointment[]
            {
                new Appointment
                {
                    Id = 1,
                    DateTime = DateTime.Now,
                    Objective = "O1",
                    Reason = "R1",
                    AdditionalInfo = "AF1",
                    Classification = new Classification { Id = 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 2,
                    DateTime = DateTime.Now,
                    Objective = "O2",
                    Reason = "R2",
                    AdditionalInfo = "AF2",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 3,
                    DateTime = DateTime.Now,
                    Objective = "O3",
                    Reason = "R3",
                    AdditionalInfo = "AF3",
                    Classification = new Classification { Id = 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 4,
                    DateTime = DateTime.Now,
                    Objective = "O4",
                    Reason = "R4",
                    AdditionalInfo = "AF4",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 5,
                    DateTime = DateTime.Now,
                    Objective = "O4",
                    Reason = "R4",
                    AdditionalInfo = "AF4",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                }
            }).AsQueryable());

            var controller = new HomeController(mockRepository.Object);
            controller.PageSize = 2;

            //act
            var result = (controller.Index(2) as ViewResult)?.ViewData.Model as AppointmetnsListViewModel ?? new();

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
            mockRepository.Setup(c => c.Appointments).Returns((new Appointment[]
            {
                new Appointment
                {
                    Id = 1,
                    DateTime = DateTime.Now,
                    Objective = "O1",
                    Reason = "R1",
                    AdditionalInfo = "AF1",
                    Classification = new Classification { Id = 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 2,
                    DateTime = DateTime.Now,
                    Objective = "O2",
                    Reason = "R2",
                    AdditionalInfo = "AF2",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 3,
                    DateTime = DateTime.Now,
                    Objective = "O3",
                    Reason = "R3",
                    AdditionalInfo = "AF3",
                    Classification = new Classification { Id = 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 4,
                    DateTime = DateTime.Now,
                    Objective = "O4",
                    Reason = "R4",
                    AdditionalInfo = "AF4",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 5,
                    DateTime = DateTime.Now,
                    Objective = "O4",
                    Reason = "R4",
                    AdditionalInfo = "AF4",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                }
            }).AsQueryable());

            var controller = new HomeController(mockRepository.Object);
            controller.PageSize = 2;

            //act
            var result = (controller.Index(2) as ViewResult)?.ViewData.Model as AppointmetnsListViewModel ?? new();

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
    }
    
}
