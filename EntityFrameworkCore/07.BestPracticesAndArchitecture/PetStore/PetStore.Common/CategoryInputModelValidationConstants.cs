using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetStore.Common
{
    public static class CategoryInputModelValidationConstants
    {
        public const int NameMinLength = 3;
        public const int NameMaxLength = 30;

        public const string NameLengthErrorMessage = "Category name must be between 3 and 30 characters long!";
    }
}
