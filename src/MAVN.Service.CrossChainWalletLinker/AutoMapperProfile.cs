using AutoMapper;
using Lykke.PrivateBlockchain.Definitions;
using MAVN.Service.CrossChainWalletLinker.Client.Models;
using MAVN.Service.CrossChainWalletLinker.Domain.Models;

namespace MAVN.Service.CrossChainWalletLinker
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<LinkingRequestResultModel, LinkingRequestResponseModel>();

            CreateMap<PublicAddressResultModel, PublicAddressResponseModel>();

            CreateMap<LinkingApprovalResultModel, LinkingApprovalResponseModel>();

            CreateMap<PublicAccountLinkedEventDTO, PublicAccountLinkedDto>()
                .ForMember(dest => dest.PrivateAddress, opt => opt.MapFrom(x => x.InternalAccount))
                .ForMember(dest => dest.PublicAddress, opt => opt.MapFrom(x => x.PublicAccount));

            CreateMap<PublicAccountUnlinkedEventDTO, PublicAccountUnlinkedDto>()
                .ForMember(dest => dest.PrivateAddress, opt => opt.MapFrom(x => x.InternalAccount))
                .ForMember(dest => dest.PublicAddress, opt => opt.MapFrom(x => x.PublicAccount));

            CreateMap<UnlinkResultModel, UnlinkResponseModel>();

            CreateMap<IConfigurationItem, ConfigurationItemResponseModel>();
        }
    }
}
