namespace PetStore.Web.ViewModels.Categories
{
    public class ListAllCategoriesViewModel
    {
        public IEnumerable<ListCategoryViewModel> AllCategories { get; set; } = null!;

        public int PageCount { get; set; }

        public int AcrivePage { get; set; }
    }
}