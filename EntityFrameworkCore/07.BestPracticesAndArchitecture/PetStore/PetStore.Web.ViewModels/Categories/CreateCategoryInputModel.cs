using PetStore.Common;
using PetStore.Data.Models;
using PetStore.Services.Mapping;
using System.ComponentModel.DataAnnotations;

namespace PetStore.Web.ViewModels.Categories
{
    public class CreateCategoryInputModel : IMapTo<Category>
    {
        [Required]
        [StringLength(CategoryInputModelValidationConstants.NameMaxLength, MinimumLength = CategoryInputModelValidationConstants.NameMinLength, ErrorMessage = CategoryInputModelValidationConstants.NameLengthErrorMessage)]
        public string Name { get; set; } = null!;
    }
}
