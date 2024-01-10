using PetStore.Data.Models;
using PetStore.Services.Mapping;

namespace PetStore.Web.ViewModels.Categories
{
    public class ListAllCategoriesViewModel : IMapFrom<Category>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
