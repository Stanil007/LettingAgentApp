using LettingAgentApp.Core.Contracts;
using LettingAgentApp.Core.DtoModels.House;
using LettingAgentApp.Infrastructure.Data.Entities;
using LettingAgentApp.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LettingAgentApp.Controllers
{
    [Authorize]
    public class HousesController : Controller
    {
        private IHouseService houseService;
        private IAgentService agentService;

        public HousesController(IHouseService _houseService,
                                IAgentService _agentService)
        {
            houseService = _houseService;
            agentService = _agentService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All([FromQuery] AllHousesQueryModel query)
        {
            var queryResult = await houseService.All(
                query.Category,
                query.SearchTerm,
                query.Sorting,
                query.CurrentPage,
                AllHousesQueryModel.HousesPerPage);

            query.TotalHousesCount = queryResult.TotalHousesCount;
            query.Houses = queryResult.Houses;

            var houseCategories = await houseService.AllCategoriesNames();
            query.Categories = houseCategories;

            return View(query);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            if (! await houseService.Exists(id))
            {
                return BadRequest();
            }

            var houseModel = houseService.HouseDetailsById(id);

            return View(houseModel);
        }

        public async Task<IActionResult> Mine()
        {
            IEnumerable<HouseServiceModel> myHouses;
            var userId = User.Id();

            if (await agentService.ExistsById(userId))
            {
                int agentId = await agentService.GetAgentId(userId);
                myHouses = await houseService.AllHousesByAgentId(agentId);
            }
            else
            {
                myHouses = await houseService.AllHousesByUserId(userId);
            }

            return View(myHouses);
        }

        [HttpGet]
       public async Task<IActionResult> Add()
        {
            if (!await agentService.ExistsById(User.Id()))
            {
                return RedirectToAction(nameof(AgentsController.Become), "Agents");
            }

            return View(new HouseFormModel
            {
                Categories = await houseService.AllCategories()
            }); 
        }

        [HttpPost]
        public async Task<IActionResult> Add(HouseFormModel model)
        {
            if(!await agentService.ExistsById(User.Id()))
            {
                return RedirectToAction(nameof(AgentsController.Become), "Agents");
            }

            if (!await houseService.CategoryExists(model.CategoryId))
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Category does not exist.");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await houseService.AllCategories();

                return View(model);
            }

            var agentId = await agentService.GetAgentId(User.Id());

            var newHouseId = houseService.Create(model,agentId);

            return RedirectToAction(nameof(Details), new {id = newHouseId});
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int houseId)
        {
            if (! await houseService.Exists(houseId))
            {
                return BadRequest();
            }

            if (! await houseService.HasAgentWithId(houseId, User.Id()))
            {
                return Unauthorized();
            }

            var house = await houseService.HouseDetailsById(houseId);
            var model = new HouseDetailsViewModel()
            {
                Address = house.Address,
                ImageUrl = house.ImageUrl,
                Title = house.Title
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(HouseDetailsServiceModel model)
        {

            if (!await houseService.Exists(model.Id))
            {
                return BadRequest();
            }

            if (!await houseService.HasAgentWithId(model.Id, User.Id()))
            {
                return Unauthorized();
            }
            await houseService.Delete(model.Id);

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (! await houseService.Exists(id))
            {
                return BadRequest();
            }

            if (! await houseService.HasAgentWithId(id, User.Id()))
            {
                return Unauthorized();
            }

            var house = await houseService.HouseDetailsById(id);
            var categoryId = await houseService.GetHouseCategoryId(id);

            var model = new HouseModel()
            {
                Id = id,
                Address = house.Address,
                CategoryId = categoryId,
                Description = house.Description,
                ImageUrl = house.ImageUrl,
                PricePerMonth = house.PricePerMonth,
                Title = house.Title,
                HouseCategories = await houseService.AllCategories()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, HouseFormModel model)
        {
            if (! await houseService.Exists(id))
            {
                return View();
            }

            if (! await houseService.HasAgentWithId(id, User.Id()))
            {
                return Unauthorized();
            }

            if (! await houseService.CategoryExists(model.CategoryId))
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Category does not exist.");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await houseService.AllCategories();
                return View(model);
            }

            await houseService.Edit(id, model.Title, model.Address,
                model.Description, model.ImageUrl, model.PricePerMonth, model.CategoryId);

            return RedirectToAction(nameof(Details), new { id = id});
        }

        [HttpPost]
        public async Task<IActionResult> Rent(int id)
        {
            if (! await houseService.Exists(id))
            {
                return BadRequest();
            }

            if (! await agentService.ExistsById(User.Id()))
            {
                return Unauthorized();
            }

            if (! await houseService.IsRented(id))
            {
                return BadRequest();
            }

            await houseService.Rent(id, User.Id());
            return RedirectToAction(nameof(Mine));
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
            if (! await houseService.Exists(id) || ! await houseService.IsRented(id))
            {
                return BadRequest();
            }

            if (! await houseService.IsRentedByUserWithId(id,User.Id()))
            {
                return Unauthorized();
            }

            await houseService.Leave(id);
            return RedirectToAction(nameof(Mine));
        }
    }
}
