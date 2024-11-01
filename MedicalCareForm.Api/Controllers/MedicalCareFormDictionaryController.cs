using MedicalCareForm.Api.Repositories;
using MedicalCareForm.Share.DTOs;
using MedicalCareForm.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalCareForm.Api.Services;

namespace MedicalCareForm.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MedicalCareFormDictionaryController : ControllerBase
    {
        private readonly IRepository<MedicalCareFormDictionary> _repository;

        public MedicalCareFormDictionaryController(IRepository<MedicalCareFormDictionary> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<MedicalCareFormDictionary>>> GetAll()
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
        public async Task<ActionResult<MedicalCareFormDictionary>> Get(int id)
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
        public async Task<ActionResult<MedicalCareFormDictionaryDTO>> Add(MedicalCareFormDictionaryDTO dictionaryDTO)
        {
            try
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

                dictionaryDTO.Id = dictionary.Id;

                return Ok(dictionaryDTO);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<MedicalCareFormDictionary>>> Update(int id, MedicalCareFormDictionaryDTO newDictionaryDTO)
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

            newDictionaryDTO.Id = dictionary.Id;

            return Ok(newDictionaryDTO);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<MedicalCareFormDictionary>>> Delete(int id)
        {
            var dictionary = await _repository.GetByKeyAsync(id);

            if (dictionary == null)
            {
                return NotFound();
            }

            await _repository.VirtualDelete(dictionary, 0);
            await _repository.SaveChangesAsync();

            return Ok("Успешно удалено");
        }

        [HttpPost("upload")]
        public async Task<ActionResult<List<MedicalCareFormDictionary>>> UploadFile(IFormFile file)
        {

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", file.FileName);

            //Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            };
            
            MedicalCareFormReader reader = new();
            List<MedicalCareFormDictionary> newDictionariesList = reader.ReadFromXml(filePath);

            var oldDictionaries = await _repository.GetAll().ToListAsync();
            foreach (var oldDictionary in oldDictionaries)
            {
                var dictionaryToDelete = await _repository.GetByKeyAsync(oldDictionary.Id);
                if (dictionaryToDelete is not null)
                {
                    await _repository.VirtualDelete(dictionaryToDelete, 0);
                }
            }
            await _repository.SaveChangesAsync();

            foreach (var newDictionary in newDictionariesList)
            {
                var dictionary = new MedicalCareFormDictionary
                {
                    Id = newDictionary.Id,
                    Code = newDictionary.Code,
                    Name = newDictionary.Name,
                    BeginDate = newDictionary.BeginDate,
                    EndDate = newDictionary.EndDate
                };
                _repository.Add(dictionary);
            }
            await _repository.SaveChangesAsync();

            var newDictionaries = await _repository.GetAll().ToListAsync();

            var newDictionariesDTO = newDictionaries.Select(d => new MedicalCareFormDictionaryDTO
            {
                Id = d.Id,
                Code = d.Code,
                Name = d.Name,
                BeginDate = d.BeginDate,
                EndDate = d.EndDate
            });

            return Ok(newDictionariesDTO);
        }
    }
}
