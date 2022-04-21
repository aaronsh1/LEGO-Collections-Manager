using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LegoCollectionManager.Models;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using LegoCollectionManager.Utils;

namespace LegoCollectionManager.Controllers
{
    public class UserController : Controller
    {
        LegoCollectionDBContext _context = new LegoCollectionDBContext();
        QueryObjectUtil _queryObjectUtil = new QueryObjectUtil();
        LoginUtil _loginUtil = new LoginUtil(); 

        bool validUser = true;
        bool validPassword = true;

        // GET: UserController
        public ActionResult Index()
        {
            ViewData["IncorrectPassword"] = "";
            string active = HttpContext.Session.GetString("_IsActive") ?? "NotActive";
            if(active != "NotActive")
                return RedirectToAction("UserSets");

            return View();
        }

        // GET: UserController/Login
        public ActionResult Login(string Username, string Password)
        {
            ViewData["IncorrectPassword"] = "";
            User userToReturn = (from u in _context.Users
                                 where u.Username == Username
                                 select u).FirstOrDefault();

            bool loggedIn = _loginUtil.login(Username, Password);


            if (!loggedIn)
            {
                validPassword = false;
                ViewData["IncorrectPassword"] = "Incorrect Username or Password. Please Try Again.";
                return View("Index");
            }
            validPassword = true;

            HttpContext.Session.SetInt32("_UserId", userToReturn.UserId);
            HttpContext.Session.SetString("_IsActive", "Active");

            return RedirectToAction("UserSets");
        }

        public ActionResult AddAvatar()
        {
            ViewBag.Avatars = _queryObjectUtil.getAvatarList();
            return View();
        }

        [HttpPost]
        public ActionResult AddAvatar(IFormCollection form)
        {
            string avatarIdstring = form["Avatar"];
            int avatarId = Int32.Parse(avatarIdstring);
            string SessionUserId = "_UserId";
            int userId = (int)HttpContext.Session.GetInt32(SessionUserId);
            UserAvatar userAvatarToAdd = new UserAvatar();
            userAvatarToAdd.Avatar = avatarId;
            userAvatarToAdd.User = userId;

            _context.UserAvatars.Add(userAvatarToAdd);
            _context.SaveChanges();

            int userToReturn = (int)HttpContext.Session.GetInt32("_UserId");

            ViewBag.UserAvatar = (from ua in _context.UserAvatars
                                  join a in _context.Avatars on ua.Avatar equals a.AvatarId
                                  where a.AvatarId == avatarId
                                  select new
                                  {
                                      link = a.Url
                                  }).FirstOrDefault().link;

            return RedirectToAction("UserSets");
        }


        // GET: UserController/Create
        public ActionResult Create()
        {
            ViewBag.InvalidUser = "";
            return View();
        }


        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection form)
        {
            ViewBag.InvalidUser = "";


            string username = form["username"];
            string password = form["password"];

            bool isCreated = _loginUtil.register(username, password);

            if(!isCreated)
            {
                validUser = false;
                ViewBag.InvalidUser = "Username is taken. Please choose an alternative.";
                return View();
            }
            validUser = true;

            int userId = (from u in _context.Users
                             where u.Username == username
                             select u).FirstOrDefault().UserId;

            HttpContext.Session.SetInt32("_UserId", userId);
            HttpContext.Session.SetString("_IsActive", "Active");

            try
            {
                return RedirectToAction(nameof(AddAvatar));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
                return NotFound();
            ViewBag.InvalidUser = "";
            if (!validUser)
            {
                ViewBag.InvalidUser = "Username is taken. Please choose an alternative.";
            }

            User userToEdit = (from u in _context.Users
                               where u.UserId == id
                               select u).ToList().FirstOrDefault();
            if (userToEdit == null)
                return NotFound();

            return View(userToEdit);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User userToEdit)
        {
            User originalUser = (from u in _context.Users
                                 where u.UserId == userToEdit.UserId
                                 select u).ToList().FirstOrDefault();

            if (originalUser == null)
                return NotFound();

            User ifUsernameExists = (from u in _context.Users
                                     where u.Username == userToEdit.Username
                                     select u).FirstOrDefault();
            if (ifUsernameExists != null)
            {
                validUser = false;
                return View();
            }
            validUser = true;
                

            _context.Entry(originalUser).CurrentValues.SetValues(userToEdit);
            _context.SaveChanges();
            try
            {
                return RedirectToAction("Details", new { id = userToEdit.UserId });
            }
            catch
            {
                return View(nameof(Index));
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            User userToDelete = (from u in _context.Users
                                 where u.UserId == id
                                 select u).ToList().FirstOrDefault();
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(User userToDelete)
        {
            if (userToDelete == null)
                return NotFound();

            _context.Remove(userToDelete);
            _context.SaveChanges();
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult SetRecommendations()
        {
            int? userId = HttpContext.Session.GetInt32("_UserId");

            if (userId == null)
                return NotFound();

            List<int?> categories = (from s in _context.Sets
                                     join sc in _context.UserSets on s.SetId equals sc.Set
                                     where sc.User == userId
                                     select s.SetCategory).ToList();

            List<Set> recomSets = (from s in _context.Sets
                                   where categories.Contains(s.SetCategory)
                                   select s).ToList();

            return View(recomSets);
        }

        public ActionResult UserSets()
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserId");
            List<Set> sets = (from us in _context.UserSets
                              join s in _context.Sets on us.Set equals s.SetId
                              where us.User == userId
                              select s).ToList() ?? new List<Set>();

            return View(sets);
        }

        public ActionResult AddUserSet()
        {
            return View(_context.Sets.ToList().GetRange(0, 50));
        }

        [HttpPost]
        public ActionResult AddUserSet(IFormCollection form)
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserId");
            int setId = Int32.Parse(form["Set"].ToString());

            UserSet userSet = new UserSet();
            userSet.User = userId;
            userSet.Set = setId;

            _context.UserSets.Add(userSet);
            _context.SaveChanges();

            return RedirectToAction("UserSets");
        }

        public ActionResult DeleteUserSet(int? id)
        {
            if (id == null)
                return NotFound();

            int userId = (int)HttpContext.Session.GetInt32("_UserId");

            int userSetToDelete = (from us in _context.UserSets
                                       join s in _context.Sets on us.Set equals s.SetId
                                       where us.Set == id && us.User == userId
                                       select new
                                       {
                                           usSetId = us.UseSetId
                                       }).FirstOrDefault().usSetId;

            UserSet toDelete = _context.UserSets.Find(userSetToDelete);
            return View(toDelete);
        }

        [HttpPost]
        public ActionResult DeleteUserSet(UserSet toDelete)
        {
            _context.UserSets.Remove(toDelete);
            _context.SaveChanges();
            return RedirectToAction("UserSets");
        }

        public ActionResult UserCustomSets()
        {
            int userId = (int)HttpContext.Session.GetInt32("_UserId");
            List<CustomSet> sets = (from cs in _context.CustomSets
                                    where cs.User == userId
                                    select cs).ToList();
            return View(sets);
        }
    }
}


