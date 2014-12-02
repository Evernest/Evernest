﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using EvernestBack;
using EvernestFront.Answers;
using EvernestFront.Contract;
using EvernestFront.Contract.Diff;
using EvernestFront.Projection;
using EvernestFront.Errors;

namespace EvernestFront
{
    public partial class User
    {
        public Int64 Id { get; private set; }

        public string Name { get { return UserContract.UserName; } }

        internal String SaltedPasswordHash { get { return UserContract.SaltedPasswordHash; } }

        internal byte[] PasswordSalt { get { return UserContract.PasswordSalt; } }

        internal string Key { get { return UserContract.Key; } } //base64 encoded int

        public List<Source> Sources { get{throw new NotImplementedException("User.Sources");} }
        // should return Sources, names, keys...?

        public List<KeyValuePair<long, AccessRights>> RelatedStreams
        {
            get { return UserContract.RelatedStreams.ToList(); }
        }


        private UserContract UserContract { get; set; }



        //change this ? Factor with Stream.nextId() ?
        private static Int64 _next;
        private static Int64 NextId() { _next++; return _next; }

        //TODO : refactor hashing and conversions between string/bytes




        private User(long userId, UserContract userContract)
        {
            Id = userId;
            UserContract = userContract;
        }

        static internal bool TryGetUser(long userId, out User user)
        {
            UserContract userContract;
            if (Projection.Projection.TryGetUserContract(userId, out userContract))
            {
                user = new User(userId, userContract);
                return true;
            }
            else
            {
                user = null;
                return false;
            }
        }

        static public GetUser GetUser(long userId)
        {
            User user;
            if (TryGetUser(userId, out user))
                return new GetUser(user);
            else
                return new GetUser(new UserIdDoesNotExist(userId));
        }


        static public IdentifyUser IdentifyUser(string userName, string password)
        {
            long userId;
            if (Projection.Projection.TryGetUserId(userName, out userId))
            {
                User user;
                if (TryGetUser(userId, out user))
                {
                    if (user.Identify(password))
                        return new IdentifyUser(userId);
                    else
                        return new IdentifyUser(new WrongPassword(userName, password));
                }
                else
                    throw new Exception("User.IdentifyUser");
                    //this should not happen since userId is read in the tables
                
            }
            else
                return new IdentifyUser(new UserNameDoesNotExist(userName));
        }



        static public AddUser AddUser(string name)
        {
            return AddUser(name, Keys.NewPassword());
        }

        static public AddUser AddUser(string name, string password)
        {
            if (!Projection.Projection.UserNameExists(name))
            {
                var id = NextId();

                var passwordSalt = System.Text.Encoding.ASCII.GetBytes(Keys.NewSalt());

                var passwordBytes = System.Text.Encoding.ASCII.GetBytes(password);
                var hmacMD5 = new HMACMD5(passwordSalt);
                var saltedHash = hmacMD5.ComputeHash(passwordBytes);
                var saltedPasswordHash = System.Text.Encoding.ASCII.GetString(saltedHash);

                var key = Keys.NewKey();

                var userContract = MakeUserContract.NewUserContract(name, saltedPasswordHash, passwordSalt, key);
                var userAdded = new UserAdded(id, userContract);

                Projection.Projection.HandleDiff(userAdded);
                //TODO: diff should be written in a stream, then sent back to be processed

                return new AddUser(name, id, key, password);
            }
            else
                return new AddUser(new UserNameTaken(name));
        }


        public SetPassword SetPassword(string password)
        {
            if (!(password.Equals(System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(password)))))
                return new SetPassword(new InvalidString(password));
            var passwordBytes = System.Text.Encoding.ASCII.GetBytes(password);
            var hmacMD5 = new HMACMD5(PasswordSalt);
            var saltedHash = hmacMD5.ComputeHash(passwordBytes);
            var saltedPasswordHash = System.Text.Encoding.ASCII.GetString(saltedHash);
            var passwordSet = new PasswordSet(Id, saltedPasswordHash);
            Projection.Projection.HandleDiff(passwordSet);
            return new SetPassword(Id, password);
        }




        private bool Identify(string password)
        {
            var hmacMD5 = new HMACMD5(PasswordSalt);
            var passwordBytes = System.Text.Encoding.ASCII.GetBytes(password);
            var saltedHash = System.Text.Encoding.ASCII.GetString(hmacMD5.ComputeHash(passwordBytes));
            return (SaltedPasswordHash.Equals(saltedHash));
        }

        

    }
}
