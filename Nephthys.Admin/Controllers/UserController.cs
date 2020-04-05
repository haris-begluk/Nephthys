using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nephthys.Admin.Data.Entities;

namespace Nephthys.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<ApplicationUser> GetUser(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            return await _userManager.Users.Include(u => u.Claims).FirstOrDefaultAsync(u => u.Email.Equals(email)); ;
        }
        public async Task<ApplicationUser> GetUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            return await _signInManager.valida
        }
    }
}