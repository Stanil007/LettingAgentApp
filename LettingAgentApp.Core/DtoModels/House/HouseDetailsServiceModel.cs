using LettingAgentApp.Core.DtoModels.Agent;
using System.Security.Principal;

namespace LettingAgentApp.Core.DtoModels.House
{
    public class HouseDetailsServiceModel : HouseServiceModel
    {
        public string Description { get; init; }

        public string Category { get; init; }

        public AgentServiceModel Agent { get; init; }
    }
}
