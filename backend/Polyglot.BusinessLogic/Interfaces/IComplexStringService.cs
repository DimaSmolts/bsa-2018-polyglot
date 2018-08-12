﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Polyglot.Common.DTOs.NoSQL;
using Polyglot.DataAccess.MongoModels;

namespace Polyglot.BusinessLogic.Interfaces
{
    public interface IComplexStringService
    {
        Task<IEnumerable<ComplexStringDTO>> GetListAsync();

        Task<ComplexStringDTO> GetComplexString(int identifier);
        
        Task<ComplexStringDTO> ModifyComplexString(ComplexStringDTO entity);

        Task<bool> DeleteComplexString(int identifier);

        Task<ComplexStringDTO> AddComplexString(ComplexStringDTO entity);
    }
}
