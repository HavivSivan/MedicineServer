
using MedicineServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicineServer.Controllers
{
    [Route("api")]
    [ApiController]
    public class MedicineController : ControllerBase
    {

        private MedicineDbContext context;

        private IWebHostEnvironment webHostEnvironment;

        public  MedicineController(MedicineDbContext context, IWebHostEnvironment env)
        {
            this.context = context;
            this.webHostEnvironment = env;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] DTO.LoginInfo loginDto)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                //Get model user class from DB with matching email. 
                Models.User? modelsUser =  context.Users.ToList().First(x=>x.UserName==loginDto.username);

                //Check if user exist for this email and if password match, if not return Access Denied (Error 403) 
                if (modelsUser == null || modelsUser.UserPass != loginDto.password)
                {
                    return Unauthorized();
                }

                //Login suceed! now mark login in session memory!
                HttpContext.Session.SetString("loggedInUser", modelsUser.UserId.ToString());

                DTO.AppUser dtoUser = new DTO.AppUser(modelsUser);
               
                return Ok(dtoUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] DTO.AppUser userDto)
        {
            try
            {
                HttpContext.Session.Clear(); //Logout any previous login attempt

                //Get model user class from DB with matching email. 
                Models.User modelsUser = new User()
                {
                    UserName = userDto.UserName,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Email = userDto.UserEmail,
                    UserPass = userDto.UserPassword,
                    UserRank = userDto.Rank,
                    UserId=userDto.Id
                };

                context.Users.Add(modelsUser);
                context.SaveChanges();

                //User was added!
                DTO.AppUser dtoUser = new DTO.AppUser(modelsUser);
                
                return Ok(dtoUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


    }
}
