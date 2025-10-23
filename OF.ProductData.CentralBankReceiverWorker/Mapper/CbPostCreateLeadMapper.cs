using OF.DataSharing.Model.CentralBank.CoPQuery;
using OF.ProductData.Model.EFModel.Products;

namespace OF.ServiceInitiation.CentralBankReceiverWorker.Mappers
{
    public static class CbPostCreateLeadMapper
    {

        public static EFCreateLeadRequest MapCbPostLeadResponsetToEF(CbPostCreateLeadRequestDto requestDto)
        {
            var leadList = new EFCreateLeadRequest();

            var header = requestDto.cbPostCreateLeadHeader!;
            var request = requestDto.cbPostCreateLeadRequest;

            

            return leadList;
        }  

    }
}
