using System.ComponentModel.DataAnnotations;

namespace Medicines.DataProcessor.ImportDtos
{
    public class PatientImportDTO
    {
        [Required]
        [MaxLength(100)]
        [MinLength(5)]
        public string FullName { get; set; } = null!;

        [Required]
        [Range(0, 2)]
        public int AgeGroup { get; set; }

        [Required]
        [Range(0, 1)]
        public int Gender { get; set; }

        public int[] Medicines { get; set; } = null!;
    }
}
