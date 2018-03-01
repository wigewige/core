using GenesisVision.Common.Models;
using GenesisVision.Core.ViewModels.Account;
using System;

namespace GenesisVision.Core.Services.Interfaces
{
    public interface IUserService
    {
        OperationResult<ProfileFullViewModel> GetUserProfileFull(Guid userId);

        OperationResult<ProfilePublicViewModel> GetUserPublicProfile(Guid userId);

        OperationResult UpdateUserProfile(Guid userId, UpdateProfileViewModel profile);
    }
}
