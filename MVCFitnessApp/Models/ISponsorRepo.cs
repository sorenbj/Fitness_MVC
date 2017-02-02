using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCFitnessApp.Models
{
    // Interface
    public interface ISponsorRepo
    {
        IEnumerable<Sponsor> GetSponsors();
        Sponsor GetSponsorById(int id);
        void InsertSponsor(Sponsor sponsor);
        void DeleteSponsor(int id);
        void EditSponsor(Sponsor sponsor);
    }
}
