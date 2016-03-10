using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Monitoreo.Models.BO
{
    public class PersonalMateriaAula
    {
        public int ID { get; set; }

        public int PersonalMateriaId { get; set; }
        public virtual PersonalMateria PersonalMateria { get; set; }

        public int AulaId { get; set; }
        public virtual Aula Aula { get; set; }

        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
    }
}