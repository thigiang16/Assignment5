using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanWiseApi.Data;
using PlanWiseApi.Models;
using PlanWiseApi.Services;

namespace PlanWiseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventTemplatesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TemplateIndexService _templateIndexService;

        public EventTemplatesController(
            AppDbContext context,
            TemplateIndexService templateIndexService)
        {
            _context = context;
            _templateIndexService = templateIndexService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var templates = await _context.EventTemplates
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return Ok(templates);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var template = await _context.EventTemplates.FindAsync(id);

            if (template == null)
            {
                return NotFound();
            }

            return Ok(template);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventTemplate template)
        {
            template.CreatedAt = DateTime.UtcNow;

            _context.EventTemplates.Add(template);
            await _context.SaveChangesAsync();

            await _templateIndexService.IndexTemplateAsync(template);

            return CreatedAtAction(nameof(GetById), new { id = template.Id }, template);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, EventTemplate template)
        {
            if (id != template.Id)
            {
                return BadRequest("Route id does not match payload id.");
            }

            var exists = await _context.EventTemplates.AnyAsync(t => t.Id == id);

            if (!exists)
            {
                return NotFound();
            }

            _context.Entry(template).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            await _templateIndexService.IndexTemplateAsync(template);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var template = await _context.EventTemplates.FindAsync(id);

            if (template == null)
            {
                return NotFound();
            }

            _context.EventTemplates.Remove(template);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}