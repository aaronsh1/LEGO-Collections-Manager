using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using LegoCollectionManager.Models;
using LegoCollectionManager.SetInformation;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace LegoCollectionManager.Controllers
{
    public class UserSetController : Controller
    {
        LegoCollectionDBContext _context = new LegoCollectionDBContext();

        public IEnumerable<SelectListItem> getSetNumbers()
        {
            IEnumerable<SelectListItem> setNumbers = from s in _context.Sets
                                                     select new SelectListItem
                                                     {
                                                        Text = $"{s.SetName} ({s.SetId.ToString()})",
                                                        Value = s.SetId.ToString()
                                                      };
            return setNumbers;
        }

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
        public ActionResult Create(int? id)
        {
            if (id == null)
                return NotFound();

            ViewBag.SetNumbers = getSetNumbers();
            ViewBag.UserId = id;

            UserSet userSetToReturn = new UserSet();
            return View(userSetToReturn);
        }

        // GET: UserSet/AddSet
        public RedirectToActionResult AddSet(string Set)
        {
            
            System.Console.WriteLine(Set);
            System.Console.WriteLine(ViewBag.UserId.toString());

            return RedirectToAction("Index");
        }

        // POST: UserSet/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection form)
        {
            UserSet userSetToAdd = new UserSet();
            userSetToAdd.User = ViewBag.UserId;
            userSetToAdd.Set = Int32.Parse(form["Set"]);
            _context.UserSets.Add(userSetToAdd);
            _context.SaveChanges();
            
            return RedirectToAction("Details", "User", new { id = ViewBag.UserId });
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
