using Newtonsoft.Json;
using OF.DataSharing.Model.CentralBank.CoPQuery;
using OF.ProductData.Model.CentralBank.Products;
using OF.ProductData.Model.EFModel.CreateLead;
using OF.ProductData.Model.EFModel.Products;

namespace OF.ServiceInitiation.CentralBankReceiverWorker.Mappers
{
    public static class CbPostCreateLeadMapper
    {

        public static (EFCreateLeadHeaderRequest HeaderEntity, EFCreateLeadRequest LeadEntity)
         MapCbPostLeadResponseToEF(CbPostCreateLeadRequestDto requestDto)
        {
            if (requestDto == null)
                throw new ArgumentNullException(nameof(requestDto));

            var header = requestDto.cbPostCreateLeadHeader;
            var request = requestDto.cbPostCreateLeadRequest?.Data;




            // 🔹 Map Lead Request
            var leadEntity = new EFCreateLeadRequest
            {
                Email = request?.Email,
                PhoneNumber = request?.PhoneNumber,
                GivenName = request?.Name?.GivenName,
                LastName = request?.Name?.LastName,
                EmiratesId = request?.EmiratesId,
                Nationality = request?.Nationality,

                LeadResidentialAddressType = request?.ResidentialAddress?.AddressType,
                LeadResidentialAddressLine = request?.ResidentialAddress?.AddressLine != null
                    ? string.Join(", ", request.ResidentialAddress.AddressLine)
                    : null,
                LeadResidentialBuildingNumber = request?.ResidentialAddress?.BuildingNumber,
                LeadResidentialBuildingName = request?.ResidentialAddress?.BuildingName,
                LeadResidentialFloor = request?.ResidentialAddress?.Floor,
                LeadResidentialStreetName = request?.ResidentialAddress?.StreetName,
                LeadResidentialDistrictName = request?.ResidentialAddress?.DistrictName,
                LeadResidentialPostBox = request?.ResidentialAddress?.PostBox,
                LeadResidentialTownName = request?.ResidentialAddress?.TownName,
                LeadResidentialCountrySubDivision = request?.ResidentialAddress?.CountrySubDivision,
                LeadResidentialCountry = request?.ResidentialAddress?.Country,

                LeadInformation = request?.LeadInformation,
                MarketingOptOut = request?.MarketingOptOut,
                ProductType = request?.ProductCategories != null
                    ? string.Join(", ", request.ProductCategories.Select(c => c.Type))
                    : null,
                CorrelationId = requestDto.CorrelationId,
                CreatedBy = "System",
                CreatedOn = DateTime.UtcNow,
                Status = "PENDING"

            };
            // 🔹 Map Header
            var headerEntity = new EFCreateLeadHeaderRequest
            {
                O3ProviderId = header?.O3ProviderId,
                O3CallerOrgId = header?.O3CallerOrgId,
                O3CallerClientId = header?.O3CallerClientId,
                O3CallerSoftwareStatementId = header?.O3CallerSoftwareStatementId,
                O3ApiUri = header?.O3ApiUri,
                O3ApiOperation = header?.O3ApiOperation,
                O3CallerInteractionId = header?.O3CallerInteractionId,
                O3OzoneInteractionId = header?.O3OzoneInteractionId,
                XFapiCustomerIpAddress = header?.XFapiCustomerIpAddress,
                RequestId = header!.RequestId
            };
            return (headerEntity, leadEntity);
        }

        public static EFCreateLeadResponse MapCbPostCreateLeadResponseToEF(
    CbCreateLeadResponseWrapper responseDto,
    long requestId)
        {
            if (responseDto == null)
                throw new ArgumentNullException(nameof(responseDto));

            var data = responseDto.centralBankCreateLeadResponse;
            var leadData = data?.data;

            if (leadData == null)
                throw new ArgumentNullException(nameof(responseDto.centralBankCreateLeadResponse.data));

            var entity = new EFCreateLeadResponse
            {
                // 🔗 Foreign key relationship
                RequestId = requestId,
                
                // Lead details
                LeadId = leadData.LeadId,
                Email = leadData.Email,
                PhoneNumber = leadData.PhoneNumber,
                GivenName = leadData.Name?.GivenName,
                LastName = leadData.Name?.LastName,
                EmiratesId = leadData.EmiratesId,
                Nationality = leadData.Nationality,

                // Address mapping
                LeadResidentiaAddressType = leadData.ResidentialAddress?.AddressType,
                LeadResidentialAddressAddressLine = leadData.ResidentialAddress?.AddressLine != null
                    ? string.Join(", ", leadData.ResidentialAddress.AddressLine)
                    : null,
                LeadResidentialAddressBuildingNumber = leadData.ResidentialAddress?.BuildingNumber,
                LeadResidentialAddressBuildingName = leadData.ResidentialAddress?.BuildingName,
                LeadResidentialAddressFloor = leadData.ResidentialAddress?.Floor,
                LeadResidentialAddressStreetName = leadData.ResidentialAddress?.StreetName,
                LeadResidentialAddressDistrictName = leadData.ResidentialAddress?.DistrictName,
                LeadResidentialAddressPostBox = leadData.ResidentialAddress?.PostBox,
                LeadResidentialAddressTownName = leadData.ResidentialAddress?.TownName,
                LeadResidentialAddressCountrySubDivision = leadData.ResidentialAddress?.CountrySubDivision,
                LeadResidentialAddressCountry = leadData.ResidentialAddress?.Country,

                // Lead info
                LeadInformation = leadData.LeadInformation,
                MarketingOptOut = leadData.MarketingOptOut,

                // Product info
                ProductType = leadData.ProductCategories != null
                    ? string.Join(", ", leadData.ProductCategories.Select(p => p.Type))
                    : null,

                // Audit metadata
                CreatedBy = "System",
                Status = "PROCESSED",
                CreatedOn = DateTime.UtcNow,
                ResponseJson = JsonConvert.SerializeObject(responseDto.centralBankCreateLeadResponse!)
            };

            return entity;
        }

    }
}
