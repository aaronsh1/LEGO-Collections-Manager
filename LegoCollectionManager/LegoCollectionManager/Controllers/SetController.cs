using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LegoCollectionManager.Models;

namespace LegoCollectionManager.Controllers
{
    public class SetController : Controller
    {
        LegoCollectionDBContext _context = new LegoCollectionDBContext();
        // GET: SetController
        public ActionResult Index()
        {
            return View(_context.Sets);
        }

        public ActionResult SetRecommendations(int? userId)
        {
            if (userId == null)
                return NotFound();

            List<int> cats = (from s in _context.Sets
                              join sc in _context.UserSets on sc.UserSetId equals s.SetId
                              where sc.User == userId
                              select new
                              {
                                  Category = s.Category,
                              }).ToList();

            List<Set> recomSets = (from s in _context.Sets
                              where cats.contains(s.Category)
                              select s).ToList();
        }

        // GET: SetController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SetController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SetController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: SetController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SetController/Edit/5
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

        // GET: SetController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SetController/Delete/5
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
