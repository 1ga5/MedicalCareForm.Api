using MedicalCareForm.Api.Repositories;
using MedicalCareForm.Data.Models;
using Moq;

namespace MedicalCareForm.Test.Helpers
{
    public static class MockHelper
    {
        public static Mock<IRepository<MedicalCareFormDictionary>> CreateMock(List<MedicalCareFormDictionary> testDictionaries)
        {
            var mock = new Mock<IRepository<MedicalCareFormDictionary>>();

            mock.Setup(repo => repo.GetAll())
                .Returns(testDictionaries.AsAsyncQueryable());

            return mock;
        }
    }
}
