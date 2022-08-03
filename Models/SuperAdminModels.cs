using System.ComponentModel.DataAnnotations;

namespace CoreBT.Models
{

    public class General
    {
        [Required]
        public Int64 ID { get; set; }
    }

    public class General2
    {
        [Required]
        public string ID { get; set; }

        public Int64 FID { get; set; }
    }

    public class RoleAccess
    {
        public Int64 FormID { get; set; }
        public bool IsView { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsDisable { get; set; }


    }

    public class Roles
    {
        [Required]
        public Int64 ID { get; set; }

        [Required]
        public Int64 RID { get; set; }
    }


    public class Child
    {
        public string text { get; set; }
        public string type { get; set; }
    }

    public class Rootmenu
    {
        public int id { get; set; }
        public string text { get; set; }
        public List<Child> children { get; set; }
        public State state { get; set; }
        public string type { get; set; }
    }

    public class State
    {
        public bool opened { get; set; }
    }

    public class stringID
    {
        public string ID { get; set; }

    }

    public class Level
    {
        public Int64 L { get; set; }

        public Int64 L1 { get; set; }
        public Int64 L2 { get; set; }
        public Int64 D { get; set; }
    }


    public class Form
    {
        public string ID { get; set; }
        public string DN { get; set; }
        public string DT { get; set; }
        public string LEN { get; set; }
        public string O { get; set; }

    }

    public class Root
    {
        List<stringID> temp { get; set; }
    }

    public class Default
    {
        [Required]
        public Int64 ID { get; set; }

        [Required]
        public string TempID { get; set; }       
    }

    public class FieldFunction
    {
        [Required]
        public Int64 ID { get; set; }

        [Required]
        public Int64 FID { get; set; }

        [Required]
        public string TempID { get; set; }

        public string FromID { get; set; }
    }

    public class FormFilter
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Forms
    {
        [Required]
        public Int64 ID { get; set; }

        [Required]
        public Int64 L1 { get; set; }

        public Int64 L2 { get; set; }

        public Int64 L3 { get; set; }

        public Int64 D { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LevelName { get; set; }

        [Required]
        public bool IsActive { get; set; }

    }


    public class Filter
    {
        [Required]
        public String L1 { get; set; }

        public String L2 { get; set; }
        public String L3 { get; set; }
        public String DocID { get; set; }

        public String TempID { get; set; }

    }
    public class Delete
    {
        [Required]
        public string ID { get; set; }
    }

    public class Cities
    {
        public int ID { get; set; }

        public string Token { get; set; }

        [Display(Name = "City Name *")]
        [Required]
        public string CityName { get; set; }

        [Display(Name = "Active")]
        [Required]
        public bool IsActive { get; set; }
    }


    public class AccountL2
    {
        public Int64 ID { get; set; }

        public string Token { get; set; }

        [Display(Name = "Account Name *")]
        [Required]
        public string AccountName { get; set; }

        [Display(Name = "Active")]
        [Required]
        public bool IsActive { get; set; }

        [Required]
        public Int64 ProjectID { get; set; }
    }

    public class AccountL3
    {
        public Int64 ID { get; set; }

        public string Token { get; set; }

        [Display(Name = "Account Name *")]
        [Required]
        public string AccountName { get; set; }

        [Display(Name = "Active")]
        [Required]
        public bool IsActive { get; set; }

        [Required]
        public Int64 AccountL2ID { get; set; }
    }

    public class Document
    {
        public Int64 ID { get; set; }

        public string Token { get; set; }

        [Display(Name = "Document Name *")]
        [Required]
        public string DName { get; set; }

        [Display(Name = "Active")]
        [Required]
        public bool IsActive { get; set; }

        [Required]
        public Int64 ProjectID { get; set; }
    }

    public class User
    {
        public string ID { get; set; }

        [Required]
        public Int64 ProjectID { get; set; }

        [Required]
        public string FullName { get; set; }
     
        public string Email { get; set; }

        [Required]
        public string Pass { get; set; }

        [Required]
        public Int64 RID { get; set; }

        [Required]
        public string EmpID { get; set; }
    }



    public class Role
    {
        public Int64 ID { get; set; }
              
        [Required]
        public string Name { get; set; }
        
        [Required]
        public bool IsActive { get; set; }

        [Required]
        public Int64 ProjectID { get; set; }
    }
    public class ProjectSetup
    {

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        public string pname { get; set; }

        [Required]
        [Display(Name = "Project Code")]

        public string pcode { get; set; }


        [Required]
        public int city { get; set; }

        [Required]
        public string Token { get; set; }

    }



}
