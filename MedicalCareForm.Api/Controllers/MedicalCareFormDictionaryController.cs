using MedicalCareForm.Api.Repositories;
using MedicalCareForm.Share.DTOs;
using MedicalCareForm.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalCareForm.Api.Services;
using System.Text;

namespace MedicalCareForm.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MedicalCareFormDictionaryController : ControllerBase
    {
        private readonly MedicalCareFormDictionaryRepository _repository;

        public MedicalCareFormDictionaryController(MedicalCareFormDictionaryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<MedicalCareFormDictionary>>> GetAllDictionaries()
        {
            var dictionaries = await _repository.GetAll().ToListAsync();

            var dictionariesDTO = dictionaries.Select(d => new MedicalCareFormDictionaryDTO
            {
                Id = d.Id,
                Code = d.Code,
                Name = d.Name,
                BeginDate = d.BeginDate,
                EndDate = d.EndDate
            });

            return Ok(dictionariesDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalCareFormDictionary>> GetDictionary(int id)
        {
            var dictionary = await _repository.GetByKeyAsync(id);

            if (dictionary == null)
            {
                return NotFound();
            }

            var dictionaryDTO = new MedicalCareFormDictionaryDTO
            {
                Id = dictionary.Id,
                Code = dictionary.Code,
                Name = dictionary.Name,
                BeginDate = dictionary.BeginDate,
                EndDate = dictionary.EndDate
            };


            return Ok(dictionaryDTO);
        }

        [HttpPost]
        public async Task<ActionResult<List<MedicalCareFormDictionary>>> AddDictionary(MedicalCareFormDictionaryDTO dictionaryDTO)
        {
            MedicalCareFormDictionary dictionary = new()
            {
                Code = dictionaryDTO.Code,
                Name = dictionaryDTO.Name,
                BeginDate = dictionaryDTO.BeginDate,
                EndDate = dictionaryDTO.EndDate
            };

            _repository.Add(dictionary);
            await _repository.SaveChangesAsync();

            return await GetAllDictionaries();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<MedicalCareFormDictionary>>> UpdateDictionary(int id, MedicalCareFormDictionaryDTO newDictionaryDTO)
        {
            var dictionary = await _repository.GetByKeyAsync(id);

            if (dictionary == null)
            {
                return NotFound();
            }

            dictionary.Code = newDictionaryDTO.Code;
            dictionary.Name = newDictionaryDTO.Name;
            dictionary.BeginDate = newDictionaryDTO.BeginDate;
            dictionary.EndDate = newDictionaryDTO.EndDate;

            await _repository.SaveChangesAsync();

            return await GetAllDictionaries();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<MedicalCareFormDictionary>>> DeleteDictionary(int id)
        {
            var dictionary = await _repository.GetByKeyAsync(id);

            if (dictionary == null)
            {
                return NotFound();
            }

            _repository.Delete(dictionary);
            await _repository.SaveChangesAsync();

            return await GetAllDictionaries();
        }

        //[HttpPost("upload")]
        //public async Task<ActionResult<List<MedicalCareFormDictionary>>> UploadFile(IFormFile file)
        //{
        //    Encoding encoding = Encoding.GetEncoding("windows-1251");
        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", file.FileName);
        //    try
        //    {

        //        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }
        //    }
        //    catch
        //    {
                
        //    }
        //    MedicalCareFormReader reader = new();
        //    List<MedicalCareFormDictionary> dictList = reader.ReadFromXml(filePath);
        //    return await GetAllDictionaries();
        //}
    }
}
