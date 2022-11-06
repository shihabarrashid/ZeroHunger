using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZeroHunger.DB;

namespace ZeroHunger.Controllers
{
    public class RestaurantController : Controller
    {
        // GET: Restaurant
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
            Restaurant rest = (from r in db.Restaurants
                                       where r.Username.Equals(Username) &&
                                             r.Password.Equals(Password)
                                       select r).FirstOrDefault();
            if (rest == null)
            {
                TempData["message"] = "Invalid Username or Password!";
                return View();
            }
            else
            {
                var loggedRest = (from r in db.Restaurants
                                 where r.Username.Equals(Username)
                                 select r).SingleOrDefault();
                var restId = loggedRest.Id;
                Session["loggedRest"] = restId;
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Remove("loggedRest");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CollectionRequest()
        {
            var db = new ZeroHungerEntities();
            var loggedRest = Convert.ToInt32(Session["loggedRest"]);
            var ext = (from cr in db.FoodCollections
                       where cr.RequestedRestaurant == loggedRest
                       select cr).ToList();

            return View(ext);
        }

        [HttpGet]
        public ActionResult CreateRequest()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateRequest(string FoodType, string Quantity)
        {
            var db = new ZeroHungerEntities();
            FoodCollection fc = new FoodCollection();
            fc.FoodType = FoodType;
            fc.Quantity = Quantity;
            fc.RequestedRestaurant = Convert.ToInt32(Session["loggedRest"]);
            fc.Status = "pending";
            db.FoodCollections.Add(fc);
            db.SaveChanges();
            TempData["message"] = "Collection Request Added";
            return RedirectToAction("CollectionRequest");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var db = new ZeroHungerEntities();
            var ext = (from fr in db.FoodCollections
                       where fr.Id == id
                       select fr).SingleOrDefault();

            db.FoodCollections.Remove(ext);
            db.SaveChanges();
            TempData["message"] = "Collection Request Removed!.";
            return RedirectToAction("CollectionRequest");
        }

        [HttpGet]
        public ActionResult ManageRestaurants()
        {
            var db = new ZeroHungerEntities();
            var restaurants = db.Restaurants.ToList();
            return View(restaurants);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Restaurant rest)
        {
            var db = new ZeroHungerEntities();
            db.Restaurants.Add(rest);
            db.SaveChanges();
            TempData["message"] = "Restaurat Added";
            return RedirectToAction("ManageRestaurants");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var db = new ZeroHungerEntities();
            var ext = (from r in db.Restaurants
                       where r.Id == id
                       select r).SingleOrDefault();
            return View(ext);
        }
        [HttpPost]
        public ActionResult Edit(Restaurant rest)
        {
            var db = new ZeroHungerEntities();
            var ext = (from r in db.Restaurants
                       where r.Id == rest.Id
                       select r).SingleOrDefault();
            ext.Name = rest.Name;
            ext.Username = rest.Username;
            ext.Password = rest.Password;
            ext.Location = rest.Location;
            db.SaveChanges();

            TempData["message"] = "Restaurant Updated.";
            return RedirectToAction("ManageRestaurants");
        }

        
        [HttpGet]
        public ActionResult DeleteRestaurant(int id)
        {
            var db = new ZeroHungerEntities();
            var ext = (from rest in db.Restaurants
                       where rest.Id == id
                       select rest).SingleOrDefault();

            db.Restaurants.Remove(ext);
            db.SaveChanges();
            TempData["message"] = "Restaurant Deleted.";
            return RedirectToAction("ManageRestaurants");
        }
    }
}