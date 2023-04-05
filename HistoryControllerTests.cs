using Microsoft.AspNetCore.Mvc;
using Moq;
using OnTime.Controllers;
using OnTime.Models;
using OnTime.Models.Repository;
using OnTime.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OnTime.Tests
{
    internal class HistoryControllerTests
    {
        public async IAsyncEnumerable<AppointmentAudit> MockIAsyncEnumerableAppoinmentsAudits()
        {
            var mockData = new List<AppointmentAudit>
            {
                new AppointmentAudit
                {
                    Id = 1,
                    ActionDate= DateTime.Now,
                    PostDateTime= DateTime.Now,
                    ActionType = "EDIT",
                    Objective = "O1",
                    Reason = "R1",
                    AdditionalInfo = "AI1",
                    Classification = "Succesfull"
                },
                new AppointmentAudit
                {
                    Id = 2,
                    ActionDate= DateTime.Now.AddDays(1),
                    PostDateTime= DateTime.Now,
                    ActionType = "UPDATE",
                    Objective = "O2",
                    Reason = "R2",
                    AdditionalInfo = "AI2",
                    Classification = "Awaiting"
                },
                new AppointmentAudit
                {
                    Id = 3,
                    ActionDate= DateTime.Now.AddDays(2),
                    PostDateTime= DateTime.Now,
                    ActionType = "DELETE",
                    Objective = "O3",
                    Reason = "R3",
                    AdditionalInfo = "AI3",
                    Classification = "MISSED"
                }
            };
            foreach(var data in mockData)
            {
                yield return data;
            }
            await Task.CompletedTask;
        }
        [Test]
        public async Task AppointmentsHystory_CanLoadAllCRUDHistoryRecords_ViewGetLoadedHistory()
        {
            //arrange
            var mockRepository = new Mock<IRepositoryAppointmentsAudit>();
            mockRepository.Setup(x => x.GetAllAsync()).Returns(MockIAsyncEnumerableAppoinmentsAudits());
            
            //act
            var controller = new HistoryController(mockRepository.Object);
            var result = (await controller.AppointmentsHistory() as ViewResult)?.ViewData.Model as HistoryListViewModel ?? new ();

            //assert
            //the records are in descending Order because of the controller action
            var records = result.AppointmentAudits.ToList();
            Assert.Multiple(() =>
            {
                Assert.That(records, Has.Count.EqualTo(3));
                Assert.That(records[0].Id , Is.EqualTo(3));
                Assert.That(records[0].ActionType, Is.EqualTo("DELETE"));
                Assert.That(records[2].Id, Is.EqualTo(1));
                Assert.That(records[2].ActionType, Is.EqualTo("EDIT"));
                Assert.That(result.SearchRequest, Is.EqualTo(false));
                Assert.That(result.SearchCriteria, Is.Empty);
            });

        }

        [Test]
        public async Task SearchHistory_CanSearchByCriteria_ReturnsViewWithObjectsBySearchCriteria()
        {
            //arrange
            var mockRepository = new Mock<IRepositoryAppointmentsAudit>();
            mockRepository.Setup(x => x.SearchAsync(It.IsAny<AppointmentsAuditSearchCriteria>())).Returns(MockIAsyncEnumerableAppoinmentsAudits());

            //act
            var controller = new HistoryController(mockRepository.Object);
            var result = (await controller.SearchHistory("AnyString") as ViewResult)?.ViewData.Model as HistoryListViewModel ?? new();

            //assert
            var records = result.AppointmentAudits.ToList();
            Assert.Multiple(() =>
            {

                Assert.That(records, Has.Count.EqualTo(3));
                Assert.That(records[0].Id, Is.EqualTo(3));
                Assert.That(records[0].ActionType, Is.EqualTo("DELETE"));
                Assert.That(records[2].Id, Is.EqualTo(1));
                Assert.That(records[2].ActionType, Is.EqualTo("EDIT"));
                Assert.That(result.SearchRequest, Is.EqualTo(true));
                Assert.That(result.SearchCriteria, Is.EqualTo("AnyString"));
            });
        }
        [Test]
        [TestCase(null)]
        [TestCase("")]
        public async Task SearchHistory_SearchCriteriaIsNull_ReturnsHistoryView(string testCase)
        {
            //arrange
            var mockRepository = new Mock<IRepositoryAppointmentsAudit>();
            mockRepository.Setup(x => x.SearchAsync(It.IsAny<AppointmentsAuditSearchCriteria>())).Returns(MockIAsyncEnumerableAppoinmentsAudits());

            //act
            var controller = new HistoryController(mockRepository.Object);
            var result = await controller.SearchHistory(testCase) as RedirectToRouteResult;

            //assert
            mockRepository.Verify(x => x.SearchAsync(It.IsAny<AppointmentsAuditSearchCriteria>()), Times.Never());
            Assert.That(result?.RouteName, Is.EqualTo("History"));
        }
    }
}
