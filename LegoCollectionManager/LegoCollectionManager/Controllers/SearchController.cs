using Microsoft.EntityFrameworkCore;
using LegoCollectionManager.Models;
using LegoCollectionManager.SetInformation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using LegoCollectionManager.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace LegoCollectionManager.Controllers
{
    public class SearchController : Controller
    {
        LegoCollectionDBContext _context = new LegoCollectionDBContext();

        public ActionResult Search(IFormCollection collection)
        {
            Console.WriteLine("Hello world from Search");

            List<int> setIds = new List<int>();

            for (int i = 0; i < 1000; i++)
            {
                StringValues x = "";
                collection.TryGetValue($"{i}", out x);

                string x2 = x.ToString().Trim();

                if (x2 == "")
                {
                    continue;
                }

                setIds.Add(Int32.Parse(x2));
                Console.WriteLine($"Value: '{x2}'");
            }

            SetInformationUtil util = new SetInformationUtil();
            Dictionary<string, int> searchPieces = new Dictionary<string, int>();
            List<SearchResultDTO> searchResults = new List<SearchResultDTO>();
            int searchPieceCount = 0;
            int searchThreshold = 0;

            //Get all of the pieces of all of the sets we search on
            foreach (var setId in setIds)
            {
                var setPieces = _context.SetPieces.Where((sp) => sp.SetId == setId);

                foreach (var setPiece in setPieces)
                {
                    string key = $"{setPiece.Piece}#{setPiece.Colour}";

                    if (!searchPieces.ContainsKey(key))
                    {
                        //If the piece is a new piece, then we add to list of search pieces
                        searchPieces.Add(key, setPiece.Amount ?? 0);

                    } else
                    {
                        //If the piece is already in search pieces, then we update the count
                        searchPieces[key] += setPiece.Amount ?? 0;
                    }

                    //Count how many pieces we have
                    searchPieceCount += setPiece.Amount ?? 0;
                }
            }

            //Calculate the piece threshold (2 x searchPieceCount)
            //The amount needed for our search pieces to match (at best) 50% of the set
            searchThreshold = (int)(2 * searchPieceCount);


            //Get all of the sets where the piece count is above the threshold
            //We can exclude all sets where we do not have enough pieces to at least give the possibility of a 50% match.
            List<Set> searchableSets = _context.Sets.Where(s => s.PiecesAmount <= searchThreshold && s.PiecesAmount > 50).Include(s => s.SetPieces).OrderBy(s => s.PiecesAmount).ToList();

            //We loop through our Searchable Sets and compare the pieces to determin a percentage match
            foreach (var set in searchableSets)
            {
                //Console.WriteLine("SEARCHING SET: " + set.SetName);

                int unMatchedCount = 0;
                List<SetPiece> missingPieces = new List<SetPiece>();
                bool failed = false;
                List<string> piecesChecked = new List<string>();
                double matchPercentage = 0;

                var setPieces = set.SetPieces;

                foreach(var setPiece in setPieces)
                {
                    

                    string key = $"{setPiece.Piece}#{setPiece.Colour}";

                    //The piece is completely missing
                    if (!searchPieces.ContainsKey(key))
                    {
                        unMatchedCount += setPiece.Amount ?? 0;

                        var temp = new SetPiece();
                        temp.SetId = -1;
                        temp.Piece = setPiece.Piece;
                        temp.Colour = setPiece.Colour;
                        temp.Amount = setPiece.Amount;

                        missingPieces.Add(temp);
                    }
                    //Not enough of that piece
                    else if (searchPieces[key] < setPiece.Amount)
                    {
                        int diff = setPiece.Amount ?? 0 - searchPieces[key];
                        unMatchedCount += diff;

                        var temp = new SetPiece();
                        temp.SetId = -1;
                        temp.Piece = setPiece.Piece;
                        temp.Colour = setPiece.Colour;
                        temp.Amount = diff;

                        missingPieces.Add(temp);
                    }

                    matchPercentage = ((set.PiecesAmount - unMatchedCount) ?? 0) / (double)(set.PiecesAmount ?? 1 /* Avoid division by 0 */) * 100.0;

                    //Check if it is still possible to reach 50% match
                    //If not, then we can exit early since this set is already a lost cause
                    if (matchPercentage < 50)
                    {
                        failed = true;
                        break;
                    }

                    piecesChecked.Add(key);
                }

                //Add to search results only if it did not fail
                if (failed)
                {
                    continue;
                }

                //Add to search results
                var searchRes = new SearchResultDTO();
                searchRes.setId = set.SetId;
                searchRes.setDTO = SetInformationDTO.GetDTO(set, util.GetSetInformation(set.SetId));
                searchRes.matchPercentage = matchPercentage;
                searchRes.missingPieces = missingPieces;

                Console.WriteLine($"Match found: {set.SetName} [{set.SetId}] ({(int)searchRes.matchPercentage}) unmatch: {unMatchedCount} setPieceAmount: {set.PiecesAmount}");

                searchResults.Add(searchRes);

                //Check if we have enough search results
                if (searchResults.Count >= 50)
                {
                    break;
                }
            }

            searchResults.Sort((a, b) => Math.Sign(b.matchPercentage - a.matchPercentage));

            return View(searchResults);
        }

        public ActionResult Index()
        {
            //60321, 6032, 75157
            List<SetInformationDTO> DTOs = new List<SetInformationDTO>();

            DTOs.Add(getSetInformation(75159));
            DTOs.Add(getSetInformation(6032));
            DTOs.Add(getSetInformation(75288));

            return View(DTOs);
        }

        private SetInformationDTO getSetInformation(int? id)
        {
            if (id == null)
                return null;

            SetInformationUtil util = new SetInformationUtil();

            SetInformation.SetInformation setInfo = util.GetSetInformation(id ?? 0);
            var setModel = _context.Sets.Find(id ?? 0);
            _context.Entry(setModel).Reference(s => s.SetCategoryNavigation).Load();

            return SetInformationDTO.GetDTO(setModel, setInfo);
        }
    }
}
