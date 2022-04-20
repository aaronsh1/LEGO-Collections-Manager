using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LegoCollectionManager.Models;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace LegoCollectionManager.Controllers
{
    public class UserController : Controller
    {
        LegoCollectionDBContext _context = new LegoCollectionDBContext();

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

        // GET: UserController
        public ActionResult Index()
        {
            return View(_context.Users);
        }

        // GET: UserController/Login
        public ActionResult Login(FormCollection form)
        {
            ViewBag.IncorrectPassword = "";
            User userToReturn = (from u in _context.Users
                                 where u.Username == form["username"]
                                 select u).ToList().FirstOrDefault();

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
                ViewBag.IncorrectPassword = "Incorrect Username or Password. Please Try Again.";
                return View("Index");
            }

            string SessionUserId = "_UserId";

            HttpContext.Session.SetInt32(SessionUserId, userToReturn.UserId);

            return RedirectToAction("Details", new { id = userToReturn.UserId });
        }

        // GET: UserController/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            ViewBag["UserSets"] = getUserSets((int) id);
            ViewBag["UserSparePieces"] = getUserPieces((int)id);

            User userToReturn = _context.Users.Find(id);
            return View(userToReturn);
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection form)
        {
            string username = form["username"];
            string password = form["password"];

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

            try
            {
                return RedirectToAction(nameof(Index));
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

            User userToEdit = (from u in _context.Users
                               where u.UserId == id
                               select u).ToList().FirstOrDefault();
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

            _context.Entry(originalUser).CurrentValues.SetValues(userToEdit);
            _context.SaveChanges();
            try
            {
                return RedirectToAction($"Details/{userToEdit.UserId}");
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
