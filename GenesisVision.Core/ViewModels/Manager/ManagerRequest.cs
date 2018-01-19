using System;

namespace GenesisVision.Core.ViewModels.Manager
{
    public class ManagerRequest
    {
        public Guid UserId { get; set; }
        public Guid RequestId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Currency { get; set; }
        public string Password { get; set; }
    }
}
