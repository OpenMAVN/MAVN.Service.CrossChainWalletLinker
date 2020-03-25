using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Sdk;

namespace Lykke.Service.CrossChainWalletLinker.Services
{
    [UsedImplicitly]
    public class ShutdownManager : IShutdownManager
    {
        private readonly IEnumerable<IStopable> _components;
        private readonly ILog _log;

        public ShutdownManager(IEnumerable<IStopable> components, ILogFactory logFactory)
        {
            _components = components;
            _log = logFactory.CreateLog(this);
        }

        public Task StopAsync()
        {
            Parallel.ForEach(_components, c =>
            {
                try
                {
                    c.Stop();
                }
                catch (Exception e)
                {
                    _log.Warning($"Couldn't stop component [{c.GetType().Name}]", e);
                }
            });

            return Task.CompletedTask;
        }
    }
}
