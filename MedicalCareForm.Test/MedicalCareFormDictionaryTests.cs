using Xunit;
using MedicalCareForm.Api.Controllers;
using MedicalCareForm.Api.Repositories;
using MedicalCareForm.Share.DTOs;
using MedicalCareForm.Data.Models;
using MedicalCareForm.Api.Services.DictionaryInterfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MedicalCareForm.Test.Helpers;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json.Linq;



namespace MedicalCareForm.Test
{
    public class MedicalCareFormDictionaryTests
    {
        [Fact]
        public async Task Get_AllDictionaries_ReturnsOkObjectResultWithListOfDictionaries()
        {
            // Arrange
            var mock = MockHelper.CreateMock(GetTestDictionaries());
            var controller = new MedicalCareFormDictionaryController(mock.Object);
            var expectedDictionaries = GetTestDictionaries().Select(d => new MedicalCareFormDictionaryDTO
            {
                Id = d.Id,
                Code = d.Code,
                Name = d.Name,
                BeginDate = d.BeginDate,
                EndDate = d.EndDate
            }).ToList();

            // Act
            var actionResult = await controller.GetAll();

            // Assert
            actionResult.Should().BeOfType<ActionResult<List<MedicalCareFormDictionary>>>();
            actionResult.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(expectedDictionaries);
        }

        [Fact]
        public async Task Get_DictionaryByNotExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            int testDictionaryId = 5;
            var mock = MockHelper.CreateMock(GetTestDictionaries());
            mock.Setup(repo => repo.GetByKeyAsync(testDictionaryId))
                .ReturnsAsync(null as MedicalCareFormDictionary);
            var controller = new MedicalCareFormDictionaryController(mock.Object);

            // Act
            var actionResult = await controller.Get(testDictionaryId);

            // Assert
            actionResult.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Get_DictionaryById_ReturnsOkObjectResultWithDictionary()
        {
            // Arrange
            int testDictionaryId = 1;
            var tookedDictionary = GetTestDictionaries().FirstOrDefault(d => d.Id == testDictionaryId);
            var mock = MockHelper.CreateMock(GetTestDictionaries());
            mock.Setup(repo => repo.GetByKeyAsync(testDictionaryId))
                .ReturnsAsync(tookedDictionary);
            var controller = new MedicalCareFormDictionaryController(mock.Object);

            // Act
            var actionResult = await controller.Get(testDictionaryId);

            // Assert
            actionResult.Should().BeOfType<ActionResult<MedicalCareFormDictionary>>();
            actionResult.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<MedicalCareFormDictionaryDTO>()
                .Which.Should().Match<MedicalCareFormDictionaryDTO>(model => model.Name == tookedDictionary.Name 
                && model.Code == tookedDictionary.Code);
        }

        [Fact]
        public async Task Add_ValidDictionary_ReturnsOkObjectResultWithDictionary()
        {
            // Arrange
            var newDictionary = new MedicalCareFormDictionaryDTO
            {
                Code = 4,
                Name = "Test",
                BeginDate = new DateTime(2013, 1, 1),
                EndDate = DateTime.MaxValue
            };

            var mock = MockHelper.CreateMock(GetTestDictionaries());
            mock.Setup(repo => repo.Add(It.IsAny<MedicalCareFormDictionary>()))
                .Callback<MedicalCareFormDictionary>(dict => dict.Id = 4);
            var controller = new MedicalCareFormDictionaryController(mock.Object);

            // Act
            var actionResult = await controller.Add(newDictionary);

            // Assert
            actionResult.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<MedicalCareFormDictionaryDTO>()
                .Which.Should().Match<MedicalCareFormDictionaryDTO>(model => 
                model.Id == 4 &&
                model.Code == newDictionary.Code &&
                model.Name == newDictionary.Name &&
                model.BeginDate == newDictionary.BeginDate &&
                model.EndDate == newDictionary.EndDate);
        }

        [Fact]
        public async Task Add_NotValidDictionary_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var mock = MockHelper.CreateMock(GetTestDictionaries());
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
            actionResult.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Put_DictionaryWithNotExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var testDictionaryId = 5;
            var newDictionary = new MedicalCareFormDictionaryDTO
            {
                Id = testDictionaryId,
                Code = 4,
                Name = "Test",
                BeginDate = new DateTime(2013, 1, 1),
                EndDate = DateTime.MaxValue
            };

            var mock = MockHelper.CreateMock(GetTestDictionaries());
            mock.Setup(repo => repo.GetByKeyAsync(testDictionaryId))
                .ReturnsAsync(null as MedicalCareFormDictionary);
            var controller = new MedicalCareFormDictionaryController(mock.Object);

