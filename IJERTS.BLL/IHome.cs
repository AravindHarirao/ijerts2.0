using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IJERTS.Objects;

namespace IJERTS.BLL
{
    public interface IHome
    {
        int PostQuery(Queries query);

        List<CurrentIssues> GetCurrentIssues();

        int IncreamentDownloadCounter(int paperId);


    }
}
