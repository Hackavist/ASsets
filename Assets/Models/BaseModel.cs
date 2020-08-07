using System;
using System.ComponentModel.DataAnnotations;

namespace Assets.Models
{
    public abstract class BaseModel
    {
        [Key] public int Id { get; set; }

        [Required] public DateTime AddedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}