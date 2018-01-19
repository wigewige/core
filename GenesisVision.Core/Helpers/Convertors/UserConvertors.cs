using GenesisVision.Core.ViewModels.Account;
using GenesisVision.DataModel.Models;

namespace GenesisVision.Core.Helpers.Convertors
{
    public static partial class Convertors
    {

        public static ProfileShortViewModel ToProfileShort(this ApplicationUser user)
        {
            return new ProfileShortViewModel
                   {
                       Email = user.Email,
                       Balance = user.Wallet?.Amount ?? 0
                   };
        }

        public static ProfileFullViewModel ToProfileFull(this ApplicationUser user)
        {
            var model = new ProfileFullViewModel
                        {
                            Email = user.Email,
                            Balance = user.Wallet?.Amount ?? 0
                        };
            if (user.Profile != null)
            {
                model.Avatar = user.Profile.Avatar;
                model.Address = user.Profile.Address;
                model.Birthday = user.Profile.Birthday;
                model.City = user.Profile.City;
                model.Country = user.Profile.Country;
                model.DocumentNumber = user.Profile.DocumentNumber;
                model.DocumentType = user.Profile.DocumentType;
                model.FirstName = user.Profile.FirstName;
                model.Gender = user.Profile.Gender;
                model.LastName = user.Profile.LastName;
                model.MiddleName = user.Profile.MiddleName;
                model.Phone = user.Profile.Phone;
            }
            return model;
        }
    }
}
