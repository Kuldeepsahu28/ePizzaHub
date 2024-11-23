using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.UI.Areas.Admin.Models
{
    public class AddOrEditItemVM
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Unite Price")]
        public decimal UnitPrice { get; set; }


        public IFormFile ImageFile { get; set; }
        public string ImageUrl { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name="Item Type")]
        [Required]
        public int ItemTypeId { get; set; }

        [Display(Name = "Create Date")]
        public DateTime CreatedDate { get; set; }= DateTime.Now;
    }
}
