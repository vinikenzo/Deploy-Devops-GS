using ecoorbit_dotnet.Application.DTOs.FireDetectionResult;
using ecoorbit_dotnet.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecoorbit_dotnet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FireDetectionResultsController : ControllerBase
{
    private readonly IFireDetectionResultService _service;

    public FireDetectionResultsController(IFireDetectionResultService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) =>
        Ok(await _service.GetByIdAsync(id));

    [HttpGet("image/{satelliteImageId:guid}")]
    public async Task<IActionResult> GetByImage(Guid satelliteImageId) =>
        Ok(await _service.GetBySatelliteImageIdAsync(satelliteImageId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFireDetectionResultDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFireDetectionResultDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
