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
        public async IAsyncEnumerable<Appointment> MockDataAsyncAppointments()
        {
            var mockData = new List<Appointment>
            {
                new Appointment
                {
                    Id = 1,
                    AppointmentDateTime = DateTime.Now,
                    PostDateTime= DateTime.Now,
                    Objective = "O1",
                    Reason = "R1",
                    AdditionalInfo = "AF1",
                    Classification = new Classification { Id = 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 2,
                    AppointmentDateTime = DateTime.Now.AddDays(1),
                    PostDateTime = DateTime.Now.AddDays(1),
                    Objective = "O2",
                    Reason = "R2",
                    AdditionalInfo = "AF2",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 3,
                    AppointmentDateTime = DateTime.Now.AddDays(2),
                    PostDateTime = DateTime.Now.AddDays(2),
                    Objective = "O3",
                    Reason = "R3",
                    AdditionalInfo = "AF3",
                    Classification = new Classification { Id = 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 4,
                    AppointmentDateTime = DateTime.Now.AddDays(3),
                    PostDateTime= DateTime.Now.AddDays(3),
                    Objective = "O4",
                    Reason = "R4",
                    AdditionalInfo = "AF4",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                },
                new Appointment
                {
                    Id = 5,
                    AppointmentDateTime = DateTime.Now.AddDays(4),
                    PostDateTime = DateTime.Now.AddDays(4),
                    Objective = "O5",
                    Reason = "R5",
                    AdditionalInfo = "AF5",
                    Classification = new Classification { Id= 1, Name = "Awaiting"}
                }
            };

            foreach (var appointment in mockData)
            {
                yield return appointment;
            }
            await Task.CompletedTask;
        }
        [Test]
        public async Task Index_GetAppointmentsFromDataBase_ViewContainDatabaseObjectsInDescendingOrder()
        {
            //arrange
            var mockRepository = new Mock<IRepositoryAppointment>();
            mockRepository.Setup(c => c.FilterAppointmentsAsync(It.IsAny<string>())).Returns(MockDataAsyncAppointments());
            

            var controller = new HomeController(mockRepository.Object);
            controller.PageSize = 2;

            //act
            var result = (await controller.Index(1,ClassificationTypes.Awaiting) as ViewResult)?.ViewData.Model as AppointmentsListViewModel ?? new();

            //assert
            List<Appointment> appointments = result.Appointments.ToList();
            Assert.Multiple(() =>
            {
                Assert.That(appointments, Has.Count.EqualTo(2));
                Assert.That(appointments[0].Objective, Is.EqualTo("O5"));
                Assert.That(appointments[0].Id, Is.EqualTo(5));
                Assert.That(appointments[1].Objective, Is.EqualTo("O4"));
                Assert.That(appointments[1].Id, Is.EqualTo(4));
            });
        }
        [Test]
        public async Task Index_CanPaginate_ReturnSecondPage()
        {
            //arrange
            var mockRepository = new Mock<IRepositoryAppointment>();
            mockRepository.Setup(c => c.FilterAppointmentsAsync(It.IsAny<string>())).Returns(MockDataAsyncAppointments());

            var controller = new HomeController(mockRepository.Object);
            controller.PageSize = 2;

            //act
            var result = (await controller.Index(2,ClassificationTypes.Awaiting) as ViewResult)?.ViewData.Model as AppointmentsListViewModel ?? new();

            //assert
            var appointments = result.Appointments.ToArray();
            Assert.Multiple(() =>
            {
                Assert.That(appointments, Has.Length.EqualTo(2));
                Assert.That(appointments[0].Id, Is.EqualTo(3));
                Assert.That(appointments[0].Objective, Is.EqualTo("O3"));
                Assert.That(appointments[1].Id , Is.EqualTo(2));
                Assert.That(appointments[1].Objective, Is.EqualTo("O2"));
            });
        }
        [Test]
        public async Task Index_PaginationInfoObjectHasAccurateData_PaginationInfoIsAccurate()
        {
            //arrange
            var mockRepository = new Mock<IRepositoryAppointment>();
            mockRepository.Setup(c => c.FilterAppointmentsAsync(ClassificationTypes.Awaiting)).Returns(MockDataAsyncAppointments());
            

            var controller = new HomeController(mockRepository.Object);
            controller.PageSize = 2;

            //act
            var result = (await controller.Index(2,ClassificationTypes.Awaiting) as ViewResult)?.ViewData.Model as AppointmentsListViewModel ?? new();

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
        public async Task Index_CanFilterByClassification_ReturnSuccesfullAppointments()
        {
            //arrange
            var mockRepository = new Mock<IRepositoryAppointment>();
            mockRepository.Setup(c => c.FilterAppointmentsAsync(ClassificationTypes.Succesfull)).Returns(MockDataAsyncAppointments());
           

            var controller = new HomeController(mockRepository.Object);
            controller.PageSize = 2;

            //act
            var result = (await controller.Index(1, ClassificationTypes.Succesfull) as ViewResult)?.ViewData.Model as AppointmentsListViewModel ?? new();

            //assert
            Assert.That(result.Classification, Is.EqualTo("Succesfull"));
        }


        //SearchIndex Tests
        [TestCase(null,"")]
        [TestCase("", null)]
        [TestCase(null, null)]
        [TestCase("","")]
        public async Task SearchIndex_ArgumentsAreNull_ReturnANotherRoute(string testCaseOne,string testCaseTwo)
        {
            //arrange
            var mockRepository = new Mock<IRepositoryAppointment>();
            mockRepository.Setup(c => c.SearchAppointmentsAsync(It.IsAny<AppointmentsSearchCriteria>(),It.IsAny<string>())).Returns(MockDataAsyncAppointments());
            var controller = new HomeController(mockRepository.Object);
            controller.PageSize = 2;

            //act
            var result = await controller.SearchIndex(testCaseOne, testCaseTwo, 1) as RedirectToRouteResult;
           
            //Assert
            mockRepository.Verify(x => x.SearchAppointmentsAsync(It.IsAny<AppointmentsSearchCriteria>(), It.IsAny<string>()), Times.Never);
           
            Assert.That(result?.RouteValues?["controller"], Is.EqualTo("Home"));
            Assert.That(result?.RouteValues?["action"], Is.EqualTo("Index"));
        }

        [Test]
        public async Task SearchIndex_CanSearchThoruAppointments_ViewGetTheFoundAppointments()
        {
            //assert
            var mockRepository = new Mock<IRepositoryAppointment>();
            mockRepository.Setup(c => c.SearchAppointmentsAsync(It.IsAny<AppointmentsSearchCriteria>(), It.IsAny<string>())).Returns(MockDataAsyncAppointments());
            var controller = new HomeController(mockRepository.Object);
            controller.PageSize = 2;

            //act
            var result = ((await controller.SearchIndex("Awaiting", "Anything", 1)) as ViewResult)?.ViewData.Model as AppointmentsListViewModel ?? new();

            //assert
            var appointments = result.Appointments.ToList();

            Assert.Multiple(() =>
            {
                Assert.That(appointments, Has.Count.EqualTo(2));
                Assert.That(appointments[0].Objective, Is.EqualTo("O5"));
                Assert.That(appointments[0].Id, Is.EqualTo(5));
                Assert.That(appointments[1].Objective, Is.EqualTo("O4"));
                Assert.That(appointments[1].Id, Is.EqualTo(4));
            });
        }

        [Test]
        public async Task SearchIndex_CanPaginate_ViewGetTheSecondPage()
        {
            //assert
            var mockRepository = new Mock<IRepositoryAppointment>();
            mockRepository.Setup(c => c.SearchAppointmentsAsync(It.IsAny<AppointmentsSearchCriteria>(), It.IsAny<string>())).Returns(MockDataAsyncAppointments());
            var controller = new HomeController(mockRepository.Object);
            controller.PageSize = 2;

            //act
            var result = ((await controller.SearchIndex("Awaiting", "Anything", 2)) as ViewResult)?.ViewData.Model as AppointmentsListViewModel ?? new();

            //assert
            var appointments = result.Appointments.ToList();

            Assert.Multiple(() =>
            {
                Assert.That(appointments, Has.Count.EqualTo(2));
                Assert.That(appointments[0].Id, Is.EqualTo(3));
                Assert.That(appointments[0].Objective, Is.EqualTo("O3"));
                Assert.That(appointments[1].Id, Is.EqualTo(2));
                Assert.That(appointments[1].Objective, Is.EqualTo("O2"));
            });
        }

        [Test]
        public async Task SearchIndex_PaginationInfoHasAccurateData()
        {
            //assert
            var mockRepository = new Mock<IRepositoryAppointment>();
            mockRepository.Setup(c => c.SearchAppointmentsAsync(It.IsAny<AppointmentsSearchCriteria>(), It.IsAny<string>())).Returns(MockDataAsyncAppointments());
            var controller = new HomeController(mockRepository.Object);
            controller.PageSize = 2;

            //act
            var result = ((await controller.SearchIndex("Awaiting", "Anything", 2)) as ViewResult)?.ViewData.Model as AppointmentsListViewModel ?? new();

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
        public async Task SearchIndex_SearchCriteriaSearchRequestClassificationAreAccurate()
        {
            //assert
            var mockRepository = new Mock<IRepositoryAppointment>();
            mockRepository.Setup(c => c.SearchAppointmentsAsync(It.IsAny<AppointmentsSearchCriteria>(), It.IsAny<string>())).Returns(MockDataAsyncAppointments());
            var controller = new HomeController(mockRepository.Object);
            controller.PageSize = 2;

            //act
            var result = ((await controller.SearchIndex("Awaiting", "Anything", 2)) as ViewResult)?.ViewData.Model as AppointmentsListViewModel ?? new();

            //assert

            Assert.Multiple(() =>
            {
                Assert.That(result.SearchRequest, Is.EqualTo(true));
                Assert.That(result.SearchCriteria, Is.EqualTo("Anything"));
                Assert.That(result.Classification, Is.EqualTo("Awaiting"));
            });
        }
    }
    
}
