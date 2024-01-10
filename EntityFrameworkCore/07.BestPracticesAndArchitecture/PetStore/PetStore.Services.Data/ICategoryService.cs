using PetStore.Web.ViewModels.Categories;

namespace PetStore.Services.Data
{
    public interface ICategoryService
    {
        Task CreateAsync(CreateCategoryInputModel inputModel);

        Task<IEnumerable<ListAllCategoriesViewModel>> GetAllAsync();

        Task<EditCategoryViewModel> GetByIdAndPrepareFromEditAsync(int id);
    }
}
