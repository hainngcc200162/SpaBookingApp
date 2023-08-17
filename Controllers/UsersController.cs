using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetUsers(int? page, string searchByName, string searchByEmail)
        {
            if (page == null || page < 1)
            {
                page = 1;
            }

            int pageSize = 5;
            int totalPages = 0;

            var query = _context.Users.AsQueryable();

            // Lọc theo tên người dùng
            if (!string.IsNullOrEmpty(searchByName))
            {
                query = query.Where(u => u.FirstName.Contains(searchByName) || u.LastName.Contains(searchByName));
            }

            // Lọc theo email
            if (!string.IsNullOrEmpty(searchByEmail))
            {
                query = query.Where(u => u.Email.Contains(searchByEmail));
            }

            decimal count = query.Count();
            totalPages = (int)Math.Ceiling(count / pageSize);

            var users = query
                .OrderByDescending(u => u.Id)
                .Skip((int)(page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            List<UserProfileDto> userProfiles = new List<UserProfileDto>();
            foreach (var user in users)
            {
                var userProfileDto = new UserProfileDto()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                };

                userProfiles.Add(userProfileDto);
            }

            var response = new
            {
                Users = userProfiles,
                TotalPages = totalPages,
                PageSize = pageSize,
                Page = page
            };

            return Ok(response);
        }



        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            var userProfileDto = new UserProfileDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            return Ok(userProfileDto);
        }

        // ...

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok(new { Message = "User deleted successfully." });
        }
    }
}
