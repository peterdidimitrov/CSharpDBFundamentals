using Microsoft.EntityFrameworkCore;
using PetStore.Data.Common.Repos;
using PetStore.Data.Models;
using PetStore.Services.Mapping;
using PetStore.Web.ViewModels.Categories;

namespace PetStore.Services.Data
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _repository;
        //private readonly IMapper _mapper;
        public CategoryService(IRepository<Category> repository)
        {
            this._repository = repository;
            //this._mapper = mapper;
        }
        public async Task CreateAsync(CreateCategoryInputModel inputModel)
        {
            Category category = AutoMapperConfig.MapperInstance.Map<Category>(inputModel);
            await this._repository.AddAsync(category);
            await this._repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ListCategoryViewModel>> GetAllAsync()
        {
            return await this._repository
                .AllAsNoTracking()
                .To<ListCategoryViewModel>()
                .ToArrayAsync();
        }

        public async Task<IEnumerable<ListCategoryViewModel>> GetAllWithPaginationAsync(int pageNumber)
        {
            return await this._repository
                .AllAsNoTracking()
                .Skip((pageNumber - 1) * 20)
                .Take(20)
                .To<ListCategoryViewModel>()
                .ToArrayAsync();
        }

        public async Task<EditCategoryViewModel> GetByIdAndPrepareForEditAsync(int id)
        {
            Category categoryToEdit = await this._repository
                .AllAsNoTracking()
                .FirstAsync(c => c.Id == id);

            return AutoMapperConfig
                .MapperInstance
                .Map<EditCategoryViewModel>(categoryToEdit);
        }
        public async Task EditCategoryAsync(EditCategoryViewModel inputModel)
        {
            Category categoryToUpdate = await this._repository
                .All()
                .FirstAsync(c => c.Id == inputModel.Id);

            categoryToUpdate.Name = inputModel.Name;
            this._repository.Update(categoryToUpdate);
            await this._repository.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id) => await this._repository.AllAsNoTracking().AnyAsync(c => c.Id == id);

    }
}
