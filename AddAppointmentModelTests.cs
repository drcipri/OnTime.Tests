using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
    public class AddAppointmentModelTests
    {
        [Test]
        public void OnPost_AppointmentIsNotValid_ReturnThePage()
        {
            //arrange
            var mockRep = new Mock<IRepositoryAppointment>();
            var appointment = new Appointment
            {
                //objective is not set
                AppointmentDateTime = DateTime.Now,
                ClassificationId = 1
            };
            AddAppointmentModel appModel = new AddAppointmentModel(mockRep.Object)
            {
                CurrentAppointment = appointment
            };
            appModel.ModelState.AddModelError("error", "customError");

            //act
            var result = appModel.OnPost() as PageResult;

            //assert
            mockRep.Verify(x => x.AddAppointment(It.IsAny<Appointment>()), Times.Never);
        }

        [Test]
        public void OnPost_AppointmentIsValid_ReturnHome()
        {
            //arrange
            var mockRep = new Mock<IRepositoryAppointment>();
            var appointment = new Appointment
            {
                Objective = "Whatever",
                AppointmentDateTime = DateTime.Now,
                ClassificationId = 1
            };
            AddAppointmentModel appModel = new AddAppointmentModel(mockRep.Object)
            {
                CurrentAppointment = appointment
            };

            //act
            var result = appModel.OnPost() as RedirectToRouteResult;

            //assert
            mockRep.Verify(x => x.AddAppointment(It.IsAny<Appointment>()), Times.Once);
            Assert.That(result?.RouteValues?["controller"], Is.EqualTo("Home"));
            Assert.That(result?.RouteValues?["action"], Is.EqualTo("Index"));
            Assert.That(result?.RouteValues?["classification"], Is.EqualTo("Awaiting"));
        }

        [Test]
        public void OnPostRemove_DeleteAppointment_ReturnHome()
        {
            //arrange
            var mockRep = new Mock<IRepositoryAppointment>();
            var appointment = new Appointment
            {
                Objective = "Whatever",
                AppointmentDateTime = DateTime.Now,
                ClassificationId = 1
            };
            AddAppointmentModel appModel = new AddAppointmentModel(mockRep.Object);

            //act
            var result = appModel.OnPostRemove(1,"ClassificationTest") as RedirectToRouteResult;

            //assert
            mockRep.Verify(x => x.RemoveById(It.IsAny<int>()), Times.Once);
            Assert.That(result?.RouteValues?["controller"], Is.EqualTo("Home"));
            Assert.That(result?.RouteValues?["action"], Is.EqualTo("Index"));
            Assert.That(result?.RouteValues?["classification"], Is.EqualTo("ClassificationTest"));
        }
    }
}
