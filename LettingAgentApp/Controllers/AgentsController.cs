using LettingAgentApp.Core.Contracts;
using LettingAgentApp.Core.DtoModels.Agent;
using LettingAgentApp.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LettingAgentApp.Controllers
{
    [Authorize]
    public class AgentsController : Controller
    {
        private readonly IAgentService agentService;

        public AgentsController(IAgentService _agentService)
        {
            agentService = _agentService;
        }

        [HttpGet]
        public async Task<IActionResult> Become()
        {
            if (await agentService.ExistsById(User.Id()))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Become(BecomeAgentFormModel model)
        {
            var userId = User.Id();

            if (await agentService.ExistsById(userId))
            {
                return BadRequest();
            }

            if (await agentService.UserWithPhoneNumberExists(model.PhoneNumber))
            {
                ModelState.AddModelError(nameof(model.PhoneNumber), "Phone number already exists, please enter different one.");
            }

            if (await agentService.UserHasRents(userId))
            {
                ModelState.AddModelError("Error", "Sorry, but you cannot be agent if you have any rents.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            agentService.Create(userId,model.PhoneNumber);

            return RedirectToAction(nameof(HousesController.All), "Houses");
        }
    }
}
