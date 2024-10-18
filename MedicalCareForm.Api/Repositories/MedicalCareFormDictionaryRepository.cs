using MedicalCareForm.Data;
using MedicalCareForm.Data.Models;
using MedicalCareForm.Repositories;

namespace MedicalCareForm.Api.Repositories
{
    public class MedicalCareFormDictionaryRepository : GenericRepository<MedicalCareFormDictionary>
    {
        public MedicalCareFormDictionaryRepository(AppDbContext context) : base(context)
        {

        }
    }
}
