﻿
using MedicineServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MedicineServer.DTO;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Data.SqlClient;

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
        [HttpPost("enableuser")]
        public async Task<IActionResult> EnableUser(int id)
        {
            var user = await context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            if (user.UserRank == 1)
            {
                return BadRequest(new { message = "Admins cannot be disabled." });
            }
            user.Active = !user.Active;
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicine(int id, [FromBody] MedicineDTO updatedMedicine)
        {
            if (id != updatedMedicine.MedicineId)
            {
                return BadRequest("Mismatched ID");
            }

            var existingMedicine = await context.Medicines.Include( m => m.Status).FirstOrDefaultAsync(m => m.MedicineId == id);

            if (existingMedicine == null)
            {
                return NotFound();
            }

            existingMedicine.NeedsPrescription = updatedMedicine.NeedsPrescription;
            existingMedicine.Status.Mstatus = updatedMedicine.Status.Mstatus;
            existingMedicine.Status.Notes = updatedMedicine.Status.Notes;

            await context.SaveChangesAsync();

            return Ok();
        }
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

             
                Models.User? modelsUser =  context.Users.ToList().FirstOrDefault(x=>x.UserName==loginDto.username);

                
                if (modelsUser == null || modelsUser.UserPassword != loginDto.password||!modelsUser.Active)
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
                    UserPassword = userDto.UserPassword,
                    UserRank = userDto.Rank,
                    UserId = userDto.Id,
                    Active = true
                    
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
        [HttpGet("isusernametaken/{username}")]
        public async Task<IActionResult> IsUsernameTaken(string username)
        {
            var exists = await context.Users.AnyAsync(u => u.UserName == username);
            return Ok(exists);
        }
        [HttpPut("updateuser")]
        public async Task<IActionResult> UpdateUser([FromBody] AppUser updatedUser)
        {
            var user = await context.Users.FindAsync(updatedUser.Id);
            if (user == null)
                return NotFound();

            user.UserName = updatedUser.UserName;
            user.UserPassword = updatedUser.UserPassword;


            await context.SaveChangesAsync();
            return Ok();
        }

        #region Backup / Restore
        [HttpGet("Backup")]
        public async Task<IActionResult> Backup()
        {
            string path = $"{this.webHostEnvironment.WebRootPath}\\..\\DBScripts\\backup.bak";
            try
            {
                System.IO.File.Delete(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            bool success = await BackupDatabaseAsync(path);
            if (success)
            {
                return Ok("Backup was successful");
            }
            else
            {
                return BadRequest("Backup failed");
            }
        }

        [HttpGet("Restore")]
        public async Task<IActionResult> Restore()
        {
            string path = $"{this.webHostEnvironment.WebRootPath}\\..\\DBScripts\\backup.bak";

            bool success = await RestoreDatabaseAsync(path);
            if (success)
            {
                return Ok("Restore was successful");
            }
            else
            {
                return BadRequest("Restore failed");
            }
        }
        //this function backup the database to a specified path
        private async Task<bool> BackupDatabaseAsync(string path)
        {
            try
            {

                //Get the connection string
                string? connectionString = context.Database.GetConnectionString();
                //Get the database name
                string databaseName = context.Database.GetDbConnection().Database;
                //Build the backup command
                string command = $"BACKUP DATABASE {databaseName} TO DISK = '{path}'";
                //Create a connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //Open the connection
                    await connection.OpenAsync();
                    //Create a command
                    using (SqlCommand sqlCommand = new SqlCommand(command, connection))
                    {
                        //Execute the command
                        await sqlCommand.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        //THis function restore the database from a backup in a certain path
        private async Task<bool> RestoreDatabaseAsync(string path)
        {
            try
            {
                //Get the connection string
                string? connectionString = context.Database.GetConnectionString();
                //Get the database name
                string databaseName = context.Database.GetDbConnection().Database;
                //Build the restore command
                string command = $@"
               USE master;
               DECLARE @latestBackupSet INT;
               SELECT TOP 1 @latestBackupSet = position
               FROM msdb.dbo.backupset
               WHERE database_name = '{databaseName}'
               AND backup_set_id IN (
                     SELECT backup_set_id
                     FROM msdb.dbo.backupmediafamily
                     WHERE physical_device_name = '{path}'
                 )
               ORDER BY backup_start_date DESC;
                ALTER DATABASE {databaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                RESTORE DATABASE {databaseName} FROM DISK = '{path}' 
                WITH FILE=@latestBackupSet,
                REPLACE;
                ALTER DATABASE {databaseName} SET MULTI_USER;";

                //Create a connection to the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //Open the connection
                    await connection.OpenAsync();
                    //Create a command
                    using (SqlCommand sqlCommand = new SqlCommand(command, connection))
                    {
                        //Execute the command
                        await sqlCommand.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion
    }
}
