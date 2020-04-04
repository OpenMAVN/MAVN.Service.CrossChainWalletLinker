using AutoMapper;
using Lykke.PrivateBlockchain.Definitions;
using MAVN.Service.CrossChainWalletLinker.Domain.Enums;
using MAVN.Service.CrossChainWalletLinker.Domain.Models;
using MAVN.Service.CrossChainWalletLinker.Domain.Services;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;

namespace MAVN.Service.CrossChainWalletLinker.DomainServices
{
    public class BlockchainEventDecoder : IBlockchainEventDecoder
    {
        private readonly EventTopicDecoder _eventTopicDecoder;
        private readonly string _publicAccountLinkedEventSignature;
        private readonly string _publicAccountUnlinkedEventSignature;
        private readonly IMapper _mapper;

        public BlockchainEventDecoder(IMapper mapper)
        {
            _mapper = mapper;
            _eventTopicDecoder = new EventTopicDecoder();
            _publicAccountLinkedEventSignature = $"0x{ABITypedRegistry.GetEvent<PublicAccountLinkedEventDTO>().Sha3Signature}";
            _publicAccountUnlinkedEventSignature = $"0x{ABITypedRegistry.GetEvent<PublicAccountUnlinkedEventDTO>().Sha3Signature}";
        }
        
        public PublicAccountLinkedDto DecodePublicAccountLinkedEvent(string[] topics, string data)
        {
            var decodedEvent = DecodeEvent<PublicAccountLinkedEventDTO>(topics, data);

            return _mapper.Map<PublicAccountLinkedDto>(decodedEvent);
        }

        public PublicAccountUnlinkedDto DecodePublicAccountUnlinkedEvent(string[] topics, string data)
        {
            var decodedEvent = DecodeEvent<PublicAccountUnlinkedEventDTO>(topics, data);

            return _mapper.Map<PublicAccountUnlinkedDto>(decodedEvent);
        }

        public BlockchainEventType GetEventType(string topic)
        {
            if (topic == _publicAccountLinkedEventSignature)
                return BlockchainEventType.PublicAccountLinked;

            if (topic == _publicAccountUnlinkedEventSignature)
                return BlockchainEventType.PublicAccountUnlinked;

            return BlockchainEventType.Unknown;
        }
        
        private T DecodeEvent<T>(string[] topics, string data) where T : class, new()
            => _eventTopicDecoder.DecodeTopics<T>(topics, data);
    }
}
