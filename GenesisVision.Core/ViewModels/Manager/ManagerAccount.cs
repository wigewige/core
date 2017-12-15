using System;

namespace GenesisVision.Core.ViewModels.Manager
{
    public class ManagerAccount
    {
        public Guid Id { get; set; }
        public Guid BrokerId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public long Login { get; set; }
    }
}
