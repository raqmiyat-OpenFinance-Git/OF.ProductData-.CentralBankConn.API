using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog;
using OF.ProductData.CoreBankConn.API.EFModel;
using OF.ProductData.CoreBankConn.API.Repositories;
using OF.ProductData.Model.EFModel;
using System.Data;

namespace OF.ProductData.CoreBankConn.API.Repositories;

public class CoreBankRepository : ICoreBankRepository
{
    private readonly CbsDbContext _context;
    private readonly IDbConnection _dbConnection;

    public CoreBankRepository(CbsDbContext context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }
    public async Task<string> GetNextSequenceNoAsync(string serviceName, string referenceNbr, Logger logger)
    {
        try
        {
            long sequenceNo = 0;
            int retryCount = 2;

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    sequenceNo = await GetNextSeqNoAsync(serviceName, referenceNbr, logger);
                    logger.Trace($"ReferenceNbr: {referenceNbr} || Sequence number retrieved: {sequenceNo}");
                    if (sequenceNo > 0) break;
                }
                catch (SqlException ex)
                {
                    logger.Error(ex, $"ReferenceNbr: {referenceNbr} || Attempt {i + 1} failed.");
                    if (i == retryCount - 1) throw;
                }
            }

            string formattedSeqNo = sequenceNo.ToString().PadLeft(7, '0');
            string sequenceNoString = $"OF{DateTime.Now:yyMMdd}{formattedSeqNo}";

            logger.Trace($"ReferenceNbr: {referenceNbr} || Formatted sequence number: {sequenceNoString}");
            return sequenceNoString;
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"ReferenceNbr: {referenceNbr} || Error in GetNextSequenceNoAsync");
            throw;
        }
    }

    public async Task<long> SaveCoreBankEnquiryAsync(CoreBankEnquiry enquiry, Logger logger)
    {
        try
        {
            if (!enquiry.CreatedOn.HasValue)
                enquiry.CreatedOn = DateTime.UtcNow;

            _context.CoreBankEnquiries.Add(enquiry);
            await _context.SaveChangesAsync();


            return enquiry.Id; // Return the generated ID
        }
        catch (DbUpdateException dbEx)
        {
            logger.Error(dbEx, "Database update error occurred while saving CoreBankEnquiry.");
            throw; // Rethrow or return a special value depending on your error handling strategy
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"ExternalRefNbr: {enquiry.ExternalReferenceNumber} || An error occurred while saving CoreBankEnquiry.");
            throw;
        }
    }

    private async Task<long> GetNextSeqNoAsync(string serviceName, string referenceNbr, Logger logger)
    {
        logger.Trace($"ExternalRefNbr: {referenceNbr} || GetNextSeqNo started for service: {serviceName}");
        long result = 0;

        try
        {

            var parameters = new DynamicParameters();
            parameters.Add("@InstrName", serviceName, DbType.String);

            var sequenceNumber = await _dbConnection.ExecuteScalarAsync<long?>(
                "OF_sp_get_next_ENQ_SEQ_No",
                parameters,
                commandType: CommandType.StoredProcedure
                );
            if (sequenceNumber.HasValue)
            {
                result = sequenceNumber.Value;
            }
            else
            {
                logger.Error($"ExternalRefNbr: {referenceNbr} || Invalid or null sequence number returned.");
            }

            logger.Trace($"ExternalRefNbr: {referenceNbr} || Sequence number result: {result}");
        }
        catch (Exception ex)
        {
            logger.Error(ex, $"ExternalRefNbr: {referenceNbr} || Error in GetNextSeqNo");
        }

        return result;
    }

}
