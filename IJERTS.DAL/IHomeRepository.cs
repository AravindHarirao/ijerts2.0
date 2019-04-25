﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IJERTS.Objects;

namespace IJERTS.DAL
{
    public interface IHomeRepository
    {
        int PostQuery(Queries query);

        List<Queries> GetQueries();

        Queries GetQueriesForId(int id);

        int UpdateQuery(int queryId, string response);

        List<CurrentIssues> GetCurrentIssues();

        int IncreamentDownloadCounter(int paperId);

    }
}
