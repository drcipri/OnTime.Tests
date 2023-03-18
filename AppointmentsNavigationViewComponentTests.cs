using Microsoft.AspNetCore.Mvc;
using Moq;
using OnTime.Controllers;
using OnTime.Models.Repository;
using OnTime.Models;
using OnTime.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnTime.Component;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace OnTime.Tests
{
    [TestFixture]
    internal class AppointmentsNavigationViewComponentTests
    {
        [Test]
        public void Invoke_CanGetClassificationNames_ViewHasTheClassifications()
        {
            //arrange
            var mockRepository = new Mock<IRepositoryClassification>();
            mockRepository.Setup(c => c.GetAll()).Returns((new List<Classification>
            {
                new Classification
                {
                   Id = 1,
                   Name = "Awaiting"
                },
                new Classification
                {
                    Id = 2,
                    Name = "Succesfull"
                }
            }).AsEnumerable());

            var vc = new AppointmentsNavigationViewComponent(mockRepository.Object);

            //act
            List<string> result = ((IEnumerable<string>?)(vc.Invoke() as ViewViewComponentResult)?.ViewData?.Model ?? Enumerable.Empty<string>()).ToList();

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(2));
                Assert.That(result[0], Is.EqualTo("Awaiting"));
                Assert.That(result[1], Is.EqualTo("Succesfull"));
            });
        }
    }
    
}
