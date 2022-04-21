using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using LegoCollectionManager.Models;
using LegoCollectionManager.SetInformation;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace LegoCollectionManager.Utils
{
    public class QueryObjectUtil
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

        public IEnumerable<SelectListItem> getAllPieces()
        {
            IEnumerable<SelectListItem> allPieces = from p in _context.Pieces
                                                    select new SelectListItem
                                                    {
                                                        Text = p.PieceName,
                                                        Value = p.PieceId.ToString()
                                                    };

            return allPieces;
        }

        public IEnumerable<SelectListItem> getColours()
        {
            IEnumerable<SelectListItem> colours = from c in _context.Colours
                                                  select new SelectListItem
                                                  {
                                                      Text = c.ColourName,
                                                      Value = c.ColourId.ToString()
                                                  };
            return colours;
        }

        public IEnumerable<Set> getUserSets(int id)
        {
            List<Set> userSets = (from s in _context.Sets
                                  join us in _context.UserSets on s.SetId equals us.UseSetId
                                  where us.User == id
                                  select s).ToList();
            return userSets;
        }

        public IEnumerable<Piece> getUserPieces(int id)
        {
            List<Piece> userPieces = (from p in _context.Pieces
                                      join up in _context.UserSparePieces on p.PieceId equals up.Piece
                                      where up.User == id
                                      select p).ToList();
            return userPieces;
        }

        public IEnumerable<SelectListItem> getAvatarList()
        {
            IEnumerable<SelectListItem> avatarList = (from a in _context.Avatars
                                                      select new SelectListItem
                                                      {
                                                          Text = a.Url,
                                                          Value = a.AvatarId.ToString()
                                                      });

            return avatarList;
        }

    }
}
