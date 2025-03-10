
using MedicineServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using System.Linq;
using MedicineServer.DTO;
using Microsoft.IdentityModel.Tokens;

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
        //[HttpPost("deleteuser")]
        //public async Task<IActionResult> DeleteUser(int id)
        //{
            
        //    var user = await context.Users.FindAsync(id);

        //    if (user == null)
        //    {
        //        return NotFound(new { message = "User not found." });
        //    }

        //    if (user.UserRank == 1)
        //    {
        //        return BadRequest(new { message = "Admins cannot be deleted." });
        //    }
        //    user.Active = false;
        //    context.Users.FirstOrDefault(u => u.UserId == id) = user;
        //    await context.SaveChangesAsync();

        //    return Ok(new { message = "User deleted successfully." });
        //}
        [HttpGet("getuserbyusername")]
        public IActionResult GetUserByUsername([FromQuery] string username)
        {
            try
            {
                var tempuser = context.Users.FirstOrDefault(u => u.UserName == username);
                if (tempuser == null)
                {
                    return NotFound(new { Message = "User not found." });
                }
                AppUser user = new AppUser(tempuser);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving the user.", Error = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] DTO.LoginInfo loginDto)
        {
            try
            {
                HttpContext.Session.Clear(); 

             
                Models.User? modelsUser =  context.Users.ToList().First(x=>x.UserName==loginDto.username);

                
                if (modelsUser == null || modelsUser.UserPass != loginDto.password)
                {
                    return Unauthorized();
                }
                HttpContext.Session.SetString("loggedInUser", modelsUser.UserId.ToString());
                AppUser dtoUser = new AppUser(modelsUser);
               
                return Ok(dtoUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("getusers")]
        public IActionResult GetUsers()
        {
            try
            {
                List<User> list = new List<User>();
                
                foreach (var user in context.Users)
                {
                    AppUser temp = new AppUser(user);
                    list.Add(user);

                }
                if (list.IsNullOrEmpty())
                {
                    return NotFound(new { Message = "No users exist." });
                }
                return Ok(list);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving Users.", Error = ex.Message });
            }
        }
        [HttpGet("getmedicinesbypharma")]
        public IActionResult GetMedicinesByPharmacy([FromQuery] int PharmacyId)
        {
            try
            {
                List<Models.Medicine> Tlist = context.Medicines.Where(x=>x.PharmacyId==PharmacyId).ToList<Models.Medicine>();
                if (Tlist == null)
                {
                    return NotFound(new { Message = "You have no medicines." });
                }
                List<MedicineDTO> list = new List<MedicineDTO>();
                foreach(Models.Medicine x in Tlist)
                {
                    MedicineDTO temp = new MedicineDTO(x);
                    list.Add(temp);
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving the medicines.", Error = ex.Message });
            }
        }
        [HttpGet("getmedicines")]
        public IActionResult GetMedicines()
        {
            try
            {
                List<MedicineDTO> list = new List<MedicineDTO>();
                foreach(var m in  context.Medicines)
                {
                    MedicineDTO temp = new MedicineDTO(m);
                    list.Add(temp);
                }
                if (list.IsNullOrEmpty())
                    return NotFound(new { Message = "No medicines exist." });
                return Ok(list);
            }
            
            catch(Exception ex)
            { 
                return StatusCode(500, new { Message = "An error occurred while retrieving medicines.", error = ex.Message }); 
            }
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] DTO.AppUser userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (context.Users.Any(u => u.UserName == userDto.UserName || u.Email == userDto.Email))
                {
                    return BadRequest("User with the same username or email already exists.");
                }

                Models.User modelsUser = new User
                {
                    UserName = userDto.UserName,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    UserPass = userDto.UserPassword,
                    UserRank = userDto.Rank,
                    UserId = userDto.Id
                };

                context.Users.Add(modelsUser);
                Console.WriteLine($"User Email: {userDto.Email}");
                if (userDto.Email.Length > 255)
                {
                    throw new ArgumentException("Email exceeds the maximum allowed length of 255 characters.");
                }
                Console.WriteLine($"User Email: {userDto.Email} (Length: {userDto.Email.Length})");
                try
                {

                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Error: {ex.InnerException?.Message}");
                    Console.WriteLine($"Error: {ex.ToString()}");
                    throw; 
                }

                return Ok(new DTO.AppUser(modelsUser));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during registration: {ex.Message}");
                return BadRequest($"Error during registration: {ex.Message}");
            }
        }


    }
}
