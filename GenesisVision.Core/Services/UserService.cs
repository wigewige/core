using GenesisVision.Common.Models;
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

        public OperationResult<ProfileFullViewModel> GetUserProfileFull(Guid userId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var user = context.Users
                                  .Include(x => x.Profile)
                                  .Include(x => x.Wallets)
                                  .First(x => x.Id == userId);
                return user.ToProfileFull();
            });
        }

        public OperationResult<ProfilePublicViewModel> GetUserPublicProfile(Guid userId)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var user = context.Users
                                  .Include(x => x.Profile)
                                  .First(x => x.Id == userId);
                return user.ToProfilePublic();
            });
        }

        public OperationResult UpdateUserProfile(Guid userId, UpdateProfileViewModel profile)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                if (context.Profiles.Any(x => x.UserName == profile.UserName && x.UserId != userId))
                    throw new Exception("Username already exists");

                var user = context.Profiles.First(x => x.UserId == userId);

                user.UserName = profile.UserName;
                user.Avatar = profile.Avatar;
                user.Address = profile.Address;
                if (profile.Birthday.HasValue)
                    user.Birthday = profile.Birthday.Value;
                user.City = profile.City;
                user.Country = profile.Country;
                user.DocumentNumber = profile.DocumentNumber;
                user.DocumentType = profile.DocumentType;
                user.FirstName = profile.FirstName;
                if (profile.Gender.HasValue)
                    user.Gender = profile.Gender.Value;
                user.LastName = profile.LastName;
                user.MiddleName = profile.MiddleName;
                user.Phone = profile.Phone;

                context.SaveChanges();
            });
        }
    }
}
