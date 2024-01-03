using DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models
{
    public class GenreModel: Record
    {
        [Required(ErrorMessage = "{0} is required!")]
        public string Name { get; set; }

        #region Extra properties required for the views
        [DisplayName("Movie Count")]
        public int MovieCountOutput { get; set; }

        [DisplayName("Movies")]
        //[Required(ErrorMessage = "{0} must be selected!")
        public List<int> MovieIdsInput { get; set; }

        [DisplayName("Movies")]
        public string MovieNamesOutput { get; set; }
        #endregion
    }
}
