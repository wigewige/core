using GenesisVision.Core.Helpers.Convertors;
using GenesisVision.Core.Services.Interfaces;
using GenesisVision.Core.ViewModels.Account;
using GenesisVision.DataModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GenesisVision.Core.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext context;

        public UserService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public ProfileFullViewModel GetUserProfile(Guid userId)
        {
            var user = context.Users
                              .Include(x => x.Profile)
                              .First(x => x.Id == userId);
            return user.ToProfileFull();
        }

        public void UpdateUserProfile(Guid userId, ProfileFullViewModel profile)
        {
            var user = context.Profiles.First(x => x.UserId == userId);

            user.Avatar = profile.Avatar;
            user.Address = profile.Address;
            user.Birthday = profile.Birthday;
            user.City = profile.City;
            user.Country = profile.Country;
            user.DocumentNumber = profile.DocumentNumber;
            user.DocumentType = profile.DocumentType;
            user.FirstName = profile.FirstName;
            user.Gender = profile.Gender;
            user.LastName = profile.LastName;
            user.MiddleName = profile.MiddleName;
            user.Phone = profile.Phone;

            context.SaveChanges();
        }
    }
}
