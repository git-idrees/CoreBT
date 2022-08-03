using CoreBT.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreBT.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin, Admin,User")]
    public class ApiController : ControllerBase
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<Tasks> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;


        public ApiController(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<Tasks> logger,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }


        // Menu
        [HttpPost]
        [ActionName("GFM")]
        public object GetFormList(General general)
        {
            dynamic dataObjects;
            dynamic toJsonObjects;
            dataObjects = PowerSql.Execute(String.Format("SELECT [DocumentName] Name,DocID Id FROM [DocumentTypes] where IsActive=1 and ProjectID={0}", general.ID));

            toJsonObjects = new
            {
                IsError = dataObjects.IsError,
                data = dataObjects.data
            };

            return JsonConvert.SerializeObject(toJsonObjects, Formatting.None);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SC")]
        public object SaveCity(Cities _city)
        {
            if (ModelState.IsValid)
            {
                dynamic dataObjects;
                dataObjects = PowerSql.ExecuteSpecial(string.Format("DECLARE @Message AS NVARCHAR(100)  EXECUTE SP_SAVE_CITIES N'{0}',{1},'{2}',@Message output  SELECT @Message Message", _city.CityName, _city.ID, _city.IsActive));

                if (dataObjects.IsError)
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, dataObjects.Message));
                else
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(false, ""));
            }
            else
            {
                return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, "Something went wrong"));
            }
        }

        [HttpGet]
        [ActionName("GCD")]
        public object GetCitiesData()
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute("SELECT CityId,CityName,IsActive,OrderID FROM Cities Order by CityName");
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DC")]
        public object DeleteCity(Delete _del)
        {
            if (!string.IsNullOrEmpty(_del.ID))
            {
                dynamic dataObjects;
                dataObjects = PowerSql.Execute(string.Format("Update Cities set IsActive=0 where CityId={0}", _del.ID));

                if (dataObjects.IsError)
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, dataObjects.Message));
                else
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(false, ""));
            }
            else
            {
                return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, "Something went wrong"));
            }
        }

        [ActionName("GCL")]
        public object GetCityList()
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute("SELECT CityId id,CityName text FROM Cities where IsActive=1 Order by OrderId ");
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("PS")]
        public async Task<object> ProjectSetup(ProjectSetup _p)
        {
            dynamic dataObjects;
            if (ModelState.IsValid)
            {

                Random rnd = new Random();
                int post = rnd.Next(1, 500);
                string temppswd = "Igm$ystem" + post;
                dataObjects = await CreateAdmin(_p.email, temppswd);
                if (!dataObjects.IsError)
                {
                    dataObjects = PowerSql.ExecuteSpecial(string.Format("DECLARE @Message AS NVARCHAR(100)  EXECUTE SP_SETUP_PROJECT N'{0}','{1}',{2},'{3}','{4}',@Message output  SELECT @Message Message", _p.pname, _p.pcode, _p.city, _p.email, ""));

                    if (dataObjects.IsError)
                        return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, dataObjects.Message));
                    else
                        return JsonConvert.SerializeObject(AlertMessages.AlertMessage(false, ""));
                }
                else
                {
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(false, dataObjects.Message));
                }
            }
            else
            {
                return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, "Something went wrong"));
            }
        }


        [ActionName("GPL")]
        public object GetProjectList()
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute("select  p.ProjectID,ProjectName,ProjectCode,c.CityID,c.CityName 'City',p.IsActive from Projects p inner join ProjectCities pc on p.ProjectID=pc.ProjectID join Cities c on pc.CityID=c.CityID  ");
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }

        // Document Types

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("GDTL")]
        public object GetDocumentTypeList(General general)
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute(string.Format("SELECT DocID ID ,DocumentName DName,IsActive,ProjectID PID FROM DocumentTypes where ProjectID='{0}' Order by DocumentName", general.ID));
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SDT")]
        public object SaveDocumentType(Document d)
        {
            if (ModelState.IsValid)
            {
                dynamic dataObjects;
                dataObjects = PowerSql.ExecuteSpecial(string.Format("DECLARE @Message AS NVARCHAR(100)  EXECUTE SP_SAVE_DOCUMENT N'{0}',{1},{2},{3},@Message output  SELECT @Message Message", d.DName, d.ID, d.ProjectID, d.IsActive));

                if (dataObjects.IsError)
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, dataObjects.Message));
                else
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(false, ""));
            }
            else
            {
                return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, "Something went wrong"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DDT")]
        public object DisableDocumentType(Delete d)
        {
            if (!string.IsNullOrEmpty(d.ID))
            {
                dynamic dataObjects;
                dataObjects = PowerSql.ExecuteSpecial(string.Format("DECLARE @Message AS NVARCHAR(100)  EXECUTE SP_DISABLE_DOCUMENT {0},@Message output  SELECT @Message Message", d.ID));

                if (dataObjects.IsError)
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, dataObjects.Message));
                else
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(false, ""));
            }
            else
            {
                return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, "Something went wrong"));
            }
        }

        [HttpPost]
        [ActionName("GD")]
        public object GetDocument(General general)
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute(String.Format("SELECT [DocID] id,[DocumentName] text FROM DocumentTypes where IsActive=1 and ProjectID in(0,{0}) Order by id", general.ID));
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }

        // Account Level 2

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SAL2")]
        public object SaveAccountL2(AccountL2 d)
        {
            if (ModelState.IsValid)
            {
                dynamic dataObjects;
                dataObjects = PowerSql.ExecuteSpecial(string.Format("DECLARE @Message AS NVARCHAR(100)  EXECUTE SP_SAVE_AccountL2 N'{0}',{1},{2},{3},@Message output  SELECT @Message Message", d.AccountName, d.ID, d.ProjectID, d.IsActive));

                if (dataObjects.IsError)
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, dataObjects.Message));
                else
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(false, ""));
            }
            else
            {
                return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, "Something went wrong"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("GAL2L")]
        public object GetAccountL2List(General general)
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute(string.Format("SELECT ID ,AccountName,IsActive,ProjectID PID FROM AccountLevelTwo where ProjectID={0} Order by AccountName", general.ID));
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DAL2")]
        public object DisableAccountL2(Delete d)
        {
            if (!string.IsNullOrEmpty(d.ID))
            {
                dynamic dataObjects;
                dataObjects = PowerSql.ExecuteSpecial(string.Format("DECLARE @Message AS NVARCHAR(100)  EXECUTE SP_DISABLE_ACCOUNTL2 {0},@Message output  SELECT @Message Message", d.ID));

                if (dataObjects.IsError)
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, dataObjects.Message));
                else
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(false, ""));
            }
            else
            {
                return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, "Something went wrong"));
            }
        }



        // Account Level 3

        [HttpPost]
        [ActionName("GAL2DD")]
        public object GetAccountLevel2DropData(General general)
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute(String.Format("SELECT [ID] id,[AccountName] text FROM AccountLevelTwo where IsActive=1 and ProjectID={0} Order by AccountName", general.ID));
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }

        [HttpPost]
        [ActionName("GAL2DDS")]
        public object GetAccountLevel2DropDataSpecial(General general)
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute(String.Format(" Select '0' id,'None' text union SELECT [ID] id,[AccountName] text FROM AccountLevelTwo where IsActive=1 and ProjectID={0} ", general.ID));
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }


        [HttpPost]
        [ActionName("GCDDS")]
        public object GetCategoryDropDataSpecial(General general)
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute(String.Format(" Select '0' id,'None' text union SELECT [DocID] id,[DocumentName] text FROM DocumentTypes where IsActive=1 and ProjectID={0} ", general.ID));
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }


        [HttpPost]
        [ActionName("GAL3DD")]
        public object GetAccountLevel3DropData(General general)
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute(String.Format("SELECT [ID] id,[AccountName] text FROM AccountLevelThree where IsActive=1 and AccountLevel2ID={0} Order by AccountName", general.ID));
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }

        [HttpPost]
        [ActionName("GAL3DDS")]
        public object GetAccountLevel3DropDataSpecial(General general)
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute(String.Format("Select '0' id,'None' text union  SELECT [ID] id,[AccountName] text FROM AccountLevelThree where IsActive=1 and AccountLevel2ID={0} ", general.ID));
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SAL3")]
        public object SaveAccountL3(AccountL3 d)
        {
            if (ModelState.IsValid)
            {
                dynamic dataObjects;
                dataObjects = PowerSql.ExecuteSpecial(string.Format("DECLARE @Message AS NVARCHAR(100)  EXECUTE SP_SAVE_AccountL3 N'{0}',{1},{2},{3},@Message output  SELECT @Message Message", d.AccountName, d.ID, d.AccountL2ID, d.IsActive));

                if (dataObjects.IsError)
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, dataObjects.Message));
                else
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(false, ""));
            }
            else
            {
                return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, "Something went wrong"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("GAL3L")]
        public object GetAccountL3List(General general)
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute(string.Format("SELECT a.ID ,a.AccountName,a.IsActive,b.AccountName L2Name,b.ID L2ID FROM AccountLevelThree a inner join AccountLevelTwo b on a.AccountLevel2ID=b.ID where b.ProjectID={0} Order by a.AccountName", general.ID));
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DAL3")]
        public object DisableAccountL3(Delete d)
        {
            if (!string.IsNullOrEmpty(d.ID))
            {
                dynamic dataObjects;
                dataObjects = PowerSql.ExecuteSpecial(string.Format("DECLARE @Message AS NVARCHAR(100)  EXECUTE SP_DISABLE_ACCOUNTL3 {0},@Message output  SELECT @Message Message", d.ID));

                if (dataObjects.IsError)
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, dataObjects.Message));
                else
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(false, ""));
            }
            else
            {
                return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, "Something went wrong"));
            }
        }

        //Users

        [ActionName("GRsL")]
        public object GetRolesList(General g)
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute(String.Format("SELECT Id id,Name text FROM Roles where IsActive=1 and L1={0} Order by Id ",g.ID));
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SUsr")]
        public object SaveUsers(User user)
        {
            dynamic dataObjects;
            if (string.IsNullOrEmpty(user.ID))
            {
                user.FullName = user.FullName.Replace(" ", "_");
                dataObjects =CreateUser(user);
                if (dataObjects.Result.IsError)
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, dataObjects.Result.Message));
                else
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(false, ""));
            }
            else
            {               
                dataObjects = UpdateUser(user);
                return dataObjects;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("GUsL")]
        public object GetUsersList(General general)
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute(string.Format(" select u.Id,ar.EmpID,UserName Name,Email,rs.Name [Role],U.LockoutEnabled IsActive from AspNetUsers u inner join AspNetUserRoles r on u.Id=r.UserId join ApplicationUsersRoles ar on u.Id=ar.UserID join Roles rs on ar.RoleID=rs.ID  where u.ProjectID = {0}", general.ID));
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }

        // Form


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("CL")]
        public object CheckLevel(General general)
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute(string.Format("SELECT  count(*) Levels from AccountLevelTwo where ProjectID={0}", general.ID));
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SFs")]
        public object SaveForms(Forms f)
        {
            if (ModelState.IsValid)
            {
                //Level Logic 
                long LevelId = 0;
                long Level = 0;
                if(f.L1 != 0)
                {
                    if(f.L2 != 0)
                    {
                        if (f.L3 != 0) { LevelId = f.L3; Level = 3; }
                        else { LevelId = f.L2; Level = 2; }
                    }
                    else{ LevelId=f.L1; Level = 1; }
                }
                if (LevelId != 0 && Level !=0) {

                    dynamic dataObjects;
                    var userid = _userManager.GetUserId(User);
                    dataObjects = PowerSql.ExecuteSpecial(string.Format("DECLARE @Message AS NVARCHAR(100)  EXECUTE SP_SAVE_FORMS {0},N'{1}',{2},{3},'{4}',{5},'{6}',{7},{8},@Message output  SELECT @Message Message", f.ID, f.Name, f.L1,LevelId, f.LevelName, f.IsActive, userid,f.D,Level));
                    if (dataObjects.IsError)
                        return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, dataObjects.Message));
                    else
                        return JsonConvert.SerializeObject(AlertMessages.AlertMessage(false, ""));
                }
                else
                {
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, "Please review input data"));
                }
            }
            else
            {
                return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, "Please review input data"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("GFSL")]
        public object GetFormsList(General general)
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute(string.Format("SELECT  f.ID ,Name,ProjectID L1,fl.[Level],fl.LevelID,fl.SubLevelID,LevelName,IsActive FROM Forms f inner join FormLevels fl on f.ID=fl.FormID where ProjectID={0} Order by f.ID", general.ID));
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DF")]
        public object DisableForm(General g)
        {
            if (g.ID  !=0)
            {
                dynamic dataObjects;
                dataObjects = PowerSql.ExecuteSpecial(string.Format("DECLARE @Message AS NVARCHAR(100)  EXECUTE SP_DISABLE_FORM {0},@Message output  SELECT @Message Message", g.ID));

                if (dataObjects.IsError)
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, dataObjects.Message));
                else
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(false, ""));
            }
            else
            {
                return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, "Something went wrong"));
            }
        }


        [ActionName("RAL")]
        public object RolesAccessList(Roles r)
        {
            dynamic dataObjects;
            if(r.RID != 0)
            {
                dataObjects = PowerSql.Execute(string.Format("select ISNULL(RoleID,0) RoleID,f.ID,f.Name, ISNULL(IsView,0) IsView,ISNULL(IsEdit,0) IsEdit,ISNULL(IsDelete,0) IsDelete,ISNULL(IsDisable,0) IsDisable  from RoleAccess ra full join Forms f on ra.FormID=f.ID Where f.ProjectID={0} and ra.RoleId={1}", r.ID,r.RID));
            }
            else
            {
                dataObjects = PowerSql.Execute(string.Format("select ISNULL(RoleID,0) RoleID,f.ID,f.Name, ISNULL(IsView,0) IsView,ISNULL(IsEdit,0) IsEdit,ISNULL(IsDelete,0) IsDelete,ISNULL(IsDisable,0) IsDisable  from RoleAccess ra full join Forms f on ra.FormID=f.ID Where f.ProjectID={0}", r.ID));
            }
            
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("LF")]
        public object LoadFields(General2 gen)
        {
            dynamic dataObjects;
            if (gen.FID != 0)
            {
                dataObjects = PowerSql.Execute(String.Format("select OrderID, [FromID], [FieldID],f.Name ,[FieldDisplayName], [DataTypeID],d.Name TypeName, [Length],tmp.[Status] from TempMappingFromFieldType  tmp inner join Fields f on tmp.FieldID=f.ID join DataTypes d on tmp.DataTypeID=d.ID   Where FieldID not in(select FieldID from MappingFromFieldType where FromID={0}) and TempID is null  order by OrderID  ", gen.FID));
            }
            else
            {
                dataObjects = PowerSql.Execute(String.Format("select OrderID, [FromID], [FieldID],f.Name ,[FieldDisplayName], [DataTypeID],d.Name TypeName, [Length],tmp.[Status]  from TempMappingFromFieldType  tmp inner join Fields f on tmp.FieldID=f.ID join DataTypes d on tmp.DataTypeID=d.ID  Where FieldID not in(select FieldID from TempMappingFromFieldType where TempID='{0}') and TempID is null  order by OrderID  ", gen.ID));
            }
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }



        [HttpPost]
        [ActionName("PDF")]
        public object PrepareDefaultForm(Default def)
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute(string.Format("DECLARE @Message AS NVARCHAR(100)  EXECUTE SP_PREPARE_DEFAULT_FORM {0},'{1}'", def.ID, def.TempID));
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }

        [HttpPost]
        [ActionName("GFF")]
        public object GetFormFunction(FieldFunction ff)
        {
            dynamic dataObjects;
            if (string.IsNullOrEmpty(ff.FromID))
            {
                dataObjects = PowerSql.Execute(String.Format(" SELECT  ID ,Name,ID [status]   FROM Functions where DataTypeID in (0,{0}) and ID in (select FunctionID from TempMappingFieldFunctions where TempID='{1}' and FieldID={2}) Union SELECT  ID ,Name,null [status]   FROM Functions where  DataTypeID in (0,{0}) and ID not in (select FunctionID from TempMappingFieldFunctions where TempID='{1}' and FieldID={2}) ", ff.ID, ff.TempID, ff.FID));
            }
            else
            {
                dataObjects = PowerSql.Execute(String.Format(" select f.ID,Name,1 status from MappingFieldFunction m inner join MappingFromFieldType mm on m.FFTID=mm.ID join Functions f on m.FunctionID=f.ID  where mm.FromID={1} union  SELECT  ID ,Name,null [status]   FROM Functions where DataTypeID in (0,{0}) and ID not in (select f.ID from MappingFieldFunction m inner join MappingFromFieldType mm on m.FFTID=mm.ID join Functions f on m.FunctionID=f.ID where mm.FromID={1})  ", ff.ID, ff.FromID, ff.FID));
            }
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("GF")]
        public object GetForm(Filter temp)
        {
            dynamic dataObjects;
            string Where = String.Format("Where AccL1ID={0} and", temp.L1);
            if (temp.L2 != "0")
            {
                Where = Where + " AccL2ID=" + temp.L2 + " and";
            }

            if (temp.L3 != "0")
            {
                Where = Where + " AccL3ID=" + temp.L3 + " and";
            }

            if (temp.DocID != "0")
            {
                Where = Where + " DocID=" + temp.DocID + " and";
            }

            if (Where.Length > 6)
            {
                Where = Where.Substring(0, Where.Length - 3);
                dataObjects = PowerSql.Execute(string.Format("DECLARE @Message AS NVARCHAR(100)  EXECUTE SP_FORM_FILTER '{0}','{1}'", Where, temp.TempID));
                return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
            }
            else
            {
                return JsonConvert.SerializeObject("", Formatting.None);

            }
        }

        /// Accounts
        /// 

        public async Task<object> CreateAdmin(string email, string pass)
        {
            string returnUrl = Url.Content("~/");
            dynamic dataObjects = string.Empty;
            var user = CreateUser();
            await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, pass);

            if (result.Succeeded)
            {
                var defaultrole = _roleManager.FindByNameAsync("Admin").Result;
                if (defaultrole != null)
                {
                    IdentityResult roleresult = await _userManager.AddToRoleAsync(user, defaultrole.Name);
                }

                _logger.LogInformation("User created a new account with password.");

                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                //bool res = await SendEmailAsync(email,  $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.", "Confirm your email");
                var loginurl = Url.Page("/Account/Login", pageHandler: null);
                bool res = await SendEmailAsync(user.Email, "Account successfull created. <br>  Please use temporary password : " + pass + " for  <a href=\"" + loginurl + "\">login</a> <br> Before start please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>", "Confirm your account");
                PowerSql.ExecuteSpecial(string.Format("EXECUTE SP_SAVE_AuthEmail N'{0}',{1}", user.Email, res));

                dataObjects = new
                {
                    IsError = false
                };
            }
            else
            {
                dataObjects = new
                {
                    IsError = true,
                    Message = result.Errors.First().Description.ToString()
                };
            }

            return dataObjects;
        }

        public async Task<object> UpdateUser(User _user)
        {
            var user = await _userManager.FindByIdAsync(_user.ID);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, _user.Pass);
            return result;
        }
        public async Task<object> CreateUser(User _user)
        {
            string returnUrl = Url.Content("~/");
            dynamic dataObjects = string.Empty;
            var Userid = _userManager.GetUserId(User);
            var user = CreateUser();
            await _userStore.SetUserNameAsync(user, _user.FullName, CancellationToken.None);
            if(PowerSql.IsValidEmail(_user.Email))
            {
                await _emailStore.SetEmailAsync(user, _user.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, _user.Pass);

                if (result.Succeeded)
                {
                    var defaultrole = _roleManager.FindByNameAsync("User").Result;
                    if (defaultrole != null)
                    {
                        IdentityResult roleresult = await _userManager.AddToRoleAsync(user, defaultrole.Name);
                    }

                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //bool res = await SendEmailAsync(email,  $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.", "Confirm your email");
                    var loginurl = Url.Page("/Account/Login", pageHandler: null);
                    bool res = await SendEmailAsync(user.Email, "Account successfull created. <br>  Please use password : " + _user.Pass + " for  <a href=\"" + loginurl + "\">login</a> <br> Before start please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>", "Confirm your account");
                    PowerSql.ExecuteSpecial(string.Format("EXECUTE SP_SAVE_AuthEmail N'{0}',{1}", user.Email, res));
                    PowerSql.Execute(string.Format("Update AspNetUsers set ProjectID={4} where id='{0}'  insert into ApplicationUsersRoles ( [UserID], [EmpID], [RoleID], [AddedBy]) values('{0}',{1},{2},'{3}')", user.Id,_user.EmpID,_user.RID,Userid,_user.ProjectID));
                    dataObjects = new
                    {
                        IsError = false
                    };
                }
                else
                {
                    dataObjects = new
                    {
                        IsError = true,
                        Message = result.Errors.First().Description.ToString()
                    };
                }

            }
            else
            {
                _user.Email = _user.Email.Replace(" ", "_");
                await _emailStore.SetEmailAsync(user, _user.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, _user.Pass);

                if (result.Succeeded)
                {
                    var defaultrole = _roleManager.FindByNameAsync("User").Result;
                    if (defaultrole != null)
                    {
                        IdentityResult roleresult = await _userManager.AddToRoleAsync(user, defaultrole.Name);
                    }

                    _logger.LogInformation("User created a new account with password.");
                    PowerSql.Execute(string.Format(" Update AspNetUsers set ProjectID={4},EmailConfirmed=1 where id='{0}' insert into ApplicationUsersRoles ( [UserID], [EmpID], [RoleID], [AddedBy]) values('{0}',{1},{2},'{3}')", user.Id, _user.EmpID, _user.RID, Userid,_user.ProjectID));
                    dataObjects = new
                    {
                        IsError = false
                    };
                }
                else
                {
                    dataObjects = new
                    {
                        IsError = true,
                        Message = result.Errors.First().Description.ToString()
                    };
                }
            }

            return dataObjects;
          
        }

        // Default for Accounts

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        public async Task<bool> SendEmailAsync(string email, string msg, string subject = "")
        {
            // Initialization.  
            bool isSend = false;

            try
            {
                // Initialization.  
                var body = msg;
                var message = new MailMessage();

                // Settings.  
                message.To.Add(new MailAddress(email));
                message.From = new MailAddress("mohd.idrees.cse@outlook.com");
                message.Subject = !string.IsNullOrEmpty(subject) ? subject : "dms-world Technical Support";
                message.Body = body;
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    // Settings.  
                    var credential = new NetworkCredential
                    {
                        UserName = "mohd.idrees.cse@outlook.com",
                        Password = "!dree$123"
                    };

                    // Settings.  
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.office365.com";
                    smtp.Port = Convert.ToInt32(587);
                    smtp.EnableSsl = true;

                    // Sending  
                    await smtp.SendMailAsync(message);

                    // Settings.  
                    isSend = true;
                }
            }
            catch (Exception)
            {
                // Info  
                return isSend;
            }
            return isSend;
        }


        /// User Apis


        [HttpPost]
        // [ValidateAntiForgeryToken]
        [ActionName("GUFM")]
        public object GetUserFormMenu(General general)
        {
            Rootmenu dataObjects = new Rootmenu();
            dynamic datasqlObjects;
            dynamic childs = string.Empty;
            datasqlObjects = PowerSql.ExecuteMultiDataSet(string.Format("SELECT ROW_NUMBER() OVER(ORDER BY name ASC) AS id, Name text,AccL1ID,AccL2ID,AccL3ID,DocID FROM [igmsys].[dbo].[Forms] where AccL1ID={0}   select ProjectID ID, ProjectName Name from Projects where ProjectID={0} select ID,AccountName Name,IsActive from AccountLevelTwo Where ProjectID={0} select ID,AccountName Name,IsActive from AccountLevelThree Where AccountLevel2ID in(select Id from AccountLevelTwo where ProjectID={0}) select Docid ID,DocumentName,IsActive Name from DocumentTypes where ProjectId={0}", general.ID));
            if (!datasqlObjects.IsError)
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                ds = datasqlObjects.data;
                // First Level 

                dt = ds.Tables[0];
                DataRow[] results = dt.Select("AccL1ID=" + general.ID + " and AccL2ID=0 and AccL3ID=0 and DocID=0");

                // If results is zero that means no form found on level1

                if (results.Length > 0)
                {


                    List<Child> list = (from row in results.AsEnumerable()
                                        select new Child
                                        {
                                            text = row.Field<string>("text"),
                                            type = "form"
                                        }).ToList();


                    State state = new State();
                    state.opened = true;


                    dataObjects.id = Convert.ToInt32(ds.Tables[1].Rows[0]["ID"]);
                    dataObjects.text = ds.Tables[1].Rows[0]["Name"].ToString();
                    dataObjects.state = state;

                    dataObjects.children = list;


                }
                else
                {
                    dataObjects.id = Convert.ToInt32(ds.Tables[1].Rows[0]["ID"]);
                    dataObjects.text = ds.Tables[1].Rows[0]["Name"].ToString();
                }




            }
            return JsonConvert.SerializeObject(dataObjects, Formatting.None);

        }


        // User Management

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SR")]
        public object SaveRole(Role d)
        {
            if (ModelState.IsValid)
            {
                var id = _userManager.GetUserId(User);

                dynamic dataObjects;
                dataObjects = PowerSql.ExecuteSpecial(string.Format("DECLARE @Message AS NVARCHAR(100)  EXECUTE SP_SAVE_ROLE N'{0}',{1},{2},'{3}',@Message output  SELECT @Message Message", d.Name, d.ID, d.ProjectID, id));

                if (dataObjects.IsError)
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, dataObjects.Message));
                else
                    return JsonConvert.SerializeObject(AlertMessages.AlertMessage(false, ""));
            }
            else
            {
                return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, "Something went wrong"));
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("GRL")]
        public object GetRoleList(General general)
        {
            dynamic dataObjects;
            dataObjects = PowerSql.Execute(string.Format("SELECT  ID ,Name,IsActive,L1 FROM Roles where L1={0} Order by Name", general.ID));
            return JsonConvert.SerializeObject(dataObjects.data, Formatting.None);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DR")]
        public object DisableRole(General gen)
        {

            dynamic dataObjects;
            var userid = _userManager.GetUserId(User);
            dataObjects = PowerSql.ExecuteSpecial(string.Format("DECLARE @Message AS NVARCHAR(100)  EXECUTE SP_DISABLE_ROLE {0},'{1}',@Message output  SELECT @Message Message", gen.ID,userid));

            if (dataObjects.IsError)
                return JsonConvert.SerializeObject(AlertMessages.AlertMessage(true, dataObjects.Message));
            else
                return JsonConvert.SerializeObject(AlertMessages.AlertMessage(false, ""));

        }

    }
}
