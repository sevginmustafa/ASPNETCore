using ForExercising.CustomModelValidations;
using ForExercising.ViewModels.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ForExercising.ViewModels
{
    public class AddYuGiOhCardViewModel : IValidatableObject
    {
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; }

        public MonsterAttribute Attribute { get; set; }

        [Range(1, 12)]
        public int Level { get; set; }

        [Required]
        [Url]
        public string Artwork { get; set; }

        public MonsterType Type { get; set; }

        [Required]
        [MinLength(20)]
        [MaxLength(200)]
        public string Description { get; set; }

        [Range(0, 5000)]
        public int Attack { get; set; }

        [Range(0, 5000)]
        public int Defense { get; set; }

        [Display(Name ="Date of creation")]
        [IsBeforeCurrentDate]
        [DataType(DataType.Date)]
        public DateTime CreatedOn { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Attack + Defense > 9000)
            {
                yield return new ValidationResult("Sum of the Attack and Defense cannot be more than 9000");
            }

            if (CreatedOn.Year<1900)
            {
                yield return new ValidationResult("The year of creation shoud be after 1900");
            }
        }
    }
}
