using IJERTS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IJERTS.DAL;

namespace IJERTS.BLL
{
    public class Home : IHome
    {

        IHomeRepository homeRepository = new HomeRepository();

        public int PostQuery(Queries query)
        {
            return (homeRepository.PostQuery(query));
        }

        public List<CurrentIssues> GetCurrentIssues()
        {
            return (homeRepository.GetCurrentIssues());
        }

        public int IncreamentDownloadCounter(int paperId)
        {
            return (homeRepository.IncreamentDownloadCounter(paperId));
        }


    }
}
