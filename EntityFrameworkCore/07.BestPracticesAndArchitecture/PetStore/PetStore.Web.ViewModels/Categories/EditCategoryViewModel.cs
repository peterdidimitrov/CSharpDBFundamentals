namespace PetStore.Web.ViewModels.Categories
{
    using System.ComponentModel.DataAnnotations;
    
    using Common;
    using Data.Models;
    using Services.Mapping;

    public class EditCategoryViewModel : IMapFrom<Category>, IMapTo<Category>
    {
        public int Id { get; set; }

        [Required]
        [StringLength(CategoryInputModelValidationConstants.NameMaxLength,
            MinimumLength = CategoryInputModelValidationConstants.NameMinLength, ErrorMessage = CategoryInputModelValidationConstants.NameLengthErrorMessage)]
        public string Name { get; set; } = null!;
    }
}
