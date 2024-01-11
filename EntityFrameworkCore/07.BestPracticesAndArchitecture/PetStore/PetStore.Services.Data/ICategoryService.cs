using PetStore.Web.ViewModels.Categories;

namespace PetStore.Services.Data
{
    public interface ICategoryService
    {
        Task CreateAsync(CreateCategoryInputModel inputModel);

        Task<IEnumerable<ListCategoryViewModel>> GetAllAsync();

        Task<IEnumerable<ListCategoryViewModel>> GetAllWithPaginationAsync(int pageNumber);

        Task<EditCategoryViewModel> GetByIdAndPrepareForEditAsync(int id);

        Task EditCategoryAsync(EditCategoryViewModel inputModel);

        Task<bool> ExistsAsync(int id);
    }
}
