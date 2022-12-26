using LettingAgentApp.Core.Contracts;
using LettingAgentApp.Core.DtoModels.Agent;
using LettingAgentApp.Core.DtoModels.House;
using LettingAgentApp.Infrastructure.Data;
using LettingAgentApp.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LettingAgentApp.Core.Services
{
    public class HouseService : IHouseService
    {
        private readonly ApplicationDbContext context;

        public HouseService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task<HousesQueryModel> All(string? category = null, string? searchTerm = null, HouseSorting sorting = HouseSorting.Newest, int currentPage = 1, int housesPerPage = 1)
        {
            var houseQuery = context.Houses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                houseQuery = context.Houses
                        .Where(h => h.Category.Name == category);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                houseQuery = houseQuery.Where(h =>
                           h.Title.ToLower().Contains(searchTerm.ToLower())
                        || h.Address.ToLower().Contains(searchTerm.ToLower())
                        || h.Description.ToLower().Contains(searchTerm.ToLower()));
            }

            houseQuery = sorting switch
            {
                HouseSorting.Price => houseQuery.OrderBy(h => h.PricePerMonth),
                HouseSorting.NotRentedFirst => houseQuery.OrderBy(h => h.RenterId != null)
                        .ThenByDescending(h => h.Id),
                _=> houseQuery.OrderByDescending(h => h.Id)
            };

            var houses = await houseQuery
                                .Skip((currentPage - 1) * housesPerPage)
                                .Take(housesPerPage)
                                .Select(h => new HouseServiceModel
                                {
                                    Id = h.Id,
                                    Title = h.Title,
                                    Address = h.Address,
                                    ImageUrl = h.ImageUrl,
                                    IsRented = h.RenterId != null,
                                    PricePerMonth = h.PricePerMonth
                                })
                                .ToListAsync();

            var totalHouses = houseQuery.Count();

            return new HousesQueryModel()
            {
                TotalHousesCount = totalHouses,
                Houses = houses
            };
        }

        public async Task<IEnumerable<HouseCategoryServiceModel>> AllCategories()
        {
            return await context.Categories
                .Select(c => new HouseCategoryServiceModel()
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> AllCategoriesNames()
        {
            return await context.Categories
                 .Select(c => c.Name)
                 .Distinct()
                 .ToListAsync();
        }

        public async Task<IEnumerable<HouseServiceModel>> AllHousesByAgentId(int agentId)
        {
            var houses = await context.Houses
              .Where(c => c.AgentId == agentId)
              .ToListAsync();

            return ProjectToModel(houses);
        }

        public async Task<IEnumerable<HouseServiceModel>> AllHousesByUserId(string userId)
        {
            var houses = await context.Houses
              .Where(c => c.RenterId == userId)
              .ToListAsync();

            return ProjectToModel(houses);
        }

        public async Task<bool> CategoryExists(int categoryId)
        {
            return await context.Categories
                .AnyAsync(c => c.Id == categoryId);
        }

        public async Task<int> Create(HouseFormModel model, int agentId)
        {
            var house = new House()
            {
                Address = model.Address,
                CategoryId = model.CategoryId,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                PricePerMonth = model.PricePerMonth,
                Title = model.Title,
                AgentId = agentId
            };

            await context.Houses.AddAsync(house);
            await context.SaveChangesAsync();

            return house.Id;
        }

        public async Task Edit(int houseId, string title,
            string address, string description, string imageUrl, decimal price, int categoryId)
        {
            var house = await context.Houses.FindAsync(houseId);

            house.Description = description;
            house.ImageUrl = imageUrl;
            house.PricePerMonth = price;
            house.Title = title;
            house.Address = address;
            house.CategoryId = categoryId;

            context.SaveChangesAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Houses.AnyAsync(h => h.Id == id);
        }

        public async Task<int> GetHouseCategoryId(int houseId)
        {
            return (await context.Houses.FindAsync(houseId)).CategoryId;
        }

        public async Task<bool> HasAgentWithId(int houseId, string currentUserId)
        {
            bool result = false;
            var house = await context.Houses
                .Where(h => h.Id == houseId)
                .Include(h => h.Agent)
                .FirstOrDefaultAsync();

            if (house.Agent != null && house.Agent.UserId == currentUserId)
            {
                result = true;
            }

            return result;
        }

        public async Task<HouseDetailsServiceModel> HouseDetailsById(int id)
        {
            return await context.Houses
                        .Where(h => h.Id == id)
                        .Select(h => new HouseDetailsServiceModel()
                        {
                            Id = h.Id,
                            Title = h.Title,
                            Address = h.Address,
                            Description = h.Description,
                            ImageUrl = h.ImageUrl,
                            PricePerMonth = h.PricePerMonth,
                            IsRented = h.RenterId != null,
                            Category = h.Category.Name,
                            Agent = new AgentServiceModel()
                            {
                                PhoneNumber = h.Agent.PhoneNumber,
                                Email = h.Agent.User.Email
                            }
                        })
                        .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<HouseIndexServiceModel>> LastThreeHouses()
        {
            return await context.Houses
                 .OrderByDescending(h => h.Id)
                 .Select(h => new HouseIndexServiceModel()
                 {
                     Id = h.Id,
                     ImageUrl = h.ImageUrl,
                     Title = h.Title
                 })
                 .Take(3)
                 .ToListAsync();
        }

        private List<HouseServiceModel> ProjectToModel(List<House> houses)
        {
            var resultHouses =  houses
               .Select(c => new HouseServiceModel()
               {
                   Address = c.Address,
                   Id = c.Id,
                   ImageUrl = c.ImageUrl,
                   IsRented = c.RenterId != null,
                   PricePerMonth = c.PricePerMonth,
                   Title = c.Title
               })
               .ToList();

            return resultHouses;
        }
    }
}
