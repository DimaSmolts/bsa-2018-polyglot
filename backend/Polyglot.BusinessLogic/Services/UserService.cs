﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Polyglot.BusinessLogic.Interfaces;
using Polyglot.DataAccess.SqlRepository;
using Polyglot.DataAccess.Entities;
using Polyglot.Common.DTOs;
using Polyglot.Core.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Polyglot.BusinessLogic.Services
{
    [Authorize]
    public class UserService : CRUDService<UserProfile, UserProfileDTO>, IUserService
    {
        public UserService(IUnitOfWork uow, IMapper mapper)
            :base(uow, mapper)
        {

        }



        public async Task<UserProfileDTO> GetByUidAsync()
        {
            var user = await CurrentUser.GetCurrentUserProfile();
            if (user == null)
            {
                return null;
            }

            return mapper.Map<UserProfile, UserProfileDTO>(user);
        }

        public async Task<bool> IsExistByUidAsync()
        {
            var user = await CurrentUser.GetCurrentUserProfile();
            return user != null;
        }

        public async Task<bool> PutUserBool(UserProfileDTO userProfileDTO)
        {
            var result = await uow.GetRepository<UserProfile>().UpdateBool((mapper.Map<UserProfile>(userProfileDTO)));
            if (result)
            {
                await uow.SaveAsync();
                return true;
            }

            return false;
        }
    }
}
