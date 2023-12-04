#nullable disable 
using DataAccess;
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
    public class DirectorModel: Record
    {

        [Required(ErrorMessage = "{0} is required!")]
        public string Name { get; set; }

        public string Surname { get; set; }

        public DateTime BirthDate { get; set; }

        [DisplayName("Retired")]
        public bool IsRetired { get; set; }

        #region Extra properties required for the views

        [DisplayName("Retired")]
        public string IsRetiredOutput { get; set; }

        [DisplayName("Movie Count")]
        public int MovieCountOutput { get; set; }
        #endregion
    }
}
