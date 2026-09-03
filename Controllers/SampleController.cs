using Microsoft.AspNetCore.Mvc;

namespace InsureApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "API is working!", timestamp = DateTime.Now });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(new { id, message = $"Retrieved item {id}" });
    }

    [HttpPost]
    public IActionResult Create([FromBody] dynamic data)
    {
        return CreatedAtAction(nameof(GetById), new { id = 1 }, new { id = 1, data });
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] dynamic data)
    {
        return Ok(new { id, message = "Updated successfully", data });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return NoContent();
    }
}
