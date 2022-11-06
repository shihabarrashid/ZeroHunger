using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ZeroHunger.DB;

namespace ZeroHunger.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string Username, string Password)
        {
            var db = new ZeroHungerEntities();
            NGOEmployee ngoEmployee = (from emp in db.NGOEmployees
                                                   where emp.Username.Equals(Username) &&
                                                         emp.Password.Equals(Password)
                                                   select emp).FirstOrDefault();
            if(ngoEmployee == null)
            {
                TempData["message"] = "Invalid Username or Password!";
                return View();
            }
            else
            {
                var loggedEmp = (from emp in db.NGOEmployees
                            where emp.Username.Equals(Username)
                            select emp).SingleOrDefault();
                var empId = loggedEmp.Id;
                Session["loggedEmp"] = empId;
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Remove("loggedEmp");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            return View();
        }


        [HttpGet]
        public ActionResult ManageEmployee()
        {
            var db = new ZeroHungerEntities();
            var employees = db.NGOEmployees.ToList();
            return View(employees);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(NGOEmployee emp)
        {
            var db = new ZeroHungerEntities();
            db.NGOEmployees.Add(emp);
            db.SaveChanges();
            TempData["message"] = "Employee Added";
            return RedirectToAction("ManageEmployee");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var db = new ZeroHungerEntities();
            var ext = (from emp in db.NGOEmployees
                       where emp.Id == id
                       select emp).SingleOrDefault();
            return View(ext);
        }

        [HttpPost]
        public ActionResult Edit(NGOEmployee employee)
        {
            var db = new ZeroHungerEntities();
            var ext = (from emp in db.NGOEmployees
                       where emp.Id == employee.Id
                       select emp).SingleOrDefault();
            ext.Name = employee.Name;
            ext.Username = employee.Username;
            ext.Password = employee.Password;
            ext.Location = employee.Location;
            db.SaveChanges();

            TempData["message"] = "Employee Updated.";
            return RedirectToAction("ManageEmployee");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var db = new ZeroHungerEntities();
            var ext = (from emp in db.NGOEmployees
                       where emp.Id == id
                       select emp).SingleOrDefault();

            db.NGOEmployees.Remove(ext);
            db.SaveChanges();
            TempData["message"] = "Employee Delete.";
            return RedirectToAction("ManageEmployee");
        }

        public ActionResult MyCollectionRequest()
        {
            var db = new ZeroHungerEntities();
            var loggedEmp = Convert.ToInt32(Session["loggedEmp"]);
            var ext = (from cr in db.FoodCollections
                       where cr.AssignedEmployee == loggedEmp
                       select cr).ToList();

            return View(ext);
        }

        public ActionResult MakeCollected(int id)
        {
            var db = new ZeroHungerEntities();
            var ext = (from cr in db.FoodCollections
                       where cr.Id == id
                       select cr).SingleOrDefault();

            ext.Status = "collected";
            db.SaveChanges();
            return RedirectToAction("MyCollectionRequest");
        }
    }
}