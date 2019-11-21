using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IJERTS.Objects
{
    public class EditorDashboard : IJERTSAbstract
    {
        public List<Paper> LstPapers { get; set; }

        public List<Users> LstUsers { get; set; }

        public List<Queries> LstQueries { get; set; }

    }
}
