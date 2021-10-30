using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entites;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper;

namespace API.Controllers
{
  public class AccountController : BaseApiController
  {
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
    {
      _mapper = mapper;
      _tokenService = tokenService;

      _context = context;

    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto register)
    {
      if (await IsExistUser(register.Username))
      {
        return BadRequest("Not Create");
      }
      var user= _mapper.Map<AppUser>(register);
      using var hmac = new HMACSHA512();
      user.UserName = register.Username;
      user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password));
      user.PasswordSalt = hmac.Key;
      

      _context.Users.Add(user);
      await _context.SaveChangesAsync();
      return new UserDto
      {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user),
        KnownAs = user.KnownAs,
        Gender = user.Gender
      };

    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
      var user = await _context.Users.Include(x=>x.Photos)
      .SingleOrDefaultAsync(a => a.UserName == loginDto.UserName);

      if (user == null) return Unauthorized("User not found.");

      using var hmac = new HMACSHA512(user.PasswordSalt);

      var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

      for (int i = 0; i < computeHash.Length; i++)
      {
        if (computeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
      }

       return new UserDto
      {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user),
        PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
        KnownAs = user.KnownAs,
        Gender = user.Gender

      };
    }
    private async Task<bool> IsExistUser(string userName)
    {
      return await _context.Users.AnyAsync(a => a.UserName == userName);
    }
  }
}