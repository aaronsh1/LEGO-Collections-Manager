using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LegoCollectionManager.Models;
using System.Linq;
using System.Collections.Generic;

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
        public RedirectToActionResult Login(string username)
        {
            User userToReturn = (from u in _context.Users
                                 where u.Username == username
                                 select u).ToList().FirstOrDefault();
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
        public ActionResult Create(User userToAdd)
        {
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
