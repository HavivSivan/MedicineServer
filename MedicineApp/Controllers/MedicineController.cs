
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

        public MedicineController(MedicineDbContext context, IWebHostEnvironment env)
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
        [HttpPost("UpdateMedicine")]
        public async Task<IActionResult> UpdateMedicine([FromBody] MedicineDTO updatedMedicine)
        {
            var existingMedicine = await context.Medicines.Include(m => m.Status).FirstOrDefaultAsync(m => m.MedicineId == updatedMedicine.MedicineId);

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
        public async Task<IActionResult> GetUserByUsername([FromQuery] string username)
        {
            var user = await context.Users
                .Where(u => u.UserName == username)
                .Select(u => new AppUser
                {
                    Id           = u.UserId,
                    UserName     = u.UserName,
                    UserPassword = u.UserPassword,
                    Email        = u.Email,
                    FirstName    = u.FirstName,
                    LastName     = u.LastName,
                    Rank     = (int)u.UserRank,
                    Active       = u.Active
                })
                .FirstOrDefaultAsync();

            if (user is null)
                return NotFound();

            return Ok(user);
        }
    

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginInfo loginDto)
        {
            try
            {
                HttpContext.Session.Clear();


                Models.User? modelsUser = context.Users.ToList().FirstOrDefault(x => x.UserName==loginDto.username);


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
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving Users.", Error = ex.Message });
            }
        }
        [HttpGet("getmedicinesbypharma")]
        public IActionResult GetMedicinesByPharmacy([FromQuery] int PharmacyId)
        {
            try
            {
                var medicines = context.Medicines
                    .Include(m => m.Status)
                    .Include(m => m.Pharmacy)
                    .Include(m => m.User)
                    .Where(m => m.PharmacyId == PharmacyId)   
                    .ToList();

                if (medicines == null || !medicines.Any())
                    return NotFound(new { Message = "No medicines for this pharmacy." });

                var dtoList = medicines
                    .Select(m => new MedicineDTO(m))
                    .ToList();

                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while retrieving medicines.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("getmedicines")]
        public IActionResult GetMedicines()
        {
            try
            {
                var medicines = context.Medicines.Include(m => m.Status).Include(m => m.Pharmacy).Include(m => m.User).Include(m=>m.Pharmacy.User).ToList();

                if (medicines == null || !medicines.Any())
                {
                    return NotFound(new { Message = "No medicines exist." });
                }

                var dtoList = medicines.Select(m => new MedicineDTO(m)).ToList();

                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while retrieving medicines.",
                    error = ex.Message
                });
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
        [HttpGet("is-username-taken/{username}")]
        public async Task<IActionResult> IsUsernameTaken(string username)
        {
            var exists = context.Users.Any(u => u.UserName == username);
            return Ok(exists);
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] AppUser updatedUser)
        {
            var user = await context.Users.FindAsync(updatedUser.Id);
            if (user == null) return NotFound();
            if (user.UserPassword==updatedUser.UserPassword)
            {
                user.UserName = updatedUser.UserName;
                await context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
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
        [HttpPost("Order")]
        public IActionResult Order(CreateOrderDTO order)
        {
            if (order==null||context.Users.FirstOrDefault(x=>x.UserId==order.UserId)==null||context.Medicines.FirstOrDefault(x=>x.MedicineId==order.MedicineId)==null)
            {
                return BadRequest("Invalid order data.");
            }

            var medicine = context.Medicines.FirstOrDefault(x => x.MedicineId == order.MedicineId);
            var user = context.Users.FirstOrDefault(x => x.UserId == order.UserId);

            if (medicine == null || user == null)
            {
                return BadRequest("Medicine or user does not exist.");
            }

            var newOrder = new Order
            {
                MedicineId = medicine.MedicineId,
                UserId = user.UserId,
                PrescriptionImage = order.PrescriptionImage,
                OStatus = order.OStatus
            };
            var medicineStatus = context.MedicineStatuses.FirstOrDefault(x => x.StatusId == medicine.StatusId);
            if (medicineStatus!=null)
            {
                medicineStatus.Mstatus = "Ordered";
                context.MedicineStatuses.Update(medicineStatus);
            }
            context.Orders.Add(newOrder);
            context.SaveChanges();

            return Ok();
        }

        [HttpPost("UpdateOrderStatus")]
        public IActionResult UpdateOrderStatus([FromBody] CreateOrderDTO updatedOrder, int Orderid)
        {
            var order = context.Orders.FirstOrDefault(o => o.OrderId == Orderid);
            if (order == null) return NotFound();

            order.OStatus = updatedOrder.OStatus;
            if (updatedOrder.OStatus=="Approved")
            {
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (updatedOrder.OStatus == "Denied")
            {
                int id = context.Medicines.FirstOrDefault(x => x.MedicineId==updatedOrder.MedicineId).StatusId;
                context.MedicineStatuses.FirstOrDefault(x => x.StatusId==id).Mstatus="Approved";
            }
            context.SaveChanges();
            return Ok();
        }
        [HttpPost("UploadPrescriptionImage")]
        public IActionResult UploadPrescriptionImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");
            var uploads = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);
            var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploads, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            var relative = Path.Combine("uploads", uniqueName).Replace("\\", "/");
            return Ok(relative);
        }
        [HttpGet("GetOrdersList")]
        public async Task<IActionResult> GetOrdersList()
        {
            try
            {
                var orders = await context.Orders.Include(o => o.Medicine).Include(o => o.User).ToListAsync();
                if (orders == null || !orders.Any())
                {
                    return NotFound(new { Message = "No orders found." });
                }
                List<OrderDTO> orderDtos = orders.Select(o => new OrderDTO(o)).ToList();
                return Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving orders.", Error = ex.Message });
            }
        }
        [HttpPost("AddMedicine")]
        public async Task<IActionResult> AddMedicine([FromBody] MedicineCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Payload is null.");
            MedicineStatus newstatus= new Models.MedicineStatus
            {
                Mstatus = "Checking",
                Notes   = "New medicine added.",
                
            };
            try
            {
                context.MedicineStatuses.Add(newstatus);
                context.SaveChanges();
            }
            catch(DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
            var newMed = new Models.Medicine
            {
                MedicineName      = dto.MedicineName,
                BrandName         = dto.BrandName,
                NeedsPrescription = false,
                PharmacyId        = dto.PharmacyId,
                StatusId          = newstatus.StatusId,
                UserId            = context.Users.FirstOrDefault(x=>x.UserName==dto.Username).UserId
            };
            context.Medicines.Add(newMed);
            
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
            return Ok();
        }
        [HttpPost("AddPharmacy")]
        public async Task<IActionResult> AddPharmacy([FromBody] PharmacyCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Payload is null.");

            var user = await context.Users.FindAsync(dto.Username);
            if (user == null || user.UserRank != 3)
                return BadRequest("Invalid user");

            user.UserRank = 2;

            var newPharmacy = new Pharmacy
            {
                PharmacyName = dto.Name,
                Adress       = dto.Address,
                Phone        = dto.Phone,
                UserId       = user.UserId
            };

            context.Pharmacies.Add(newPharmacy);
            await context.SaveChangesAsync();

            return Ok(new { PharmacyId = newPharmacy.PharmacyId });
        }
        [HttpGet("GetPharmacies")]
        public async Task<List<PharmacyDTO>> GetPharmacies()
        {
            try
            {
                var pharmacies = await context.Pharmacies
                                             .Include(p => p.User)
                                             .ToListAsync();

                var dtos = pharmacies
                           .Select(p => new PharmacyDTO(p))
                           .ToList();

                return dtos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving pharmacies: {ex.Message}");
                return new List<PharmacyDTO>();
            }
        }
        public class MedicineCreateDTO
        {
            public string MedicineName { get; set; }
            public string BrandName { get; set; }
            public int PharmacyId { get; set; }
            public string Username { get; set; }
        }
        public class PharmacyCreateDTO
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            public int Username { get; set; }
        }
        public class LoginInfo
        {
            public string username { get; set; }
            public string password { get; set; }
        }
        public class CreateOrderDTO
        {
            public int MedicineId { get; set; }
            public int UserId { get; set; }
            public string? PrescriptionImage { get; set; }
            public string OStatus { get; set; }
        }

    }
}
