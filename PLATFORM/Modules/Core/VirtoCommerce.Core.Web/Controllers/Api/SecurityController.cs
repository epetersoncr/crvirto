﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Omu.ValueInjecter;
using VirtoCommerce.CoreModule.Web.Converters;
using VirtoCommerce.CoreModule.Web.Security;
using VirtoCommerce.CoreModule.Web.Security.Models;
using VirtoCommerce.Foundation.Data.Security.Identity;
using VirtoCommerce.Foundation.Frameworks.Extensions;
using VirtoCommerce.Foundation.Security.Model;
using VirtoCommerce.Framework.Web.Security;
using ApiAccount = VirtoCommerce.Foundation.Security.Model.ApiAccount;

namespace VirtoCommerce.CoreModule.Web.Controllers.Api
{
    [RoutePrefix("api/security")]
    public class SecurityController : ApiController
    {
        private readonly Func<IFoundationSecurityRepository> _securityRepository;
        private readonly Func<IAuthenticationManager> _authenticationManagerFactory;
        private readonly Func<ApplicationSignInManager> _signInManagerFactory;
        private readonly Func<ApplicationUserManager> _userManagerFactory;
        private readonly IApiAccountProvider _apiAccountProvider;
        private readonly IPermissionService _permissionService;
        private readonly IRoleManagementService _roleService;

        public SecurityController(Func<IFoundationSecurityRepository> securityRepository, Func<ApplicationSignInManager> signInManagerFactory,
                                  Func<ApplicationUserManager> userManagerFactory, Func<IAuthenticationManager> authManagerFactory,
                                  IApiAccountProvider apiAccountProvider,
            IPermissionService permissionService, IRoleManagementService roleService)
        {
            _securityRepository = securityRepository;
            _signInManagerFactory = signInManagerFactory;
            _userManagerFactory = userManagerFactory;
            _authenticationManagerFactory = authManagerFactory;
            _apiAccountProvider = apiAccountProvider;
            _permissionService = permissionService;
            _roleService = roleService;
        }

