using System;

namespace GenesisVision.Core.ViewModels.Broker
{
    public class Broker
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
