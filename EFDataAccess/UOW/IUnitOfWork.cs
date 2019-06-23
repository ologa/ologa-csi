using EFDataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPPS.CSI.Domain;

namespace EFDataAccess.UOW
{
    /// <summary>
    /// Interface for the Unit of Work"
    /// </summary>
    public interface IUnitOfWork
    {
        // Save pending changes to the data store.
        void Commit();

        int ExecuteSqlCommand(string sql);        
    }
}