        private ApplicationUserManager _userManager;
        private ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    _userManager = _userManagerFactory();
                }
                return _userManager;
            }
        }

        #region Internal Web admin actions

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Login(UserLogin model)
        {
            if (await _signInManagerFactory().PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true) == SignInStatus.Success)
            {
                return Ok(await GetUserExtended(model.UserName));
            }

            return StatusCode(HttpStatusCode.Unauthorized);
        }

        [HttpGet]
        [Route("usersession")]
        [ResponseType(typeof(ApplicationUserExtended))]
        public async Task<IHttpActionResult> GetCurrentUserSession()
        {
            return Ok(await GetUserExtended(User.Identity.Name));
        }

        [HttpPost]
        [Route("logout")]
        public IHttpActionResult Logout()
        {
            _authenticationManagerFactory().SignOut();
            return Ok(new { status = true });
        }

        [HttpGet]
        [Route("permissions")]
        [ResponseType(typeof(PermissionDescriptor[]))]
        public IHttpActionResult GetPermissions()
        {
            var result = _permissionService.GetAllPermissions()
                .OrderBy(p => p.Name)
                .ToArray();

            return Ok(result);
        }

        [HttpGet]
        [Route("roles")]
        [ResponseType(typeof(RoleSearchResponse))]
        public IHttpActionResult SearchRoles([FromUri]RoleSearchRequest request)
        {
            var result = _roleService.SearchRoles(request);
            return Ok(result);
        }

        [HttpGet]
        [Route("roles/{roleId}")]
        [ResponseType(typeof(RoleDescriptor))]
        public IHttpActionResult GetRole(string roleId)
        {
            var result = _roleService.GetRole(roleId);
            return Ok(result);
        }

        /// <summary>
        /// DELETE: api/security/roles?id=1&amp;id=2
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("roles")]
        [ResponseType(typeof(RoleDescriptor))]
        public IHttpActionResult DeleteRoles([FromUri(Name = "ids")] string[] roleIds)
        {
            if (roleIds != null)
            {
                foreach (var roleId in roleIds)
                {
                    _roleService.DeleteRole(roleId);
                }
            }

            return Ok();
        }

        [HttpPut]
        [Route("roles")]
        [ResponseType(typeof(RoleDescriptor))]
        public IHttpActionResult UpdateRole(RoleDescriptor role)
        {
            var result = _roleService.AddOrUpdateRole(role);
            return Ok(result);
        }

        #endregion

        #region Public methods

        [HttpGet]
        [Route("usersession/{userName}")]
        public async Task<ApplicationUserExtended> GetUserSession(string userName)
        {
            return await GetUserExtended(userName);
        }

        #region Methods needed to integrate identity security with external user store that will call api methods

        #region IUserStore<ApplicationUser> Members

        [Route("users/id/{userId}")]
        [HttpGet]
        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return await UserManager.FindByIdAsync(userId);
        }

        [Route("users/name/{userName}")]
        [HttpGet]
        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return await UserManager.FindByNameAsync(userName);
        }

        [Route("users/email/{email}")]
        [HttpGet]
        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await UserManager.FindByEmailAsync(email);
        }

        #endregion

        /// <summary>
        ///  GET: api/security/apiaccounts/new
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(Security.Models.ApiAccount))]
        [Route("apiaccounts/new")]
        public IHttpActionResult GenerateNewApiAccount()
        {
            var apiAccount = _apiAccountProvider.GenerateApiCredentials();
            apiAccount.IsActive = true;
            var result = apiAccount.ToWebModel();
            return Ok(result);
        }


        /// <summary>
        ///  GET: api/security/users/jo@domain.com
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ApplicationUserExtended))]
        [Route("users/{name}")]
        public async Task<IHttpActionResult> GetUserByName(string name)
        {
            var retVal = await GetUserExtended(name);
            return Ok(retVal);
        }

        /// <summary>
        /// POST: api/security/users/jo@domain.com/changepassword
        /// </summary>
        /// <param name="name"></param>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(IdentityResult))]
        [Route("users/{name}/changepassword")]
        public async Task<IHttpActionResult> ChangePassword(string name, [FromBody] ChangePasswordInfo changePassword)
        {
            var user = await GetUserExtended(name);
            if (user == null)
            {
                return NotFound();
            }
            var retVal = await UserManager.ChangePasswordAsync(user.Id, changePassword.OldPassword, changePassword.NewPassword);
            return Ok(retVal);
        }

        [HttpGet]
        [ResponseType(typeof(IdentityResult))]
        [Route("users")]
        public async Task<IHttpActionResult> GetUserByLogin(string loginProvider, string providerKey)
        {
            var retVal = await GetUserExtended(loginProvider, providerKey);

            return Ok(retVal);
        }

        /// <summary>
        ///  GET: api/security/users?q=ddd&amp;start=0&amp;count=20
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(UserSearchResult))]
        [Route("users")]
        public async Task<IHttpActionResult> SearchUsersAsync([ModelBinder(typeof(UserSearchCriteriaBinder))] UserSearchCriteria criteria)
        {
            var query = UserManager.Users;
            var retVal = new UserSearchResult
            {
                TotalCount = query.Count(),
                Users = new List<ApplicationUserExtended>()
            };

            var result = query.OrderBy(x => x.UserName)
                             .Skip(criteria.Start)
                             .Take(criteria.Count)
                             .ToArray();

            foreach (var user in result)
            {
                var userExt = await GetUserExtended(user.UserName);
                retVal.Users.Add(userExt);
            }

            return Ok(retVal);
        }

        /// <summary>
        /// DELETE: api/security/users?names=21
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("users")]
        public async Task<IHttpActionResult> DeleteAsync([FromUri] string[] names)
        {
            foreach (var name in names)
            {
                var dbUser = await UserManager.FindByNameAsync(name);
                if (dbUser == null)
                {
                    return NotFound();
                }
                await UserManager.DeleteAsync(dbUser);
                using (var repository = _securityRepository())
                {
                    var account = repository.GetAccountByName(name);
                    if (account != null)
                    {
                        repository.Remove(account);
                        repository.UnitOfWork.Commit();
                    }
                }
            }
            return Ok();
        }

        /// <summary>
        /// PUT: api/security/users
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("users")]
        public async Task<IHttpActionResult> UpdateAsync(ApplicationUserExtended user)
        {
            var dbUser = await UserManager.FindByIdAsync(user.Id);

            dbUser.InjectFrom(user);


            var result = await UserManager.UpdateAsync(dbUser);

            if (result.Succeeded)
            {
                using (var repository = _securityRepository())
                {
                    var acount = repository.GetAccountByName(user.UserName);
                    if (acount == null)
                    {
                        return BadRequest("Acount not found");
                    }
                    acount.RegisterType = (int)user.UserType;
                    acount.AccountState = (int)user.UserState;
                    acount.MemberId = user.MemberId;
                    acount.StoreId = user.StoreId;
                    if (user.ApiAcounts != null)
                    {
                        var source = new ObservableCollection<ApiAccount>(user.ApiAcounts.Select(x => x.ToFoundation()));
                        var inventoryComparer = AnonymousComparer.Create((ApiAccount x) => x.ApiAccountId);
                        acount.ApiAccounts.ObserveCollection(x => repository.Add(x), x => repository.Remove(x));
                        source.Patch(acount.ApiAccounts, inventoryComparer, (sourceAccount, targetAccount) => sourceAccount.Patch(targetAccount));
                    }

                    repository.UnitOfWork.Commit();
                }

                return Ok();
            }

            return BadRequest(String.Join(" ", result.Errors));
        }

        /// <summary>
        /// POST: api/security/users/create
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("users/create")]
        public async Task<IHttpActionResult> CreateAsync(ApplicationUserExtended user)
        {
            var dbUser = new ApplicationUser();

            dbUser.InjectFrom(user);

            IdentityResult result;
            if (!string.IsNullOrEmpty(user.Password))
            {
                result = await UserManager.CreateAsync(dbUser, user.Password);
            }
            else
            {
                result = await UserManager.CreateAsync(dbUser);
            }

            if (result.Succeeded)
            {
                using (var repository = _securityRepository())
                {
                    var account = new Account
                    {
                        UserName = user.UserName,
                        AccountId = user.Id,
                        MemberId = user.MemberId,
                        AccountState = AccountState.Approved.GetHashCode(),
                        RegisterType = user.UserType.GetHashCode(),
                        StoreId = user.StoreId
                    };

                    repository.Add(account);
                    repository.UnitOfWork.Commit();
                }

                return Ok();
            }
            else
            {
                return BadRequest(String.Join(" ", result.Errors));
            }
        }

        /// <summary>
        /// POST: api/security/notifications
        /// </summary>
        /// <param name="securityMessage"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("users/notifications")]
        public async Task<IHttpActionResult> CreateSendMessageJob(SecurityMessage securityMessage)
        {
            // AO
            // TODO: Get actual store
            // TODO: Localize subject and message to user's shop language
            // TODO: Queue as job

            SendingMethod sendingMethod;
            if (Enum.TryParse(securityMessage.SendingMethod, out sendingMethod))
            {
                if (sendingMethod == SendingMethod.Email)
                {
                    string code = await UserManager.GeneratePasswordResetTokenAsync(securityMessage.UserId);

                    var uriBuilder = new UriBuilder(securityMessage.CallbackUrl);
                    var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                    query["code"] = code;
                    uriBuilder.Query = query.ToString();

                    string message = string.Format(
                        "Please reset your password by clicking <strong><a href=\"{0}\">here</a></strong>",
                        HttpUtility.HtmlEncode(uriBuilder.ToString()));
                    string subject = string.Format("{0} reset password link", securityMessage.StoreId);

                    await UserManager.SendEmailAsync(securityMessage.UserId, subject, message);

                    return Ok();
                }
            }

            return BadRequest();
        }

        #endregion

        #endregion

        #region Helpers

        private async Task<ApplicationUserExtended> GetUserExtended(string loginProvider, string providerKey)
        {
            var applicationUser = await UserManager.FindAsync(new UserLoginInfo(loginProvider, providerKey));
            return GetUserExtended(applicationUser);
        }

        private async Task<ApplicationUserExtended> GetUserExtended(string userName)
        {
            var applicationUser = await UserManager.FindByNameAsync(userName);
            return GetUserExtended(applicationUser);
        }

        private ApplicationUserExtended GetUserExtended(ApplicationUser applicationUser)
        {
            ApplicationUserExtended result = null;

            if (applicationUser != null)
            {
                result = new ApplicationUserExtended();
                result.InjectFrom(applicationUser);

                using (var repository = _securityRepository())
                {
                    var user = repository.GetAccountByName(applicationUser.UserName);
                    result.InjectFrom(user);
                    if (user != null)
                    {
                        var permissions =
                            user.RoleAssignments.Select(x => x.Role)
                                .SelectMany(x => x.RolePermissions)
                                .Select(x => x.Permission);

                        result.UserState = (UserState)user.AccountState;
                        result.UserType = (UserType)user.RegisterType;
                        result.Permissions = permissions.Select(x => x.PermissionId).Distinct().ToArray();
                        result.ApiAcounts = user.ApiAccounts.Select(x => x.ToWebModel()).ToList();
                    }
                }
            }

            return result;
        }

        #endregion
    }
}