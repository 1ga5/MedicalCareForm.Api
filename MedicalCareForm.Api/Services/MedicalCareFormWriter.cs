using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using MedicalCareForm.Api.Services.DictionaryInterfaces;
using MedicalCareForm.Data.Models;

namespace MedicalCareForm.Api.Services
{
    class MedicalCareFormWriter : IDictionaryJsonWriter<MedicalCareFormDictionary>
    {
        // Для отображения кириллицы в json файле
        private readonly JsonSerializerOptions options1 = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            WriteIndented = true
        };

        public bool WriteToJson(List<MedicalCareFormDictionary> dictionaryList, string outputPath)
        {
            string json = JsonSerializer.Serialize(dictionaryList, options1);
            File.WriteAllText(outputPath, json);
            return true;
        }
    }
}

