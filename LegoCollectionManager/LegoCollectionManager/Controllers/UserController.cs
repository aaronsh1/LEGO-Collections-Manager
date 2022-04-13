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

        // GET: UserController
        public ActionResult Index()
        {
            return View(_context.Sets);
        }

        // GET: UserController/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            User userToReturn = new User();
            userToReturn = _context.Users.Find(id);
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

        public ActionResult EditPieces(int? setId)
        {
            if (setId == null)
                return NotFound();

            ViewBag.SetId = setId;
            IEnumerable<SetPiece> pieces = (from sp in _context.SetPieces
                                            where sp.SetId == setId
                                            select sp);


            return View(pieces);
        }

        public ActionResult EditPiece(int? setPieceId)
        {
            ViewBag.Pieces = getAllPieces();
            ViewBag.Colours = getColours();
            SetPiece setPieceToEdit = _context.SetPieces.Find(setPieceId);
            return View(setPieceToEdit);
        }

        [HttpPost]
        public ActionResult EditPiece(SetPiece setPieceToEdit)
        {
            SetPiece originalSetPiece = (from s in _context.SetPieces
                                         where s.SetId == setPieceToEdit.SetId
                                         select s).ToList().FirstOrDefault();

            _context.Entry(originalSetPiece).CurrentValues.SetValues(setPieceToEdit);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: SetController/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            Set SetToDelete = (from s in _context.Sets
                               where s.SetId == id
                               select s).ToList().FirstOrDefault();
            return View();
        }

        // POST: SetController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Set setToDelete)
        {
            if (setToDelete == null)
                return NotFound();

            _context.Remove(setToDelete);
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
