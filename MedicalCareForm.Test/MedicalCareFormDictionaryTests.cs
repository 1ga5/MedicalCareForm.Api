using Xunit;
using MedicalCareForm.Api.Controllers;
using MedicalCareForm.Api.Repositories;
using MedicalCareForm.Share.DTOs;
using MedicalCareForm.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;


namespace MedicalCareForm.Test
{
    public class MedicalCareFormDictionaryTests
    {
        [Fact]
        public async Task GetAllDictionaries()
        {
            // Arrange
            var mock = new Mock<IRepository<MedicalCareFormDictionary>>();
            mock.Setup(repo => repo.GetAll())
                .Returns(GetTestDictionaries().AsQueryable);
            var controller = new MedicalCareFormDictionaryController(mock.Object);

            // Act
            var actionResult = await controller.GetAll();

            // Assert
            var viewResult = Assert.IsType<ActionResult<MedicalCareFormDictionary>>(actionResult);
        }

        [Fact]
        public async Task GetDictionaryByIdWhenDictionaryNotFound()
        {
            // Arrange
            int testDictionaryId = 5;
            var mock = new Mock<IRepository<MedicalCareFormDictionary>>();
            mock.Setup(repo => repo.GetByKeyAsync(testDictionaryId))
                .ReturnsAsync(null as MedicalCareFormDictionary);
            var controller = new MedicalCareFormDictionaryController(mock.Object);

            // Act
            var actionResult = await controller.Get(testDictionaryId);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetDictionaryById()
        {
            // Arrange
            int testDictionaryId = 1;
            var mock = new Mock<IRepository<MedicalCareFormDictionary>>();
            mock.Setup(repo => repo.GetByKeyAsync(testDictionaryId))
                .ReturnsAsync(GetTestDictionaries().FirstOrDefault(d => d.Id == testDictionaryId));
            var controller = new MedicalCareFormDictionaryController(mock.Object);

            // Act
            var actionResult = await controller.Get(testDictionaryId);

            // Assert
            var viewResult = Assert.IsType<ActionResult<MedicalCareFormDictionary>>(actionResult);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var model = Assert.IsType<MedicalCareFormDictionaryDTO>(okResult.Value);

            Assert.Equal("Экстренная", model.Name);
            Assert.Equal(1, model.Code);
        }

        [Fact]
        public async Task AddDictionary()
        {
            // Arrange
            var mock = new Mock<IRepository<MedicalCareFormDictionary>>();
            var controller = new MedicalCareFormDictionaryController(mock.Object);
            mock.Setup(repo => repo.Add(It.IsAny<MedicalCareFormDictionary>()))
                .Callback<MedicalCareFormDictionary>(dict => dict.Id = 4);

            var newDictionary = new MedicalCareFormDictionaryDTO
            {
                Code = 4,
                Name = "Test",
                BeginDate = new DateTime(2013, 1, 1),
                EndDate = DateTime.MaxValue
            };

            // Act
            var actionResult = await controller.Add(newDictionary);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var model = Assert.IsType<MedicalCareFormDictionaryDTO>(okResult.Value);

            Assert.Equal(4, model.Id);
            Assert.Equal(newDictionary.Code, model.Code);
            Assert.Equal(newDictionary.Name, model.Name);
            Assert.Equal(newDictionary.BeginDate, model.BeginDate);
            Assert.Equal(newDictionary.EndDate, model.EndDate);
        }

        [Fact]
        public async Task AddDictionaryWithBadRequest()
        {
            // Arrange
            var mock = new Mock<IRepository<MedicalCareFormDictionary>>();
            var controller = new MedicalCareFormDictionaryController(mock.Object);
            mock.Setup(repo => repo.Add(It.IsAny<MedicalCareFormDictionary>()))
                .Throws(new Exception("Failed to add dictionary"));

            var newDictionary = new MedicalCareFormDictionaryDTO
            {
                Code = 4,
                Name = "Test",
                BeginDate = new DateTime(2013, 1, 1),
                EndDate = DateTime.MaxValue
            };

            // Act
            var actionResult = await controller.Add(newDictionary);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task PutDictionaryWithNotFound()
        {
            // Arrange
            var testDictionaryId = 5;
            var mock = new Mock<IRepository<MedicalCareFormDictionary>>();
            mock.Setup(repo => repo.GetByKeyAsync(testDictionaryId))
                .ReturnsAsync(null as MedicalCareFormDictionary);
            var controller = new MedicalCareFormDictionaryController(mock.Object);

            var newDictionary = new MedicalCareFormDictionaryDTO
            {
                Id = testDictionaryId,
                Code = 4,
                Name = "Test",
                BeginDate = new DateTime(2013, 1, 1),
                EndDate = DateTime.MaxValue
            };

            // Act
            var actionResult = await controller.Update(testDictionaryId, newDictionary);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PutDictionary()
        {
            // Arrange
            var testDictionaryId = 1;
            var mock = new Mock<IRepository<MedicalCareFormDictionary>>();
            mock.Setup(repo => repo.GetByKeyAsync(testDictionaryId))
                .ReturnsAsync(GetTestDictionaries().FirstOrDefault(d => d.Id == testDictionaryId));
            var controller = new MedicalCareFormDictionaryController(mock.Object);

            var newDictionary = new MedicalCareFormDictionaryDTO
            {
                Id = testDictionaryId,
                Code = 4,
                Name = "Test",
                BeginDate = new DateTime(2013, 1, 1),
                EndDate = DateTime.MaxValue
            };

            // Act
            var actionResult = await controller.Update(testDictionaryId, newDictionary);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var model = Assert.IsType<MedicalCareFormDictionaryDTO>(okResult.Value);

            Assert.Equal(1, model.Id);
            Assert.Equal(newDictionary.Code, model.Code);
            Assert.Equal(newDictionary.Name, model.Name);
            Assert.Equal(newDictionary.BeginDate, model.BeginDate);
            Assert.Equal(newDictionary.EndDate, model.EndDate);
        }

        [Fact]
        public async Task DeleteDictionaryWithNotFound()
        {
            // Arrange
            int testDictionaryId = 5;
            var mock = new Mock<IRepository<MedicalCareFormDictionary>>();
            mock.Setup(repo => repo.GetByKeyAsync(testDictionaryId))
                .ReturnsAsync(null as MedicalCareFormDictionary);
            var controller = new MedicalCareFormDictionaryController(mock.Object);

            // Act
            var actionResult = await controller.Delete(testDictionaryId);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task DeleteDictionary()
        {
            // Arrange
            int testDictionaryId = 1;
            var mock = new Mock<IRepository<MedicalCareFormDictionary>>();

            var dictionaryToDelete = new MedicalCareFormDictionary
            {
                Id = testDictionaryId,
                Code = 1,
                Name = "Экстренная",
                BeginDate = new DateTime(2013, 1, 1),
                EndDate = DateTime.MaxValue
            };

            mock.Setup(repo => repo.GetByKeyAsync(testDictionaryId))
                .ReturnsAsync(dictionaryToDelete);

            mock.Setup(repo => repo.VirtualDelete(dictionaryToDelete, 0))
                .Verifiable();
            var controller = new MedicalCareFormDictionaryController(mock.Object);

            // Act
            var actionResult = await controller.Delete(testDictionaryId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal("Успешно удалено", okResult.Value);
        }

        //[Fact]
        public async Task UploadFile()
        {
            var mock = new Mock<IRepository<MedicalCareFormDictionary>>();
            var controller = new MedicalCareFormDictionaryController(mock.Object);

            var tempFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "test.xml");
            await File.WriteAllTextAsync(tempFilePath, "<packet>\r\n<zglv>\r\n<type>FRMMP</type>\r\n<version>2.0</version>\r\n<date>15.12.2017</date>\r\n</zglv>\r\n<zap>\r\n<IDFRMMP>1</IDFRMMP>\r\n<FRMMPNAME>Экстренная</FRMMPNAME>\r\n<DATEBEG>01.01.2013</DATEBEG>\r\n<DATEEND/>\r\n</zap>\r\n<zap>\r\n<IDFRMMP>2</IDFRMMP>\r\n<FRMMPNAME>Неотложная</FRMMPNAME>\r\n<DATEBEG>01.01.2013</DATEBEG>\r\n<DATEEND/>\r\n</zap>\r\n<zap>\r\n<IDFRMMP>3</IDFRMMP>\r\n<FRMMPNAME>Плановая</FRMMPNAME>\r\n<DATEBEG>01.01.2013</DATEBEG>\r\n<DATEEND/>\r\n</zap>\r\n</packet>");
        }

        private List<MedicalCareFormDictionary> GetTestDictionaries()
        {
            var dictionaries = new List<MedicalCareFormDictionary>
            {
                new MedicalCareFormDictionary {Id = 1, Code = 1, Name = "Экстренная", BeginDate = new DateTime(2013, 1, 1), EndDate = DateTime.MaxValue },
                new MedicalCareFormDictionary {Id = 2, Code = 2, Name = "Неотложная", BeginDate = new DateTime(2013, 1, 1), EndDate = DateTime.MaxValue },
                new MedicalCareFormDictionary {Id = 3, Code = 3, Name = "Плановая", BeginDate = new DateTime(2013, 1, 1), EndDate = DateTime.MaxValue },
            };

            return dictionaries;
        }
    }
}
