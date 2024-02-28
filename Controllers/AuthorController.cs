using Microsoft.AspNetCore.Mvc;
using Pankaj_New_Code28.Models;
using Pankaj_New_Code28.Data;
using Pankaj_New_Code28.Filter;
using Pankaj_New_Code28.Entities;
using Pankaj_New_Code28.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Pankaj_New_Code28.Controllers
{
    /// <summary>
    /// Controller responsible for managing author-related operations in the API.
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for adding, retrieving, updating, and deleting author information.
    /// </remarks>
    [Route("api/author")]
    [Authorize]
    public class AuthorController : ControllerBase
    {
        private readonly Pankaj_New_Code28Context _context;

        public AuthorController(Pankaj_New_Code28Context context)
        {
            _context = context;
        }

        /// <summary>Adds a new author to the database</summary>
        /// <param name="model">The author data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [UserAuthorize("Author",Entitlements.Create)]
        public IActionResult Post([FromBody] Author model)
        {
            _context.Author.Add(model);
            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }

        /// <summary>Retrieves a list of authors based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"Property": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <returns>The filtered list of authors</returns>
        [HttpGet]
        [UserAuthorize("Author",Entitlements.Read)]
        public IActionResult Get([FromQuery] string filters)
        {
            List<FilterCriteria> filterCriteria = null;
            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var query = _context.Author.AsQueryable();
            var result = FilterService<Author>.ApplyFilter(query, filterCriteria);
            return Ok(result);
        }

        /// <summary>Retrieves a specific author by its primary key</summary>
        /// <param name="entityId">The primary key of the author</param>
        /// <returns>The author data</returns>
        [HttpGet]
        [Route("{entityId:Guid}")]
        [UserAuthorize("Author",Entitlements.Read)]
        public IActionResult GetById([FromRoute] Guid entityId)
        {
            var entityData = _context.Author.FirstOrDefault(entity => entity.Id == entityId);
            return Ok(entityData);
        }

        /// <summary>Deletes a specific author by its primary key</summary>
        /// <param name="entityId">The primary key of the author</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("Author",Entitlements.Delete)]
        [Route("{entityId:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid entityId)
        {
            var entityData = _context.Author.FirstOrDefault(entity => entity.Id == entityId);
            if (entityData == null)
            {
                return NotFound();
            }

            _context.Author.Remove(entityData);
            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }

        /// <summary>Updates a specific author by its primary key</summary>
        /// <param name="entityId">The primary key of the author</param>
        /// <param name="updatedEntity">The author data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("Author",Entitlements.Update)]
        [Route("{entityId:Guid}")]
        public IActionResult UpdateById(Guid entityId, [FromBody] Author updatedEntity)
        {
            if (entityId != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var entityData = _context.Author.FirstOrDefault(entity => entity.Id == entityId);
            if (entityData == null)
            {
                return NotFound();
            }

            var propertiesToUpdate = typeof(Author).GetProperties().Where(property => property.Name != "Id").ToList();
            foreach (var property in propertiesToUpdate)
            {
                property.SetValue(entityData, property.GetValue(updatedEntity));
            }

            var returnData = this._context.SaveChanges();
            return Ok(returnData);
        }
    }
}