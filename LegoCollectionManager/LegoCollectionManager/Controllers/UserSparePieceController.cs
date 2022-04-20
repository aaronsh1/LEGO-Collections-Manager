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
    public class UserSparePieceController : Controller
    {
        LegoCollectionDBContext _context = new LegoCollectionDBContext();

        // GET: UserSparePieceController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UserSparePieceController/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            UserSparePiece userSparePieceToReturn = _context.UserSparePieces.Find(id);
            return View(userSparePieceToReturn);
        }

        // GET: UserSparePieceController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserSparePieceController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserSparePiece userSparePieceToAdd)
        {
            _context.UserSparePieces.Add(userSparePieceToAdd);
            _context.SaveChanges();

            return View();
        }

        // GET: UserSparePieceController/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            UserSparePiece userSparePieceToEdit = (from u in _context.UserSparePieces
                                                   where u.User == id
                                                   select u).ToList().FirstOrDefault();
            return View(userSparePieceToEdit);
    }

        // POST: UserSparePieceController/Edit/5
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

        // GET: UserSparePieceController/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            UserSparePiece userSparePieceToDelete = (from u in _context.UserSparePieces
                                                     where u.User == id
                                                     select u).ToList().FirstOrDefault();
            return View();
        }

        // POST: UserSparePieceController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(UserSparePiece? userSparePieceToDelete)
        {
            if (userSparePieceToDelete == null)
                return NotFound();

            _context.Remove(userSparePieceToDelete);
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
