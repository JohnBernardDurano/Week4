﻿using ENTJAVA_Week3.Models.DB;
using ENTJAVA_Week3.Models.ViewModel;


namespace ENTJAVA_Week3.Models.EntityManager
{
    public class UserManager
    {
        public void AddUserAccount(UserModel user)
        {
            using (MyDBContext db = new MyDBContext())
            {
                //Add checking here if login exists

                SystemUsers newSysUser = new SystemUsers
                {
                    LoginName = user.LoginName,
                    CreatedBy = 1,
                    PasswordEncryptedText = user.Password, //this has to be encrypted
                    CreatedDateTime = DateTime.Now,
                    ModifiedBy = 1,
                    ModifiedDateTime = DateTime.Now
                };

                db.SystemUsers.Add(newSysUser);
                db.SaveChanges();

                int newUserId = db.SystemUsers.First(u => u.LoginName == newSysUser.LoginName).UserID;

                Users newUser = new Users
                {
                    UserID = newUserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Gender = "1",
                    CreatedBy = 1,
                    CreatedDateTime = DateTime.Now,
                    ModifiedBy = 1,
                    ModifiedDateTime = DateTime.Now
                };

                db.Users.Add(newUser);
                db.SaveChanges();
            }
        }

        public void UpdateUserAccount(UserModel user)
        {
            using (MyDBContext db = new MyDBContext())
            {
                // Check if a user with the given login name already exists
                SystemUsers existingSysUser = db.SystemUsers.FirstOrDefault(u => u.LoginName == user.LoginName);
                Users existingUser = db.Users.FirstOrDefault(u => u.UserID == existingSysUser.UserID);

                if (existingSysUser != null && existingUser != null)
                {
                    // Update the existing user
                    existingSysUser.ModifiedBy = 1; // This has to be updated
                    existingSysUser.ModifiedDateTime = DateTime.Now;


                    // You can also update other properties of the user as needed
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Gender = user.Gender;

                    db.SaveChanges();
                }
                else
                {
                    // Add a new user since the user doesn't exist
                    SystemUsers newSysUser = new SystemUsers
                    {
                        LoginName = user.LoginName,
                        CreatedBy = 1,
                        PasswordEncryptedText = user.Password, // Update this to handle encryption
                        CreatedDateTime = DateTime.Now,
                        ModifiedBy = 1,
                        ModifiedDateTime = DateTime.Now
                    };

                    db.SystemUsers.Add(newSysUser);
                    db.SaveChanges();

                    int newUserId = newSysUser.UserID;

                    Users newUser = new Users
                    {
                        UserID = newUserId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Gender = "1",
                        CreatedBy = 1,
                        CreatedDateTime = DateTime.Now,
                        ModifiedBy = 1,
                        ModifiedDateTime = DateTime.Now
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();
                }
            }

        }

        public UsersModel GetAllUsers()
        {
            UsersModel list = new UsersModel();

            using (MyDBContext db = new MyDBContext())
            {
                //var users = from u in db.Users
                //            join us in db.SystemUsers
                //        on u.UserID equals us.UserID
                //            join sg in db.SYSGenres
                //        on u.UserID equals sg.GenreID
                //            //select new { u, us};
                //            select new { u, us, sg };

                var users = from u in db.SystemUsers
                            join us in db.SYSGenres
                        on u.LoginName equals us.LoginName
                            join sg in db.Users
                        on u.UserID equals sg.UserID
                            select new { u, us, sg};

                list.Users = users.Select(records => new UserModel()
                {
                    LoginName = records.us.LoginName,
                    FirstName = records.sg.FirstName,
                    LastName = records.sg.LastName,
                    Gender = records.sg.Gender,
                    CreatedBy = records.u.CreatedBy,
                    Genre = records.us.GenreType
                }).ToList();
            }

            return list;
        }

        public bool IsLoginNameExist(string loginName)
        {
            using (MyDBContext db = new MyDBContext())
            {
                return db.SystemUsers.Where(u => u.LoginName.Equals(loginName)).Any();
            }
        }

    }
}
