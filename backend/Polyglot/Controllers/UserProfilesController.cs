﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Polyglot.BusinessLogic.Implementations;
using Polyglot.BusinessLogic.Interfaces;
using Polyglot.Common.DTOs;
using Polyglot.DataAccess.Entities;
using Polyglot.DataAccess.SqlRepository;

namespace Polyglot.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private readonly ICRUDService<UserProfile, UserProfileDTO> service;

        public UserProfilesController(ICRUDService<UserProfile, UserProfileDTO> service, DbContext ctx)
        {
            //this.service = service;
#warning some hell shit
            var uow = new UnitOfWork(ctx);
            this.service = new CRUDService<UserProfile, UserProfileDTO>(uow, Polyglot.Common.Mapping.AutoMapper.GetDefaultMapper());
        }
        // GET: UserProfiles
        [HttpGet]
        public async Task<IActionResult> GetAllUserProfiles()
        {
            var projects = await service.GetListAsync();
            return projects == null ? NotFound("No user profiles found!") as IActionResult
                : Ok(projects);
        }

        // GET: UserProfiles/5
        [HttpGet("{id}", Name = "GetUserProfile")]
        public async Task<IActionResult> GetUserProfile(int id)
        {
            var project = await service.GetOneAsync(id);
            return project == null ? NotFound($"User profile with id = {id} not found!") as IActionResult
                : Ok(project);
        }

        // POST: UserProfiles
        public async Task<IActionResult> AddUserProfile([FromBody]UserProfileDTO project)
        {
            if (!ModelState.IsValid)
                return BadRequest() as IActionResult;

            var entity = await service.PostAsync(project);
            return entity == null ? StatusCode(409) as IActionResult
                : Created($"{Request?.Scheme}://{Request?.Host}{Request?.Path}{entity.Id}",
                entity);
        }

        // PUT: UserProfiles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifyUserProfile(int id, [FromBody]UserProfileDTO project)
        {
            if (!ModelState.IsValid)
                return BadRequest() as IActionResult;

            var entity = await service.PutAsync(project);
            return entity == null ? StatusCode(304) as IActionResult
                : Ok(entity);
        }

        // DELETE: ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserProfile(int id)
        {
            var success = await service.TryDeleteAsync(id);
            return success ? Ok() : StatusCode(304) as IActionResult;
        }
    }
}
