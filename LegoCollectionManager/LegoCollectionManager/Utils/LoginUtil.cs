using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LegoCollectionManager.Models;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using LegoCollectionManager.Utils;

namespace LegoCollectionManager.Utils
{
    public class LoginUtil
    {
        LegoCollectionDBContext _context = new LegoCollectionDBContext();

        public bool login(string username, string password)
        {
            User userToReturn = (from u in _context.Users
                                 where u.Username == username
                                 select u).FirstOrDefault();

            if (userToReturn == null)
            {
                return false;
            }

            string saltFromDb = userToReturn.Salt;
            byte[] salt = Convert.FromBase64String(saltFromDb);
            string passwordFromDb = userToReturn.Password;



            string hashedPassword = System.Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));


            if (passwordFromDb != hashedPassword)
            {
                return false;
            }

            return true;
        }

        public bool register(string username, string password)
        {
            User ifExists = (from u in _context.Users
                             where u.Username == username
                             select u).FirstOrDefault();

            if (ifExists != null)
            {
                return false;
            }

            byte[] salt = new byte[128 / 8];

            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            string hashedPassword = System.Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            User userToAdd = new User();
            userToAdd.Username = username;
            userToAdd.Salt = System.Convert.ToBase64String(salt);
            userToAdd.Password = hashedPassword;

            _context.Users.Add(userToAdd);
            _context.SaveChanges();

            return true;
        }
    }
}
