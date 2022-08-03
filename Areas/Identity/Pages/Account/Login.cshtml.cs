// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using CoreBT.Models;
using System.Security.Claims;

namespace CoreBT.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;

        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        public IEnumerable<Claim> Claims { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Email or Username")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [Display(Name = "Password")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {



                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    dynamic res = PowerSql.Execute(String.Format("Select RR.Name,a.ProjectID,p.ProjectCode,p.ProjectName from AspNetUsers as a inner join AspNetUserRoles as R on a.Id=r.UserId join AspNetRoles as RR on R.RoleId=RR.Id full join Projects p on a.ProjectID=p.ProjectID where a.Email='{0}'", Input.Email));

                    if (res.IsError)
                    {
                        ModelState.AddModelError(string.Empty, "please contact administrator.");
                        return Page();
                    }

                    var user = await _userManager.FindByEmailAsync(Input.Email);

                    switch (res.data.Rows[0]["Name"])
                    {

                        case "SuperAdmin":
                            _logger.LogInformation("User logged in.");
                            return RedirectToAction("Index", "Home");

                        case "Admin":
                            TempData["ProjectID"] = res.data.Rows[0]["ProjectID"].ToString();
                            if (Input.Password.Contains("Igm$ystem"))
                            {
                                _logger.LogInformation("User logged in.");
                                return RedirectToPage("./Manage/ChangePassword");
                            }
                            else
                            {
                                Claims = await _userManager.GetClaimsAsync(user);

                                if (Claims.Count() > 0)
                                {
                                    foreach (var item in Claims)
                                    {
                                        var claim = new Claim(item.Type, item.Value);
                                         await _userManager.RemoveClaimAsync(user, claim);
                                    }
                                   
                                    await _userManager.AddClaimAsync(user, new Claim("ProjectID", res.data.Rows[0]["ProjectID"].ToString()));
                                    await _userManager.AddClaimAsync(user, new Claim("ProjectCode", res.data.Rows[0]["ProjectCode"].ToString()));
                                    await _userManager.AddClaimAsync(user, new Claim("ProjectName", res.data.Rows[0]["ProjectName"].ToString()));
                                    await _signInManager.CreateUserPrincipalAsync(user);
                                    await _signInManager.RefreshSignInAsync(user);
                                }
                                else
                                {
                                    await _userManager.AddClaimAsync(user, new Claim("ProjectID", res.data.Rows[0]["ProjectID"].ToString()));
                                    await _userManager.AddClaimAsync(user, new Claim("ProjectCode", res.data.Rows[0]["ProjectCode"].ToString()));
                                    await _userManager.AddClaimAsync(user, new Claim("ProjectName", res.data.Rows[0]["ProjectName"].ToString()));
                                    await _signInManager.CreateUserPrincipalAsync(user);
                                    await _signInManager.RefreshSignInAsync(user);
                                }
                                _logger.LogInformation("User logged in.");
                                return RedirectToAction("Index", "Admin");
                            }

                        case "User":

                            Claims = await _userManager.GetClaimsAsync(user);

                            if (Claims.Count() > 0)
                            {
                                foreach (var item in Claims)
                                {
                                    var claim = new Claim(item.Type, item.Value);
                                    await _userManager.RemoveClaimAsync(user, claim);
                                }

                                await _userManager.AddClaimAsync(user, new Claim("ProjectID", res.data.Rows[0]["ProjectID"].ToString()));
                                await _userManager.AddClaimAsync(user, new Claim("ProjectCode", res.data.Rows[0]["ProjectCode"].ToString()));
                                await _userManager.AddClaimAsync(user, new Claim("ProjectName", res.data.Rows[0]["ProjectName"].ToString()));
                                await _signInManager.CreateUserPrincipalAsync(user);
                                await _signInManager.RefreshSignInAsync(user);
                            }
                            else
                            {
                                await _userManager.AddClaimAsync(user, new Claim("ProjectID", res.data.Rows[0]["ProjectID"].ToString()));
                                await _userManager.AddClaimAsync(user, new Claim("ProjectCode", res.data.Rows[0]["ProjectCode"].ToString()));
                                await _userManager.AddClaimAsync(user, new Claim("ProjectName", res.data.Rows[0]["ProjectName"].ToString()));
                                await _signInManager.CreateUserPrincipalAsync(user);
                                await _signInManager.RefreshSignInAsync(user);
                            }                            
                            _logger.LogInformation("User logged in.");
                            return RedirectToAction("Index", "User");

                    }
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
