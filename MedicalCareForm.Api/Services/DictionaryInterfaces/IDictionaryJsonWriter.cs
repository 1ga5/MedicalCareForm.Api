using MedicalCareForm.Data.Models;

namespace MedicalCareForm.Api.Services.DictionaryInterfaces
{
    interface IDictionaryJsonWriter<T> where T : DictionaryBaseType
    {
        bool WriteToJson(List<T> dictionaryList, string outputPath);
    }
}