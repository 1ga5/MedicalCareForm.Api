using System.Text;
using System.Xml.Linq;
using MedicalCareForm.Api.Services.DictionaryInterfaces;
using MedicalCareForm.Data.Models;


namespace MedicalCareForm.Api.Services
{
    public class MedicalCareFormReader : IDictionaryXMlReader<MedicalCareFormDictionary>
    {
        public List<MedicalCareFormDictionary> ReadFromXml(string filePath)
        {
            List<MedicalCareFormDictionary> medicalCareFormList = [];
            XDocument xdoc = XDocument.Load(filePath);
            XElement? packet = xdoc.Element("packet");
            if (packet is not null)
            {
                foreach (XElement zap in packet.Elements("zap"))
                {
                    var codeValue = zap.Element("IDFRMMP")?.Value;
                    var nameValue = zap.Element("FRMMPNAME")?.Value;
                    var beginDateValue = zap.Element("DATEBEG")?.Value;
                    var endDateValue = zap.Element("DATEEND")?.Value;

                    medicalCareFormList.Add(new MedicalCareFormDictionary
                    {
                        Code = string.IsNullOrEmpty(codeValue) ? 0 : int.Parse(codeValue),
                        Name = string.IsNullOrEmpty(nameValue) ? "unknown" : nameValue,
                        BeginDate = string.IsNullOrWhiteSpace(beginDateValue) ? DateTime.MinValue : DateTime.Parse(beginDateValue),
                        EndDate = string.IsNullOrWhiteSpace(endDateValue) ? DateTime.MaxValue : DateTime.Parse(endDateValue)
                    });
                }
            }

            return medicalCareFormList;
        }
    }
}

