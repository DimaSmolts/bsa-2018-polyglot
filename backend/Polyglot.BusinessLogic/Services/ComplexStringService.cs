﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Polyglot.BusinessLogic.Interfaces;
using Polyglot.Common.DTOs.NoSQL;
using Polyglot.DataAccess.Interfaces;
using Polyglot.DataAccess.MongoModels;
using Polyglot.DataAccess.MongoRepository;
using Polyglot.DataAccess.SqlRepository;

namespace Polyglot.BusinessLogic.Services
{
    public class ComplexStringService : IComplexStringService
    {
        private readonly IMongoRepository<ComplexString> _repository;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ComplexStringService(IMongoRepository<ComplexString> repository, IMapper mapper, IUnitOfWork uow)
        {
            _uow = uow;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ComplexStringDTO>> GetListAsync()
        {

            var targets = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ComplexStringDTO>>(targets);
        }

        public async Task<ComplexStringDTO> GetComplexString(int identifier)
        {

            var target = await _repository.GetAsync(identifier);
            if (target != null)
            {
                return _mapper.Map<ComplexStringDTO>(target);
            }

            return null;

        }

        public async Task<IEnumerable<TranslationDTO>> GetStringTranslationsAsync(int identifier)
        {
            var target = await _repository.GetAsync(identifier);
            if (target != null)
            {
                return _mapper.Map<IEnumerable<TranslationDTO>>(target.Translations);
            }

            return null;
        }

        public async Task<ComplexStringDTO> SetStringTranslations(int identifier, IEnumerable<TranslationDTO> translations)
        {
            var target = await _repository.GetAsync(identifier);
            if (target != null)
            {
                target.Translations = _mapper.Map<List<Translation>>(translations);
                var result = await _repository.Update(_mapper.Map<ComplexString>(target));
                return _mapper.Map<ComplexStringDTO>(result);
            }
            return null;

        }

        public async Task<ComplexStringDTO> EditStringTranslation(int identifier,TranslationDTO translation)
        {
            var target = await _repository.GetAsync(identifier);
            if (target != null)
            {
                var translationsList= _mapper.Map<List<Translation>>(target.Translations);
                var currentTranslation = translationsList.FirstOrDefault(x => x.Id == translation.Id);
                currentTranslation.History.Add(new AdditionalTranslation
                {
                    CreatedOn = currentTranslation.CreatedOn,
                    TranslationValue = currentTranslation.TranslationValue,
                    UserId = currentTranslation.UserId
                });
                currentTranslation.TranslationValue = translation.TranslationValue;
                currentTranslation.UserId = translation.UserId;
                currentTranslation.CreatedOn=DateTime.Now;
                target.Translations = translationsList;

                var result = await _repository.Update(_mapper.Map<ComplexString>(target));
                return _mapper.Map<ComplexStringDTO>(result);
            }
            return null;

        }

        public async Task<ComplexStringDTO> ModifyComplexString(ComplexStringDTO entity)
        {
            var sqlComplexString = new Polyglot.DataAccess.Entities.ComplexString
            {
                TranslationKey = entity.Key,
                ProjectId = entity.ProjectId
            };
            await _uow.GetRepository<Polyglot.DataAccess.Entities.ComplexString>().CreateAsync(sqlComplexString);
            await _uow.SaveAsync();
            var target = await _repository.Update(_mapper.Map<ComplexString>(entity));
            if (target != null)
            {
                return _mapper.Map<ComplexStringDTO>(target);
            }
            return null;

        }

        public async Task<bool> DeleteComplexString(int identifier)
        {
            await _uow.GetRepository<Polyglot.DataAccess.Entities.ComplexString>().DeleteAsync(identifier);
            await _uow.SaveAsync();
            await _repository.DeleteAsync(identifier);

            return true;
        }

        public async Task<ComplexStringDTO> AddComplexString(ComplexStringDTO entity)
        {
            var sqlComplexString = new Polyglot.DataAccess.Entities.ComplexString
            {
                TranslationKey = entity.Key,
                ProjectId = entity.ProjectId
            };
            var savedEntity = await _uow.GetRepository<Polyglot.DataAccess.Entities.ComplexString>().CreateAsync(sqlComplexString);
            await _uow.SaveAsync();
            entity.Id = savedEntity.Id;
            var target = await _repository
                .CreateAsync(_mapper.Map<ComplexString>(entity));
            if (target != null)
            {

                return _mapper.Map<ComplexStringDTO>(target);
            }
            return null;
        }
    }
}
