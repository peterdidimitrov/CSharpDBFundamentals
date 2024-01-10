using AutoMapper;
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

        public async Task<IEnumerable<ListAllCategoriesViewModel>> GetAllAsync()
        {
            return await this._repository
                .AllAsNoTracking()
                .To<ListAllCategoriesViewModel>()
                .ToArrayAsync();
        }

        public async Task<EditCategoryViewModel> GetByIdAndPrepareFromEditAsync(int id)
        {
            Category categoryToEdit = await this._repository
                .AllAsNoTracking()
                .FirstAsync(c => c.Id == id);

            return AutoMapperConfig.MapperInstance.Map<EditCategoryViewModel>(categoryToEdit);
        }
    }
}
