﻿using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Job.QuorumTransactionWatcher.Contract;
using Lykke.RabbitMqBroker.Subscriber;
using MAVN.Service.CrossChainWalletLinker.Domain.RabbitMq.Handlers;

namespace MAVN.Service.CrossChainWalletLinker.DomainServices.RabbitMq.Subscribers
{
    public class UndecodedSubscriber : JsonRabbitSubscriber<UndecodedEvent>
    {
        private readonly IUndecodedEventHandler _undecodedEventHandler;

        public UndecodedSubscriber(
            IUndecodedEventHandler undecodedEventHandler,
            string connectionString,
            string exchangeName,
            string queueName,
            ILogFactory logFactory) 
            : base(connectionString, exchangeName, queueName, logFactory)
        {
            _undecodedEventHandler = undecodedEventHandler;
        }

        protected override async Task ProcessMessageAsync(UndecodedEvent message)
        {
            await _undecodedEventHandler.HandleAsync(message.Topics, message.Data, message.OriginAddress);
        }
    }
}
