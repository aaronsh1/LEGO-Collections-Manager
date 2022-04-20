using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LegoCollectionManager.Models;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace LegoCollectionManager.Controllers
{
    public class UserController : Controller
    {
        LegoCollectionDBContext _context = new LegoCollectionDBContext();
        bool validUser = true;
        bool validPassword = true;

        public IEnumerable<Set> getUserSets(int id)
        {
            List<Set> userSets = (from s in _context.Sets
                                  join us in _context.UserSets on s.SetId equals us.UseSetId
                                  where us.User == id
                                  select s).ToList();
            return userSets;
        }

        public IEnumerable<Piece> getUserPieces(int id)
        {
            List<Piece> userPieces = (from p in _context.Pieces
                                      join up in _context.UserSparePieces on p.PieceId equals up.Piece
                                      where up.User == id
                                      select p).ToList();
            return userPieces;
        }

        public IEnumerable<SelectListItem> getAvatarList()
        {
            IEnumerable<SelectListItem> avatarList = (from a in _context.Avatars
                                                      select new SelectListItem
                                                      {
                                                          Text = a.Url,
                                                          Value = a.AvatarId.ToString()
                                                      });

            return avatarList;
        }

        // GET: UserController
        public ActionResult Index()
        {
            ViewData["IncorrectPassword"] = "";

            return View();
        }

        // GET: UserController/Login
        public ActionResult Login(IFormCollection form)
        {
            ViewData["IncorrectPassword"] = "";
            User userToReturn = (from u in _context.Users
                                 where u.Username == form["username"].ToString()
                                 select u).FirstOrDefault();

            if (userToReturn == null)
            {
                ViewData["IncorrectPassword"] = "Incorrect Username or Password. Please Try Again.";
                return View("Index");
            }

            string password = form["password"];
            string saltFromDb = userToReturn.Salt;
            byte[] salt = System.Text.Encoding.ASCII.GetBytes(saltFromDb);
            string passwordFromDb = userToReturn.Password;



            string hashedPassword = System.Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));


            if (passwordFromDb != hashedPassword)
            {
                validPassword = false;
                return View("Index");
            }
            validPassword = true;

            string SessionUserId = "_UserId";

            HttpContext.Session.SetInt32(SessionUserId, userToReturn.UserId);

            return RedirectToAction("Details", new { id = userToReturn.UserId });
        }

        // GET: UserController/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            ViewBag.UserSets = getUserSets((int) id);
            ViewBag.UserSparePieces = getUserPieces((int)id);

            User userToReturn = _context.Users.Find(id);
            return View(userToReturn);
        }

        public ActionResult AddAvatar()
        {
            ViewBag.Avatars = getAvatarList();
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

            return RedirectToAction("Details", new { id = userToReturn });
        }


        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection form)
        {
            ViewBag.InvalidUser = "";
            if (!validUser)
            {
                ViewBag.InvalidUser = "Username is taken. Please choose an alternative.";
            }


            string username = form["username"];
            string password = form["password"];

            User ifExists = (from u in _context.Users
                             where u.Username == username
                             select u).FirstOrDefault();

            if(ifExists != null)
            {
                validUser = false;
                return View();
            }
            validUser = true;

            byte[] salt = new byte[128 / 8];

            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            string hashedPassword = System.Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            User userToAdd = new User();
            userToAdd.Username = username;
            userToAdd.Salt = System.Convert.ToBase64String(salt);
            userToAdd.Password = hashedPassword;

            _context.Users.Add(userToAdd);
            _context.SaveChanges();

            string SessionUserId = "_UserId";

            int userId = (from u in _context.Users
                             where u.Username == username
                             select u).FirstOrDefault().UserId;

            HttpContext.Session.SetInt32(SessionUserId, userId);

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
    }
}
