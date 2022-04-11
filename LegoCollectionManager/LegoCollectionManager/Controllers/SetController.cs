using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using LegoCollectionManager.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace LegoCollectionManager.Controllers
{
    public class SetController : Controller
    {
        LegoCollectionDBContext _context = new LegoCollectionDBContext();

        public IEnumerable<SelectListItem> getSetCategories()
        {
            IEnumerable<SelectListItem> setCategories = from sc in _context.SetCategories
                                                        select new SelectListItem
                                                        {
                                                            Text = sc.SetCategoryName,
                                                            Value = sc.SetCategoryId.ToString()
                                                        };
            return setCategories;
        }

        public IEnumerable<SetPiece> getSetPieces(int id)
        {
            IEnumerable<SetPiece> setPieces = from sc in _context.SetPieces
                                              where sc.SetId == id
                                              select sc;

            return setPieces;
        }
        // GET: SetController
        public ActionResult Index()
        {
            return View(_context.Sets);
        }

        public ActionResult SetRecommendations(int? userId)
        {
            if (userId == null)
                return NotFound();

            List<int?> categories = (from s in _context.Sets
                              join sc in _context.UserSets on s.SetId equals sc.UseSetId 
                              where sc.User == userId
                              select s.SetCategory).ToList();

            List<Set> recomSets = (from s in _context.Sets
                              where categories.Contains(s.SetCategory)
                              select s).ToList();

            return View(recomSets);
        }

        // GET: SetController/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            Set setToReturn = new Set();
            setToReturn = _context.Sets.Find(id);
            return View(setToReturn);
        }

        // GET: SetController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SetController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Set setToAdd)
        {
            ViewBag["SetCategory"] = getSetCategories();

            _context.Sets.Add(setToAdd);
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

        public ActionResult AddPieces(int? id)
        {
            if (id == null)
                return NotFound();

            ViewBag.SetId = 100;
            IEnumerable<SetPiece> pieces = (from sp in _context.SetPieces
                                            where sp.SetId == id
                                            select sp);


            return View(pieces);
        }

        [HttpPost]
        public ActionResult AddPieces(int setId, FormCollection form)
        {
            SetPiece SetPieceToAdd = new SetPiece();
            SetPieceToAdd.Piece = Int32.Parse(form["Piece"]);
            SetPieceToAdd.SetId = setId;
            SetPieceToAdd.Amount = Int32.Parse(form["Amount"]);
            SetPieceToAdd.Colour = Int32.Parse(form["Colour"]);

            _context.SetPieces.Add(SetPieceToAdd);
            _context.SaveChanges();

            return View(setId);
        }

        // GET: SetController/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            Set setToEdit = (from s in _context.Sets
                             where s.SetId == id
                             select s).ToList().FirstOrDefault();
            return View(setToEdit);
        }

        // POST: SetController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Set setToEdit)
        {
            Set originalSet = (from s in _context.Sets
                             where s.SetId == setToEdit.SetId
                             select s).ToList().FirstOrDefault();

            if (originalSet == null)
                return NotFound();

            _context.Entry(originalSet).CurrentValues.SetValues(setToEdit);
            _context.SaveChanges();
            try
            {
                return RedirectToAction($"Details/{setToEdit.SetId}");
            }
            catch
            {
                return View(nameof(Index));
            }
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
