using System;
using System.ComponentModel.DataAnnotations;

namespace CItyInfoTut.API.Models
{
    public class PointOfInterestForCreationDto
    {
        [Required( ErrorMessage = "The name is required.")]
        [MaxLength(50)]
		public string Name { get; set; }

        [MaxLength(200)]
		public string Description { get; set; }

        public PointOfInterestForCreationDto()
        {
        }
    }
}
