﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polyglot.BusinessLogic.Interfaces;
using Polyglot.Common.DTOs;
using Polyglot.DataAccess.Entities;

namespace Polyglot.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ManagersController : ControllerBase
    {
        private readonly ICRUDService service;

        public ManagersController(ICRUDService service)
        {
            this.service = service;
        }

        // GET: Managers
        [HttpGet]
        public async Task<IActionResult> GetAllManagers()
        {
            var managers = await service.GetListAsync<Manager, ManagerDTO>();
            return managers == null ? NotFound("No managers found!") as IActionResult
                : Ok(managers);
        }

        // GET: Managers/5
        [HttpGet("{id}", Name = "GetManager")]
        public async Task<IActionResult> GetManager(int id)
        {
            var manager = await service.GetOneAsync<Manager, ManagerDTO>(id);
            return manager == null ? NotFound($"Manager with id = {id} not found!") as IActionResult
                : Ok(manager);
        }

        // POST: Managers
        public async Task<IActionResult> AddManager([FromBody]ManagerDTO project)
        {
            if (!ModelState.IsValid)
                return BadRequest() as IActionResult;

            var entity = await service.PostAsync<Manager, ManagerDTO>(project);
            return entity == null ? StatusCode(409) as IActionResult
                : Created($"{Request?.Scheme}://{Request?.Host}{Request?.Path}{entity.Id}",
                entity);
        }

        // PUT: Managers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifyManager(int id, [FromBody]ManagerDTO project)
        {
            if (!ModelState.IsValid)
                return BadRequest() as IActionResult;

            var entity = await service.PutAsync<Manager, ManagerDTO>(project);
            return entity == null ? StatusCode(304) as IActionResult
                : Ok(entity);
        }

        // DELETE: ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManager(int id)
        {
            var success = await service.TryDeleteAsync<Manager>(id);
            return success ? Ok() : StatusCode(304) as IActionResult;
        }
    }
}
