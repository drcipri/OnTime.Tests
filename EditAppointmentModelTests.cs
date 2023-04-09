using Microsoft.AspNetCore.Mvc;
using Moq;
using OnTime.Models;
using OnTime.Models.Repository;
using OnTime.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OnTime.Tests
{
    [TestFixture]
    public class EditAppointmentModelTests
    {
        [Test]
        public void OnGet_AppointmentIsNull_EditAppointmentPropertyIsNotSet()
        {
            //arrange
            var mockRep = new Mock<IRepositoryAppointment>();
            mockRep.Setup(x => x.GetAppointment(It.IsAny<int>())).Returns(default(Appointment));

            EditAppointmentModel editModel = new EditAppointmentModel(mockRep.Object);

            //act
            editModel.OnGet(1);

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(editModel.EditAppointment.Id, Is.EqualTo(0));
                Assert.That(editModel.EditAppointment.Objective, Is.Empty);
                Assert.That(editModel.EditAppointment.Reason, Is.Null);
                Assert.That(editModel.EditAppointment.ClassificationId, Is.EqualTo(0));
            });
        }
       
        [Test]
        public void OnGet_AppointmentIsNotNull_EditAppointmentPropertyIsSet()
        {
            //arrange
            var mockRep = new Mock<IRepositoryAppointment>();
            mockRep.Setup(x => x.GetAppointment(It.IsAny<int>())).Returns(new Appointment
            {
                Id = 1,
                Objective = "O1",
                Reason = "R1",
                PostDateTime = DateTime.Now,
                ClassificationId = 1,
            });

            EditAppointmentModel editModel = new EditAppointmentModel(mockRep.Object);

            //act
            editModel.OnGet(1);

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(editModel.EditAppointment.Id, Is.EqualTo(1));
                Assert.That(editModel.EditAppointment.Objective, Is.EqualTo("O1"));
                Assert.That(editModel.EditAppointment.Reason, Is.EqualTo("R1"));
                Assert.That(editModel.EditAppointment.ClassificationId, Is.EqualTo(1));
            });

        }

        [Test]
        public void OnPost_ModelStateIsNotValid_ReturnPage()
        {
            //Arrange
            var mockRep = new Mock<IRepositoryAppointment>();
            EditAppointmentModel editModel = new EditAppointmentModel(mockRep.Object);
            editModel.ModelState.AddModelError("error", "customError");

            //act
            editModel.OnPost(1);

            //assert
            mockRep.Verify(x => x.UpdateAppointment(It.IsAny<Appointment>()), Times.Never);
        }

        [Test]
        public void OnPost_ModelStateIsValid_ReturnHome()
        {
            //Arrange
            var mockRep = new Mock<IRepositoryAppointment>();
            EditAppointmentModel editModel = new EditAppointmentModel(mockRep.Object);
            editModel.EditAppointment = new Appointment
            {
                Objective = "O2",
                Reason = "R2",
                PostDateTime = DateTime.Now,   
                Classification = new Classification { Id = 2, Name = "Test2"},
                ClassificationId = 2
            };
            //act
            var result =  editModel.OnPost(2) as RedirectToRouteResult;

            //assert
            mockRep.Verify(x => x.UpdateAppointment(It.IsAny<Appointment>()), Times.Once);
            Assert.Multiple(() =>
            {
                Assert.That(editModel.EditAppointment.Id, Is.EqualTo(2));
                Assert.That(editModel.EditAppointment.Objective, Is.EqualTo("O2"));
                Assert.That(editModel.EditAppointment.Reason, Is.EqualTo("R2"));
                Assert.That(editModel.EditAppointment.ClassificationId, Is.EqualTo(2));
                Assert.That(result?.RouteValues?["classification"], Is.EqualTo("Test2"));
                Assert.That(result?.RouteValues?["Controller"], Is.EqualTo("Home"));
                Assert.That(result?.RouteValues?["action"], Is.EqualTo("Index"));
            });
        }
    }
}
