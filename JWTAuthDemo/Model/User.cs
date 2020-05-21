using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JWTAuthDemo.Model
{
    public class User
    {
        [Key]
        [Required]
        public int ID { get; set; }
        
        [Required(ErrorMessage = "An user name is required.")]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "A password is required.")]
        [StringLength(6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "An email address is required.")]
        [StringLength(50)]
        public string EmailAddress { get; set; }
    }

}
