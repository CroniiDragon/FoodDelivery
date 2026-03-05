using FoodDelivery.Shared.DTOs;
using FoodDelivery.UserService.DTOs;
using FoodDelivery.UserService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.UserService.Controllers;

// SRP: Responsabilitate unica - primeste requesturi HTTP si returneaza raspunsuri
// DIP: Depinde de IUserService, nu de implementarea concreta
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET api/users
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<UserResponseDto>>.Ok(users));
    }

    // GET api/users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound(ApiResponse<UserResponseDto>.Fail($"Utilizatorul cu id={id} nu exista."));
        return Ok(ApiResponse<UserResponseDto>.Ok(user));
    }

    // POST api/users/customers
    [HttpPost("customers")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> CreateCustomer(
        [FromBody] CreateCustomerDto dto)
    {
        var created = await _userService.CreateCustomerAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<UserResponseDto>.Ok(created, "Client creat cu succes."));
    }

    // POST api/users/couriers
    [HttpPost("couriers")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> CreateCourier(
        [FromBody] CreateCourierDto dto)
    {
        var created = await _userService.CreateCourierAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<UserResponseDto>.Ok(created, "Curier creat cu succes."));
    }

    // POST api/users/login
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> Login(
        [FromBody] LoginDto dto)
    {
        var user = await _userService.LoginAsync(dto);
        if (user == null)
            return Unauthorized(ApiResponse<UserResponseDto>.Fail("Email sau parola incorecta."));
        return Ok(ApiResponse<UserResponseDto>.Ok(user, "Autentificare reusita."));
    }

    // DELETE api/users/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var result = await _userService.DeleteAsync(id);
        if (!result)
            return NotFound(ApiResponse<bool>.Fail($"Utilizatorul cu id={id} nu exista."));
        return Ok(ApiResponse<bool>.Ok(true, "Utilizator sters cu succes."));
    }
}
