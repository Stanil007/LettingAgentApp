using LettingAgentApp.Core.DtoModels.House;

namespace LettingAgentApp.Core.Contracts
{
    public interface IHouseService
    {
        Task<IEnumerable<HouseIndexServiceModel>> LastThreeHouses();

        Task<IEnumerable<HouseCategoryServiceModel>> AllCategories();

        Task<bool> CategoryExists(int categoryId);

        Task<int> Create(HouseFormModel model, int agentId);

        Task<HousesQueryModel> All(
            string? category = null,
            string? searchTerm = null,
            HouseSorting sorting = HouseSorting.Newest,
            int currentPage = 1,
            int housesPerPage = 1);

        Task<IEnumerable<string>> AllCategoriesNames();

        Task<IEnumerable<HouseServiceModel>> AllHousesByAgentId(int id);

        Task<IEnumerable<HouseServiceModel>> AllHousesByUserId(string userId);

        Task<HouseDetailsServiceModel> HouseDetailsById(int id);

        Task<bool> Exists(int id);

        Task Edit(int houseId, string title,
            string address, string description, string imageUrl, decimal price, int categoryId);

        Task<bool> HasAgentWithId(int houseId, string currentUserId);

        Task<int> GetHouseCategoryId(int houseId);

        Task Delete(int houseId);

        Task<bool> IsRented(int houseId);

        Task<bool> IsRentedByUserWithId(int houseId, string currentUserId);

        Task Rent(int houseId, string currentUserId);

        Task Leave(int houseId);
    }
}
