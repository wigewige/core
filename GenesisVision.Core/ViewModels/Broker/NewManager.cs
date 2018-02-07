using System;
using System.ComponentModel.DataAnnotations;

namespace GenesisVision.Core.ViewModels.Broker
{
    public class NewManager
    {
        [Required]
        public Guid RequestId { get; set; }
        [Required]
        public string Login { get; set; }
    }
}
