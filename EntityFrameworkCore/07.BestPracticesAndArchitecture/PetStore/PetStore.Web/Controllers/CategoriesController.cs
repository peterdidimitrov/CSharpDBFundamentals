namespace PetStore.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using PetStore.Services.Data;
    using PetStore.Web.ViewModels.Categories;

    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            this._categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Error", "Home", new { errorMessage = "There was an error with validation of the entity!" });
            }

            await this._categoryService.CreateAsync(model);

            return this.RedirectToAction("All");
        }

        [HttpGet]
        public async Task<IActionResult> All(int page)
        {
            IEnumerable<ListCategoryViewModel> allCategories = await this._categoryService.GetAllWithPaginationAsync(page);

            int allCategoriesCount = allCategories.Count();

            ListAllCategoriesViewModel viewModel = new ListAllCategoriesViewModel()
            {
                AllCategories = allCategories,
                PageCount = (int)Math.Ceiling(allCategoriesCount / 20.0),
                AcrivePage = page
            };

            return this.View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            bool isValidCategory = await this._categoryService.ExistsAsync(id);

            if (!isValidCategory)
            {
                return this.RedirectToAction("All");
            }

            EditCategoryViewModel categoryToEdit = await this._categoryService
                .GetByIdAndPrepareForEditAsync(id);

            return this.View(categoryToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCategoryViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Error", "Home", new { errorMessage = "Validation error!" });
            }

            bool isValidCategory = await this._categoryService.ExistsAsync(model.Id);

            if (!isValidCategory)
            {
                return this.RedirectToAction("All");
            }

            await this._categoryService.EditCategoryAsync(model);

            return this.RedirectToAction("All");
        }

        //[HttpPost]
        //public IActionResult Delete(int id)
        //{
        //    return "";
        //}
    }
}
