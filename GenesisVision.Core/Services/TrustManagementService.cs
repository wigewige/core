using System.Linq;
using GenesisVision.Core.Data;
using GenesisVision.Core.Services.Interfaces;

namespace GenesisVision.Core.Services
{
    public class TrustManagementService : ITrustManagementService
    {
        private readonly ApplicationDbContext context;

        public TrustManagementService(ApplicationDbContext context)
        {
            this.context = context;
        }
    }
}
