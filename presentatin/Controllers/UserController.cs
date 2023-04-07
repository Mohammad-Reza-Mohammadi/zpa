using Data.Contracts;
using Data.Repositories;
using ECommerce.Utility;
using Entities.User.Owned;
using Entities.User.UserProprety.EnumProperty;
using Entities.Useres;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using presentation.Models;
using System.Drawing;
using System.Security.Claims;
using Utility.SwaggerConfig.Permissions;
using WebFramework.Api;
using static Utility.SwaggerConfig.Permissions.Permissions;
using User = Entities.Useres.User;

namespace presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }



        [PermissionAuthorize(Permissions.User.GetAll,Admin.admin)]
        [HttpGet]
        public async Task<ApiResult<List<User>>> GetAll(CancellationToken cancellationToken)
        {
            var users = await _userRepository.TableNoTracking.ToListAsync(cancellationToken);
            //return users; 
            return new ApiResult<List<User>>
            {
                IsSuccess = true,
                StatusCode = ApiResultStatusCode.Success,
                Message = "عملیات با موفقیت انجام شد",
                Data = users,
            };
        }

        [PermissionAuthorize(Permissions.User.GetUserById, Admin.admin)]
        [HttpGet("{id:int}")]
        public async Task<ApiResult<User>> GetUserById(int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(cancellationToken, id);
            //if (user == null)
            //    return NotFound();
       //     return user;

            return new ApiResult<User>
            {
                IsSuccess = true,
                StatusCode = ApiResultStatusCode.Success,
                Message = "عملیات با موفقیت انجام شد",
                Data = user
            };
        }











        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> SuignUp([FromForm] SignupUserDto signupUserDto, CancellationToken cancellationToken)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int currentUserId1 = Convert.ToInt32(currentUserId);

            var exists = await _userRepository.TableNoTracking.AnyAsync(p => p.UserName == signupUserDto.UserName);

            if (exists)
            {
                return Content("نام کاربری تکراری است");
            }

            var user = new User()
            {
                UserName = signupUserDto.UserName,
                UserPhoneNumber = signupUserDto.phoneNumber,
                UserAge = signupUserDto.Age,
                UserGender = signupUserDto.Gender,
                UserRole = signupUserDto.Role,
            };
            var parentEployeeId = signupUserDto.ParetnEmployeeId;
            if (parentEployeeId == null)
            {
                await _userRepository.AddUserAsync(user, signupUserDto.Password, cancellationToken);
            }
            else
            {
                user.UserParetnEmployeeId = signupUserDto.ParetnEmployeeId;
                await _userRepository.AddUserAsync(user, signupUserDto.Password, cancellationToken);
            }


            return Content($"{signupUserDto.UserName} : با موفقیت اضافه شد ");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<User>> Login(string FirstName, string password, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Login(FirstName, password, cancellationToken);
            if (user == null)
                return Content($"{FirstName} یافت نشد");
            return user;
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<ActionResult> Update([FromForm] UpdateUserDto user, CancellationToken cancellationToken)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId1 = Convert.ToInt32(userId);

            var exists = await _userRepository.TableNoTracking.AnyAsync(p => p.UserName == user.UserName);

            if (exists)
            {
                return Content("نام کاربری تکراری است");
            }


            
            var getUser = await _userRepository.GetByIdAsync(cancellationToken, userId1);

            #region update user

            getUser.UserName = user.UserName;
            getUser.UserAge = user.Age;
            getUser.UserPhoneNumber = user.PhoneNumber;
            getUser.UserEmail = user.Email;
            // ElementAt(i)
            getUser.UserAddresses.ElementAt(0).UserAddressTitle = user.AddressTitle;
            getUser.UserAddresses.ElementAt(0).UserAddressCity = user.City;
            getUser.UserAddresses.ElementAt(0).UserAddressTown = user.Town;
            getUser.UserAddresses.ElementAt(0).UserAddressStreet = user.Street;
            getUser.UserAddresses.ElementAt(0).UserAddressPostalCode = user.PostalCode;

            #endregion

            await _userRepository.UpdateUserAsync(getUser, userId1, user.ProductImage, cancellationToken);


            return Ok();
        }

        [PermissionAuthorize(Permissions.User.Delete, Admin.admin)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(cancellationToken, id);
            user.UserIsActive = false;
            await _userRepository.UpdateAsync(user, cancellationToken);

            return Ok();
        }

        [PermissionAuthorize(Permissions.User.AddSoperviserPermissionById, Admin.admin)]
        [HttpPost]
        public async Task<ActionResult> AddSoperviserPermissionById(int Id, CancellationToken cancellationToken)
        {

            bool Result = await _userRepository.ChangePermissinByID(Id, cancellationToken);

            if (Result == false)
            {
                return Content("شما قادر به تغییر مجوز های کاربر وارد شده نمی باشید");
            }
            return Content("مجوز با موفقیت اضافه شد");

        }

        [PermissionAuthorize(Permissions.User.AddAllSoperviserPermission, Admin.admin)]
        [HttpPost]
        public async Task<ActionResult> AddAllSoperviserPermission(CancellationToken cancellationToken)
        {
            await _userRepository.AllSupervisorChangePermissin(cancellationToken);
            return Content("عملیات با موفقیت انجام شد");
        }


    }
}
