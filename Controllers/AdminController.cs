using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZeroHunger.DB;

namespace ZeroHunger.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
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
            NGOAdmin ngoAdmin = (from admin in db.NGOAdmins
                                       where admin.Username.Equals(Username) &&
                                             admin.Password.Equals(Password)
                                       select admin).FirstOrDefault();
            if (ngoAdmin == null)
            {
                TempData["message"] = "Invalid Username or Password!";
                return View();
            }
            else
            {
                var loggedAdmin = (from admin in db.NGOAdmins
                                 where admin.Username.Equals(Username)
                                 select admin).SingleOrDefault();
                var adminId = loggedAdmin.Id;
                Session["loggedAdmin"] = adminId;
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Remove("loggedAdmin");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult CollectionRequests()
        {
            var db = new ZeroHungerEntities();
            var foodCollections = db.FoodCollections.ToList();
            return View(foodCollections);
        }

        [HttpGet]
        public ActionResult AssignToEmployee(int id)
        {
            var db = new ZeroHungerEntities();
            var ext = (from fc in db.FoodCollections
                       where fc.Id == id
                       select fc).SingleOrDefault();
            return View(ext);
        }

        [HttpPost]
        public ActionResult AssignEmployee(int Id, int AssignedEmployee)
        {
            var db = new ZeroHungerEntities();
            var ext = (from fc in db.FoodCollections
                       where fc.Id == Id
                       select fc).SingleOrDefault();
           ext.AssignedEmployee = AssignedEmployee;
            db.SaveChanges();

            TempData["message"] = "Employee Assigned.";
            return RedirectToAction("Dashboard");
        }
    }
}