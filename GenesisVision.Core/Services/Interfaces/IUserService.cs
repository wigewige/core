using GenesisVision.Core.ViewModels.Account;
using GenesisVision.DataModel.Models;
using System;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface IUserService
    {
        ProfileFullViewModel GetUserProfile(Guid userId);

        void UpdateUserProfile(Guid userId, ProfileFullViewModel profile);
    }
}
