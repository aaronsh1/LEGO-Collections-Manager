using LegoCollectionManager.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegoCollectionManager.Controllers
{
  public class PieceCategoryController : Controller
  {

    LegoCollectionDBContext _context = new LegoCollectionDBContext();
    private Dictionary<string, List<Piece>> dictionary;

    public PieceCategoryController()
    {
      dictionary = new Dictionary<string, List<Piece>>();
      fillDictionary();
      
    }

    private void fillDictionary()
    {
      var category = _context.PieceCategories.Select(x => x.PieceCategoryId).Distinct().ToList();
      foreach (var cat in category)
      {
        dictionary.Add(_context.PieceCategories.Where(x => x.PieceCategoryId == cat).First().PieceCategoryName, _context.Pieces.Where(x => x.PieceCategory == cat).ToList());
      }
    }

    public IActionResult Index()
    {
      ViewData["dictionary"] = dictionary[_context.PieceCategories.Select(x => x.PieceCategoryName).First()];
      ViewData["displayData"] = _context.PieceCategories.Select(x => x.PieceCategoryName).ToList();
      ViewData["category"] = _context.PieceCategories.Select(x => x.PieceCategoryName).First();
      return View();
    }

    public ViewResult Search(string value)
    {
      ViewData["dictionary"] = dictionary[value];
      ViewData["displayData"] = _context.PieceCategories.Select(x => x.PieceCategoryName).ToList();
      ViewData["category"] = _context.PieceCategories.Select(x => x.PieceCategoryName).First();
      return View();
    }
  }
}
