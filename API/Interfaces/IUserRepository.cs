using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entites;

namespace API.Interfaces
{
    public interface IUserRepository
    {
         void Update(AppUser user);
         Task<bool> SaveAllAsync();
         Task<IEnumerable<AppUser>> GetUsersAsync();

         Task<AppUser> GetUserByIdAsync(int id);
         Task<AppUser> GetUserByUserNameAsync(string username);

         Task<MemberDto> GetMemberAsync(string username);
         Task<IEnumerable<MemberDto>> GetMembersAsync();

         
    }
}