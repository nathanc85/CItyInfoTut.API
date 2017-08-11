﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CItyInfoTut.API.Entities
{
    public class PointOfInterest
    {

		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [ForeignKey("CityId")]
        public City City { get; set; }

        public int CityId { get; set; }



        public PointOfInterest()
        {
        }
    }
}