            // Act
            var actionResult = await controller.Update(testDictionaryId, newDictionary);

            // Assert
            actionResult.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Put_ValidDictionary_ReturnsOkObjectResultWithDictionary()
        {
            // Arrange
            var testDictionaryId = 1;
            var newDictionary = new MedicalCareFormDictionaryDTO
            {
                Id = testDictionaryId,
                Code = 4,
                Name = "Test",
                BeginDate = new DateTime(2013, 1, 1),
                EndDate = DateTime.MaxValue
            };

            var mock = MockHelper.CreateMock(GetTestDictionaries());
            mock.Setup(repo => repo.GetByKeyAsync(testDictionaryId))
                .ReturnsAsync(GetTestDictionaries().FirstOrDefault(d => d.Id == testDictionaryId));
            var controller = new MedicalCareFormDictionaryController(mock.Object);

            // Act
            var actionResult = await controller.Update(testDictionaryId, newDictionary);

            // Assert
            actionResult.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<MedicalCareFormDictionaryDTO>()
                .Which.Should().Match<MedicalCareFormDictionaryDTO>(model =>
                model.Id == newDictionary.Id &&
                model.Code == newDictionary.Code &&
                model.Name == newDictionary.Name &&
                model.BeginDate == newDictionary.BeginDate &&
                model.EndDate == newDictionary.EndDate);
        }

        [Fact]
        public async Task Delete_DictionaryWithNotExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            int testDictionaryId = 5;
            var mock = MockHelper.CreateMock(GetTestDictionaries());
            mock.Setup(repo => repo.GetByKeyAsync(testDictionaryId))
                .ReturnsAsync(null as MedicalCareFormDictionary);
            var controller = new MedicalCareFormDictionaryController(mock.Object);

            // Act
            var actionResult = await controller.Delete(testDictionaryId);

            // Assert
            actionResult.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ValidDictionary_ReturnsOkObjectResultWithMessage()
        {
            // Arrange
            int testDictionaryId = 1;
            var mock = MockHelper.CreateMock(GetTestDictionaries());

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
            actionResult.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be("Успешно удалено");
        }

        [Fact]
        public async Task Upload_ValidFile_ReturnsOkObjectResultWithListOfDictionaries()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "<packet>\r\n<zglv>\r\n<type>FRMMP</type>\r\n<version>2.0</version>\r\n<date>15.12.2017</date>\r\n</zglv>\r\n<zap>\r\n<IDFRMMP>1</IDFRMMP>\r\n<FRMMPNAME>Экстренная</FRMMPNAME>\r\n<DATEBEG>01.01.2013</DATEBEG>\r\n<DATEEND/>\r\n</zap>\r\n<zap>\r\n<IDFRMMP>2</IDFRMMP>\r\n<FRMMPNAME>Неотложная</FRMMPNAME>\r\n<DATEBEG>01.01.2013</DATEBEG>\r\n<DATEEND/>\r\n</zap>\r\n<zap>\r\n<IDFRMMP>3</IDFRMMP>\r\n<FRMMPNAME>Плановая</FRMMPNAME>\r\n<DATEBEG>01.01.2013</DATEBEG>\r\n<DATEEND/>\r\n</zap>\r\n</packet>";
            var fileName = "V014Test.xml";

            Directory.CreateDirectory("Files");
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", fileName);

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), CancellationToken.None))
                .Callback<Stream, CancellationToken>((stream, token) => ms.CopyTo(stream))
                .Returns(Task.CompletedTask);
            var file = fileMock.Object;

            var readerMock = new Mock<IDictionaryXMlReader<MedicalCareFormDictionary>>();
            readerMock.Setup(reader => reader.ReadFromXml(It.IsAny<string>())).Returns(GetTestDictionaries());

            var controllerMock = MockHelper.CreateMock(GetTestDictionaries());

            var addedDictionaries = new List<MedicalCareFormDictionary>();
            int idNext = 1;
            controllerMock.Setup(repo => repo.Add(It.IsAny<MedicalCareFormDictionary>()))
                .Callback<MedicalCareFormDictionary>(dict => 
                {
                    dict.Id = idNext++;
                    addedDictionaries.Add(dict);
                });

            var controller = new MedicalCareFormDictionaryController(controllerMock.Object);

            // Act
            var actionResult = await controller.UploadFile(file);

            // Assert
            actionResult.Should().BeOfType<ActionResult<List<MedicalCareFormDictionary>>>();
            addedDictionaries.Should().BeEquivalentTo(GetTestDictionaries());

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
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
