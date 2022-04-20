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
    public class CustomSetController : Controller
    {
        LegoCollectionDBContext _context = new LegoCollectionDBContext();
        // GET: CustomSetController
        public ActionResult Index()
        {
            return View(_context.CustomSets.ToList());
        }

        // GET: CustomSetController/Details/5
        public ActionResult Details(int id)
        {
            CustomSet customSetToView = _context.CustomSets.Find(id);
            return View(customSetToView);
        }

        // GET: CustomSetController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomSetController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection form)
        {
            CustomSet customSetToAdd = new CustomSet();
            customSetToAdd.CustomSetName = form["CustomSetName"].ToString();
            customSetToAdd.User = (int)HttpContext.Session.GetInt32("_UserId");
            customSetToAdd.PiecesAmount = Int32.Parse(form["PiecesAmount"].ToString());

            _context.CustomSets.Add(customSetToAdd);
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

        // GET: CustomSetController/Edit/5
        public ActionResult Edit(int? id)
        {
            if(id == null)
                return NotFound();
            CustomSet customSetToEdit = _context.CustomSets.Find(id);
            if (customSetToEdit == null)
                return NotFound();
            if(customSetToEdit.User != (int)HttpContext.Session.GetInt32("_UserId"))
                return RedirectToAction("Details", new { id = id });

            return View(customSetToEdit);
        }

        // POST: CustomSetController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection form)
        {
            CustomSet originalSet = _context.CustomSets.Find(id);
            CustomSet customSetToUpdate = new CustomSet();
            customSetToUpdate.CustomSetName = form["Name"].ToString();
            customSetToUpdate.PiecesAmount = Int32.Parse(form["Pieces Amount"].ToString());

            _context.Entry(originalSet).CurrentValues.SetValues(customSetToUpdate);
            _context.SaveChanges();

            try
            {
                return RedirectToAction("Details", new { id = id });
            }
            catch
            {
                return View();
            }
        }

        // GET: CustomSetController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CustomSetController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
    }
}
