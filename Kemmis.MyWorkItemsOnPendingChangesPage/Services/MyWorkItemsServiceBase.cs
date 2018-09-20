using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.Services
{
    internal abstract class MyWorkItemsServiceBase
    {
        private readonly IServiceProvider _serviceProvider;
        private ITeamFoundationContext _context;

        protected MyWorkItemsServiceBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected ITeamFoundationContext Context
        {
            get
            {
                if (_context == null)
                {
                    var contextManager = GetService<ITeamFoundationContextManager>();
                    if (contextManager != null)
                    {
                        _context = contextManager.CurrentContext;
                    }
                }

                return _context;
            }
        }

        private T GetService<T>()
        {

            if (_serviceProvider != null)
            {
                return (T)_serviceProvider.GetService(typeof(T));
            }

            return default(T);
        }
    }
}
