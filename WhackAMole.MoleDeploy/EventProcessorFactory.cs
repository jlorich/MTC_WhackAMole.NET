using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhackAMole.MoleDeploy
{
    public class EventProcessorFactory : IEventProcessorFactory
    {
        private readonly Func<string, Task> _Handler;

        public EventProcessorFactory(Func<string, Task> handler)
        {
            _Handler = handler;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            var processor = new EventProcessor(_Handler);
            return processor;
        }
    }
}
