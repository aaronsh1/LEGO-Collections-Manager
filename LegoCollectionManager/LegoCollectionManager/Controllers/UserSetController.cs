using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LegoCollectionManager.Models;
using System.Linq;
using System.Collections.Generic;

namespace LegoCollectionManager.Controllers
{
    public class UserSetController : Controller
    {
        LegoCollectionDBContext _context = new LegoCollectionDBContext();

        // GET: UserSet
        public ActionResult Index()
        {
            return View();
        }

        // GET: UserSet/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            UserSet  userSetToReturn = _context.UserSets.Find(id);
            return View(userSetToReturn);
        }

        // GET: UserSet/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserSet/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserSet userSetToAdd)
        {
            _context.UserSets.Add(userSetToAdd);
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

        // GET: UserSet/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            UserSet userSetToEdit = (from u in _context.UserSets
                                  where u.User == id
                                  select u).ToList().FirstOrDefault();
            return View(userSetToEdit);
        }

        // POST: UserSet/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserSet/Delete/5
        public ActionResult Delete(int? id)
        {

            if (id == null)
                return NotFound();

            UserSet userSetToDelete = (from u in _context.UserSets
                                       where u.User == id
                                       select u).ToList().FirstOrDefault();
            return View();
        }

        // POST: UserSet/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(UserSet userSetToDelete)
        {
            if (userSetToDelete == null)
                return NotFound();

            _context.Remove(userSetToDelete);
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
