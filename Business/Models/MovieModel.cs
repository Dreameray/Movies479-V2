#nullable disable 
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class MovieModel
    {
        
        [Required(ErrorMessage = "{0} is required!")]
        public string Name { get; set; }

        public short Year { get; set; }

        public double Revenue { get; set; }

        [DisplayName("Director")]
        [Required(ErrorMessage = "{0} is required!")]
        public int? DirectorId { get; set; }

        #region Extra properties required for the views
        [DisplayName("Director")]
        public string DirectorNameOutput { get; set; }
        #endregion

    }
}
