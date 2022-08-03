using CoreBT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Data;

namespace CoreBT.Controllers
{

    [Authorize(Roles = "SuperAdmin, Admin")]
    public class MasterData : Controller
    {

        private UserManager<IdentityUser> _userManager;

        public MasterData(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        // Cities
        public IActionResult City()
        {
            return View();
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


        // Lookups

        public IActionResult LevelThree()
        {
            return View();
        }

        public IActionResult LevelTwo()
        {
            return View();
        }



        public IActionResult Lookup()
        {
            return View();
        }

        public IActionResult DocumentType()
        {
            return View();
        }

        [ActionName("SF")]
        public object SaveFields(List<stringID> temp, string TempID,Int64 ID)
        {
            dynamic dataObjects;
            if (temp.Count > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ID", typeof(Int64));
                foreach (var item in temp)
                {
                    dt.Rows.Add(new object[] { item.ID });
                }

                string commaSeparatedList = string.Join(",", temp.Select(x => x.ID));
                //dataObjects = PowerSql.Execute(string.Format("DECLARE @Message AS NVARCHAR(100)  EXECUTE SP_SAVE_FROMFIELDS '{0}',{1}", TempID, dt));
                dataObjects = PowerSql.ExecuteProcedure(dt, TempID,ID);
                return JsonConvert.SerializeObject(dataObjects, Formatting.None);
            }
            else
            {
                return JsonConvert.SerializeObject("", Formatting.None);
            }
        }


        [ActionName("SFO")]
        public object SaveForm(List<Form> temp, string TempID, List<Level> levels, string NM,Int64 ID)
        {
            dynamic dataObjects;
            if (temp.Count > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("FieldID", typeof(Int64));
                dt.Columns.Add("DataTypeID", typeof(Int64));
                dt.Columns.Add("Length", typeof(Int64));
                dt.Columns.Add("DisplayName", typeof(string));
                dt.Columns.Add("OrderID", typeof(Int64));

                foreach (var item in temp)
                {
                    dt.Rows.Add(new object[] { item.ID, item.DT, item.LEN, item.DN, item.O });
                }
                var id = _userManager.GetUserId(User);
                long l, l1, l2, D;
                bool isNumeric = long.TryParse(levels.FirstOrDefault().L.ToString(), out l);
                bool isNumeric1 = long.TryParse(levels.FirstOrDefault().L1.ToString(), out l1);
                bool isNumeric2 = long.TryParse(levels.FirstOrDefault().L2.ToString(), out l2);
                bool isNumeric3 = long.TryParse(levels.FirstOrDefault().D.ToString(), out D);

                if (isNumeric && isNumeric1 && isNumeric2 && isNumeric3)
                {
                    dataObjects = PowerSql.CreateForm(dt, TempID,l,l1,l2,D, NM, id,ID);
                    return JsonConvert.SerializeObject(dataObjects, Formatting.None);
                }
                else
                {
                    dataObjects = new
                    {
                        IsError = true
                    };
                    return JsonConvert.SerializeObject(dataObjects, Formatting.None);
                }

            }
            else
            {
                dataObjects = new
                {
                    IsError = true
                };
                return JsonConvert.SerializeObject(dataObjects, Formatting.None);
            }
        }

      
        [ActionName("SRs")]
        public object SaveRoles(List<RoleAccess> temp, string NM, Int64 ID, Int64 PID)
        {
            dynamic dataObjects;
            if (temp.Count > 0)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("FormID", typeof(Int64));
                dt.Columns.Add("IsView", typeof(bool));
                dt.Columns.Add("IsEdit", typeof(bool));
                dt.Columns.Add("IsDelete", typeof(bool));
                dt.Columns.Add("IsDisable", typeof(bool));

                foreach (var item in temp)
                {
                    dt.Rows.Add(new object[] { item.FormID, item.IsView, item.IsEdit, item.IsDelete, item.IsDisable });
                }
                var Userid = _userManager.GetUserId(User);                          
                dataObjects = PowerSql.ExecuteRoleAccess(dt, NM,ID,PID,Userid);
                return JsonConvert.SerializeObject(dataObjects, Formatting.None);
            }
            else
            {
                dataObjects = new
                {
                    IsError = true
                };
                return JsonConvert.SerializeObject(dataObjects, Formatting.None);
            }
        }
    }
}
