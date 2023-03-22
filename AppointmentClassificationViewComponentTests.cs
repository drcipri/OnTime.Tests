using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using NUnit.Framework.Constraints;
using OnTime.Component;
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
    internal class AppointmentClassificationViewComponentTests
    {
        [Test]  
        public void Invoke_GetClassificationNamesAndPassThemToTheView_PartialViewGetClassifictionNames()
        {
            //Arrange
            var mockRepository = new Mock<IRepositoryClassification>();
            mockRepository.Setup(c => c.GetAll()).Returns(new List<Classification>
            {
                new Classification
                {
                    Id = 1,
                    Name = ClassificationTypes.Awaiting
                },
                new Classification
                {
                    Id = 2,
                    Name = ClassificationTypes.Succesfull
                }
            }.AsEnumerable());
            var vc = new AppointmentClassificationViewComponent(mockRepository.Object);

            //act
            var result = (vc.Invoke(1, ClassificationTypes.Awaiting) as ViewViewComponentResult)?.ViewData?.Model as MarkAppointmentClassification ?? new();
            List<string> names = result.Classifications.ToList();

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(names[0], Is.EqualTo("Awaiting"));
                Assert.That(names[1], Is.EqualTo("Succesfull"));
            });
        }
        [Test]
        public void Invoke_CanGetCurrentAppointmentIdAndCurrentClassification_PartialViewGetThisData()
        {
            //Arrange
            var mockRepository = new Mock<IRepositoryClassification>();
            mockRepository.Setup(c => c.GetAll()).Returns(new List<Classification>
            {
                new Classification
                {
                    Id = 1,
                    Name = ClassificationTypes.Awaiting
                },
                new Classification
                {
                    Id = 2,
                    Name = ClassificationTypes.Succesfull
                }
            }.AsEnumerable());
            var vc = new AppointmentClassificationViewComponent(mockRepository.Object);

            //act
            var result = (vc.Invoke(4, ClassificationTypes.Awaiting) as ViewViewComponentResult)?.ViewData?.Model as MarkAppointmentClassification ?? new();

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result.AppointmentId, Is.EqualTo(4));
                Assert.That(result.CurrentClassification, Is.EqualTo("Awaiting"));
            });
        }
    }
}
