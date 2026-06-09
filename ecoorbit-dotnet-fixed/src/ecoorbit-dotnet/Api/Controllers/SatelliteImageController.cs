using System.Security.Claims;
using ecoorbit_dotnet.Application.DTOs.SatelliteImage;
using ecoorbit_dotnet.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecoorbit_dotnet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SatelliteImagesController : ControllerBase
{
    private readonly ISatelliteImageService _service;

    public SatelliteImagesController(ISatelliteImageService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) =>
        Ok(await _service.GetByIdAsync(id));

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUser(Guid userId) =>
        Ok(await _service.GetByUserIdAsync(userId));

    [HttpGet("my")]
    public async Task<IActionResult> GetMine()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _service.GetByUserIdAsync(userId));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSatelliteImageDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSatelliteImageDto dto)
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
