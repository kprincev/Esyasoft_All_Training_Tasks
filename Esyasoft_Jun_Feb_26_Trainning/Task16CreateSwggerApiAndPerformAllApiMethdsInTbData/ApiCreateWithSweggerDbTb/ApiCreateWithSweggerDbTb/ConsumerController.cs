using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ApiCreateWithSweggerDbTb.Models;

[Route("api/[controller]")]
[ApiController]
public class ConsumerController : ControllerBase
{
    private readonly AppDbContext _context;

    public ConsumerController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ GET: api/consumer
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Consumer>>> GetAll()
    {
        return await _context.Consumers.ToListAsync();
    }

    // ✅ GET: api/consumer/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Consumer>> GetById(int id)
    {
        var consumer = await _context.Consumers.FindAsync(id);

        if (consumer == null)
            return NotFound();

        return consumer;
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> InsertBulk(List<Consumer> consumers)
    {
        if (consumers == null || consumers.Count == 0)
            return BadRequest("No data provided");

        _context.Consumers.AddRange(consumers);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Multiple records inserted successfully",
            Count = consumers.Count
        });
    }


    // ✅ PUT: api/consumer/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Consumer consumer)
    {
        if (id != consumer.Id)
            return BadRequest();

        _context.Entry(consumer).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ✅ DELETE: api/consumer/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var consumer = await _context.Consumers.FindAsync(id);
        if (consumer == null)
            return NotFound();

        _context.Consumers.Remove(consumer);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
