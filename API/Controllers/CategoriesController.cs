using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Domain.Entities;
using Dtos;
using System.Linq.Expressions;
using API.Mappers;
using Domain.Services;

namespace API.Controllers
{
    [Authorize]
    public class CategoriesController : ApiController
    {
        private readonly ICategoryService _categoryService;

        private static readonly Expression<Func<Category, CategoryDto>> AsCategoryDto =
            x => new CategoryDto
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName
            };
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Categories
        public IQueryable<CategoryDto> GetCategories()
        {
            return _categoryService.GetAll().Select(AsCategoryDto);
        }

        // GET: api/Categories/5
        [ResponseType(typeof(CategoryDto))]
        public async Task<IHttpActionResult> GetCategory(int id)
        {
            Category category = await _categoryService.GetSingle(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(Mapping.ToCategoryDto(category));
        }

        // PUT: api/Categories/5
        [ResponseType(typeof(CategoryDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> PutCategory(int id, CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != categoryDto.CategoryId)
            {
                return BadRequest("Mismatch");
            }

            var updatedCategory = await _categoryService.Edit(Mapping.ToCategory(categoryDto));

            return Ok(Mapping.ToCategoryDto(updatedCategory));
        }

        // POST: api/Categories
        [ResponseType(typeof(CategoryDto))]
        public async Task<IHttpActionResult> PostCategory(CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _categoryService.Add(Mapping.ToCategory(categoryDto));

            return CreatedAtRoute("DefaultApi", new { id = category.CategoryId }, Mapping.ToCategoryDto(category));
        }

        // DELETE: api/Categories/5
        [ResponseType(typeof(CategoryDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> DeleteCategory(int id)
        {
            Category category = await _categoryService.GetSingle(id);

            Category deletedCategory = await _categoryService.Delete(category);

            return Ok(Mapping.ToCategoryDto(deletedCategory));
        }
    }
}