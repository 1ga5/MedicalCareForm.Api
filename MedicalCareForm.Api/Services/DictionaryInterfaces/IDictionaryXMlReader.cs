using MedicalCareForm.Data.Models;

namespace MedicalCareForm.Api.Services.DictionaryInterfaces
{
    interface IDictionaryXMlReader<T> where T : DictionaryBaseType
    {
        List<T> ReadFromXml(string filePath);
    }
}
