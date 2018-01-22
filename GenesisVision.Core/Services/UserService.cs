using GenesisVision.Core.Helpers.Convertors;
using GenesisVision.Core.Models;
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

        public OperationResult<ProfileShortViewModel> GetUserProfileShort(Guid userId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var user = context.Users
                                  .Include(x => x.Wallet)
                                  .First(x => x.Id == userId);
                return user.ToProfileShort();
            });
        }

        public OperationResult<ProfileFullViewModel> GetUserProfileFull(Guid userId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var user = context.Users
                                  .Include(x => x.Profile)
                                  .Include(x => x.Wallet)
                                  .First(x => x.Id == userId);
                return user.ToProfileFull();
            });
        }

        public OperationResult UpdateUserProfile(Guid userId, ProfileFullViewModel profile)
        {
            return InvokeOperations.InvokeOperation(() =>
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
            });
        }
    }
}
