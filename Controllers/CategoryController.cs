using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Category>>> Get() {
            return new List<Category>();
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Category>> GetById(int id) {
            return new Category();
        }

        [HttpPost]
        public async Task<ActionResult<List<Category>>> Post([FromBody]Category model, [FromServices]DataContext context) {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try {
                context.Categories.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            } 
            catch(Exception) {
                return BadRequest(new { message = "Não encontrado" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<List<Category>>> Put(int id, [FromBody]Category model, [FromServices]DataContext context) {
            if (model.Id != id) return NotFound(new { message = "Categoria não encontrada" }); 

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException) {
                return BadRequest(new { message = "Não foi possível atualizar a categoria" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível atualizar a categoria" });
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<List<Category>>> Delete(int id, [FromServices]DataContext context) {
            
            var categoria = await context.Categories.FirstOrDefaultAsync(cat => cat.Id == id);

            if (categoria == null) return NotFound(new { message = "Categoria não encontrada" });

            try
            {
                context.Categories.Remove(categoria);
                await context.SaveChangesAsync();
                return Ok(new { message = "Categoria removida com sucesso" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível remover a categoria" });
            }
        }
    }
}