using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Entities
{
    public class State
    {
        public int StateId { get; set; }
        [Required]
        [Index(IsUnique =true)]
        [StringLength(100)]
        public string StateName { get; set; }
        public ICollection<District> Districts { get; set; }
    }
}
