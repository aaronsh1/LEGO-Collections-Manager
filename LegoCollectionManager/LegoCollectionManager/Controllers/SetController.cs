using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using LegoCollectionManager.Models;
using LegoCollectionManager.SetInformation; 
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using LegoCollectionManager.Utils;

namespace LegoCollectionManager.Controllers
{
    public class SetController : Controller
    {
        LegoCollectionDBContext _context = new LegoCollectionDBContext();

        QueryObjectUtil _queryObjectUtil = new QueryObjectUtil();

        public IEnumerable<int> tester()
        {
            List<int> tests = new List<int> { 1, 2, 3 };
            return tests;
        }

        // GET: SetController
        public ActionResult Index()
        {
            return View(_context.Sets);
        }

        public ActionResult SetRecommendations()
        {
            string SessionUserId = "_UserId";
            int? userId = HttpContext.Session.GetInt32(SessionUserId);

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

            SetInformationUtil util = new SetInformationUtil();

            SetInformation.SetInformation setInfo = util.GetSetInformation(id ?? 0);
            Set setModel = _context.Sets.Find(id);

            return View(SetInformationDTO.GetDTO(setModel, setInfo));
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
            ViewBag.Pieces = _queryObjectUtil.getAllPieces();
            ViewBag.Colours = _queryObjectUtil.getColours();
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
