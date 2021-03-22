using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("users")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices]DataContext context) {
            var users = await context.Users
                .AsNoTracking()
                .ToListAsync();
            return users;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Post(
            [FromServices]DataContext context,
            [FromBody]User model
        )
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                context.Users.Add(model);
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromServices]DataContext context,
            [FromBody]User model
        )
        {
            var user = await context.Users
                .AsNoTracking()
                .Where(x => x.username == model.username && x.Password == model.Password)
                .FirstOrDefaultAsync();

            if (user == null) return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = TokenService.GenerateToken(user);
            return new {
                username = user.username,
                userRole = user.Role,
                token = token
            };
        }

        [HttpPut] 
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(
            [FromServices]DataContext context,
            int id,
            [FromBody]User model
        )
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id != model.Id) return NotFound(new { message = "Usuário não encontrado" });

            try
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível atualizar o usuário" });
            }
        }
    }
}