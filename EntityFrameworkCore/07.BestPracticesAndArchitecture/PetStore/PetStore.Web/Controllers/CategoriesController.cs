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
                return this.RedirectToAction("Error", "Home", new {errorMessage = "There was an error with validation of the entity!"});
            }

            await this._categoryService.CreateAsync(model);

            return this.RedirectToAction("All");
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            IEnumerable<ListAllCategoriesViewModel> allCategories = await this._categoryService.GetAllAsync();

            return this.View(allCategories);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {

        }
    }
}
