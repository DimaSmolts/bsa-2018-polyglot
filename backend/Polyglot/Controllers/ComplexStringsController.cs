﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Polyglot.BusinessLogic.Interfaces;
using Polyglot.Common.DTOs;
using Polyglot.Common.DTOs.NoSQL;
using Polyglot.DataAccess.Entities;
using Polyglot.DataAccess.FileRepository;
using Polyglot.DataAccess.Interfaces;
using Polyglot.DataAccess.MongoModels;
using Polyglot.DataAccess.MongoRepository;

namespace Polyglot.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ComplexStringsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IComplexStringService dataProvider;
        private readonly ICRUDService<Language, LanguageDTO> service;

        public IFileStorageProvider fileStorageProvider;
        public ComplexStringsController(IComplexStringService dataProvider, IMapper mapper, IFileStorageProvider provider, ICRUDService<Language, LanguageDTO> service)
        {
            this.dataProvider = dataProvider;
            this.mapper = mapper;
            fileStorageProvider = provider;
            this.service = service;
        }

        // GET: ComplexStrings
        [HttpGet]
        public async Task<IActionResult> GetAllComplexStrings()
        {
            //await dataProvider.AddComplexString(new ComplexStringDTO { Key = "asdf", ProjectId = 1, Description = "", Comments = new List<CommentDTO>(), OriginalValue = "asdfgh", Translations = new List<Common.DTOs.NoSQL.TranslationDTO> { new Common.DTOs.NoSQL.TranslationDTO { CreatedOn = DateTime.Now, Language = "Russian", TranslationValue = "фыва" } }});
            var complexStrings = await dataProvider.GetListAsync();
            return complexStrings == null ? NotFound("No files found!") as IActionResult
                : Ok(mapper.Map<IEnumerable<ComplexStringDTO>>(complexStrings));
        }

        // GET: ComplexStrings/5
        [HttpGet("{id}", Name = "GetComplexString")]
        public async Task<IActionResult> GetComplexString(int id)
        {
            var complexString = await dataProvider.GetComplexString(id);
            return complexString == null ? NotFound($"ComplexString with id = {id} not found!") as IActionResult
                : Ok(mapper.Map<ComplexStringDTO>(complexString));
        }

        // GET: ComplexStrings/5/translations
        [HttpGet("{id}/translations", Name = "GetComplexStringTranslations")]
        public async Task<IActionResult> GetComplexStringTranslations(int id)
        {
            var translation = await dataProvider.GetStringTranslationsAsync(id);
            return translation == null ? NotFound($"ComplexString with id = {id} not found!") as IActionResult
                : Ok(mapper.Map<IEnumerable<Common.DTOs.NoSQL.TranslationDTO>>(translation));
        }

        // POST: ComplexStrings
        [HttpPost]
        public async Task<IActionResult> AddComplexString()
        {
            Request.Form.TryGetValue("str", out StringValues res);
            ComplexStringDTO complexString = JsonConvert.DeserializeObject<ComplexStringDTO>(res);
            if (Request.Form.Files.Count != 0)
            {
                IFormFile file = Request.Form.Files[0];
                byte[] byteArr;
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    await file.CopyToAsync(ms);
                    byteArr = ms.ToArray();
                }


                complexString.PictureLink = await fileStorageProvider.UploadFileAsync(byteArr, FileType.Photo, Path.GetExtension(file.FileName));
            }
            var entity = await dataProvider.AddComplexString(complexString);
            return entity == null ? StatusCode(409) as IActionResult
                : Created($"{Request?.Scheme}://{Request?.Host}{Request?.Path}{entity.Id}",
                mapper.Map<ComplexStringDTO>(entity)); 
        }

        // PUT: ComplexStrings/5
        [HttpPut("{id}")]
        public IActionResult ModifyComplexString(int id, [FromBody]ComplexStringDTO complexString)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            complexString.Id = id;

            var entity = dataProvider.ModifyComplexString(complexString);
            return entity == null ? StatusCode(304) as IActionResult
                : Ok(mapper.Map<ComplexStringDTO>(entity));
        }

        // DELETE: ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComplexString(int id)
        {
            var success = await dataProvider.DeleteComplexString(id);
            return success == null ? Ok() : StatusCode(304);
        }
    }
}
