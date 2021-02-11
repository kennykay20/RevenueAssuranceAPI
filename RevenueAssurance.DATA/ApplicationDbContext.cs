using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using anythingGoodApi.AnythingGood.DATA.Models;
using RevAssuranceWebAPi.AnythingGood.DATA.Models;
using RevAssuranceApi.RevenueAssurance.DATA.Models;

namespace RevAssuranceApi.RevenueAssurance.DATA.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options) {
        }
        public DbSet<AuditTrail> admAuditTrail { get; set;}
        public DbSet<admBankServiceSetup> admBankServiceSetup { get; set;}
        public DbSet<admClientProfile> admClientProfile { get; set;}
        public DbSet<admUserLogin> admUserLogin { get; set;}
        public DbSet<admAccountType> admAccountType  { get; set;}
        public DbSet<admAmmendAndRepriReason> admAmmendAndRepriReason { get; set;}
        public DbSet<admErrorMsg> admErrorMsg { get; set;}
        public DbSet<admCountry> admCountry {get; set;}
        public DbSet<admAppReturnMsgSetUp> admAppReturnMsgSetUp { get; set;}
        public DbSet<admBank> admBank { get; set;}
        public DbSet<admBankBranch> admBankBranch { get; set;}
        public DbSet<admBranchesSupQuery>  admBranchesSupQuery { get; set;}
        public DbSet<admBroadCast> admBroadCast { get; set;}
        public DbSet<admChannel>  admChannel { get; set;}
        public DbSet<admCharge> admCharge { get; set;}
        public DbSet<admChargeExemption> admChargeExemptions { get; set;}
        public DbSet<admChequeProduct> admChequeProduct { get; set;}
        public DbSet<admChqVendor> admChqVendor { get; set;}
        public DbSet<admCollateralType> admCollateralType { get; set;}
        public DbSet<admCouterChqReason> admCouterChqReason { get; set;}
        public DbSet<admCurrency> admCurrencies { get; set;}
        public DbSet<admDataPullControl> admDataPullControl { get; set;}
        public DbSet<admDepartment> admDepartment { get; set;}
        public DbSet<admIndustrySecMapg> admIndustrySecMapg{ get; set;}
        public DbSet<admIndustrySector> admIndustrySector{ get; set;}
        public DbSet<admInstrumentCode> admInstrumentCode{ get; set;}
        public DbSet<admIssuranceCoverType> admIssuranceCoverType{ get; set;}
        public DbSet<admLicenseSetUp> admLicenseSetUp { get; set;}
        public DbSet<admLicenseSetUpHistory> admLicenseSetUpHistory{ get; set;}
        public DbSet<admMenuControl> admMenuControl{ get; set;}
        public DbSet<admRejectionReason> admRejectionReason { get; set;}
        public DbSet<admRole> admRole{ get; set;}
        public DbSet<admRoleAssignment> admRoleAssignment{ get; set;}
        public DbSet<admRoleLimit> admRoleLimits { get; set;}
        public DbSet<admService> admService { get; set;}
        public DbSet<admServiceReference> admServiceReference{ get; set;}
        public DbSet<admStatementSetUp> admStatementSetUp { get; set;}
        public DbSet<admStatusItem> admStatusItem { get; set;}
        public DbSet<admSupervisorBranch> admSupervisorBranch { get; set;}
        public DbSet<admTemplate> admTemplates { get; set;}
        public DbSet<admTranCodeConfig> admTranCodeConfig{ get; set;}
        public DbSet<admTransactionConfiguration> admTransactionConfiguration{ get; set;}
        public DbSet<admUploadAudit> admUploadAudit { get; set;}
        public DbSet<admUserLimit> admUserLimits { get; set;}
        public DbSet<admUserProfile> admUserProfile { get; set;}
        public DbSet<AggregatedCounter> AggregatedCounter { get; set;}
        public DbSet<BatchControl> BatchControl{ get; set;}
        public DbSet<BatchItems> BatchItems { get; set; }
        public DbSet<BatchItemsTemp> BatchItemsTemp { get; set; }
        public DbSet<CardRequestTemp> CardRequestTemp { get; set;}
        public DbSet<CbsTransaction> CbsTransaction { get; set;}
        public DbSet<CollateralStatus> CollateralStatus { get; set;}
        public DbSet<Counter> Counter { get; set;}
        public DbSet<EMailFormat> EMailFormat { get; set;}
        public DbSet<FXRate> FXRate { get; set;}
        public DbSet<OprAcctStatment> OprAcctStatment { get; set;}
        public DbSet<OprAmortizationSchedule> OprAmortizationSchedule { get; set;}
        public DbSet<OprApproval> OprApproval { get; set;}
        public DbSet<OprBankGuarantee> OprBankGuarantee { get; set;}
        public DbSet<OprBidBond> OprBidBond { get; set;}
        public DbSet<OprBidSecurity> OprBidSecurity { get; set;}
        public DbSet<OprBondsAndGuarantee> OprBondsAndGuarantee { get; set;}
        public DbSet<OprCard> OprCard { get; set;}
        public DbSet<OprChqBookRequest> OprChqBookRequest { get; set;}
        public DbSet<oprCollateral> oprCollateral { get; set;}
        public DbSet<oprCounterChq> oprCounterChq { get; set;}
        public DbSet<oprInstrmentTemp> oprInstrmentTem { get; set;}
        public DbSet<OprInstrument> OprInstrument { get; set;}
        public DbSet<OprOverDraft_backUp> OprOverDraft_backUp { get; set;}
        public DbSet<OprOverDraft> OprOverDraft { get; set;}
        public DbSet<OprPerformanceBond> OprPerformanceBond { get; set;}
        public DbSet<OprPrintingHistory> OprPrintingHistory { get; set;}
        public DbSet<OprReferenceLetter> OprReferenceLetter { get; set;}
        public DbSet<OprRejectionReason> OprRejectionReason { get; set;}
        public DbSet<oprServiceCharges> oprServiceCharges { get; set;}
        public DbSet<oprStatementReq> oprStatementReq { get; set;}
        public DbSet<OprStopChqRequest> OprStopChqRequest { get; set;}
        public DbSet<OprToken> OprToken { get; set;}
        public DbSet<OprTradeReference> OprTradeReference { get; set;}
        public DbSet<ReccuringBatch> ReccuringBatch { get; set;}
        public DbSet<RecurringBatchControl> RecurringBatchControl { get; set;}
        public DbSet<SalaryTemp> SalaryTemp { get; set;}
        public DbSet<ServiceRefModel> ServiceRefModel { get; set;}
        public DbSet<ServicesModel> ServicesModel { get; set;}
        public DbSet<TokenRequisitionTemp> TokenRequisitionTemp { get; set;}
        public DbSet<TransactionLog> TransactionLog { get; set;}
        public DbSet<UserLimit>  UserLimit { get; set;}
        public DbSet<UsersInRole> UsersInRole { get; set;}
        public DbSet<ValidationTransLimit> ValidationTransLimit { get; set;}
        public DbSet<OprBusinessSearch> OprBusinessSearch { get; set;}
        public DbSet<OprAcctClosure> OprAcctClosure { get; set;}
        public DbSet<BatchControlTemp> BatchControlTemp { get; set;}
        public DbSet<admAmendReprintReason> admAmendReprintReason { get; set;} 
        public DbSet<OprAmmendAndReprint> OprAmmendAndReprint { get; set;} 
        public DbSet<oprDocRetrieval> oprDocRetrieval { get; set;} 
        public DbSet<admDocumentChg> admDocumentChg { get; set;} 
        public DbSet<oprDocChgDetail> oprDocChgDetail { get; set;} 
        public DbSet<admStatementChg> admStatementChg { get; set;} 
        public DbSet<oprPinReset> oprPinReset { get; set;} 
        public DbSet<admPinItems> admPinItems { get; set;} 
        public DbSet<admAPIConfig> admAPIConfig { get; set;} 


public  async Task<int> SaveChanges(int? UserId)
{
    int saved = -1;
    try
    {
        foreach (var entry in ChangeTracker.Entries().ToList()
        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
        {

            AuditEntry auditEntry = new AuditEntry(entry);

            var oldValues = await entry.GetDatabaseValuesAsync().ConfigureAwait(false);

            foreach (var property in entry.Properties.ToList())
            {
                if (property.IsTemporary)
                {
                    // value will be generated by the database, get the value after saving
                    //auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        //auditEntry.OldValues[propertyName] = property.OriginalValue;
                        auditEntry.OldValues[propertyName] = oldValues[propertyName];
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            //auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.OldValues[propertyName] = oldValues[propertyName];
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            auditEntry.columnName = propertyName;
                        }
                        break;
                }
            }
            if (entry.State == EntityState.Added)
            {                       
            }
            if (entry.State == EntityState.Modified)
            {
                
                object[] ObjectId = entry.Metadata.FindPrimaryKey()
                    .Properties
                    .Select(p => entry.Property(p.Name).CurrentValue)
                    .ToArray();


                /*admAuditTrail  audit = new admAuditTrail();

                audit.eventdateutc = DateTime.Now;
                audit.tablename = entry.Metadata.Relational().TableName;
                audit.originalvalue = auditEntry.OldValues.Count == 0 ? null: JsonConvert.SerializeObject(auditEntry.OldValues);
                audit.newvalue =  auditEntry.NewValues.Count == 0 ? null: JsonConvert.SerializeObject(auditEntry.NewValues);
                Guid id = Guid.NewGuid();
                audit.auditlogid = id;
                audit.eventtype = "M";// entry.Property("Status").CurrentValue.ToString();
                audit.recordid = ObjectId == null ? "" :  ObjectId[0].ToString();
                audit.userId  = (int)UserId;
                audit.columnname =  auditEntry.columnName;
                admAuditTrail.Add(audit);
                */

                AuditTrail  audit = new AuditTrail();

                audit.Eventdateutc = DateTime.Now;
                audit.Tablename = entry.Metadata.Relational().TableName;
                audit.Originalvalue = auditEntry.OldValues.Count == 0 ? null: JsonConvert.SerializeObject(auditEntry.OldValues);
                audit.Newvalue =  auditEntry.NewValues.Count == 0 ? null: JsonConvert.SerializeObject(auditEntry.NewValues);
                audit.Eventtype = "M";// entry.Property("Status").CurrentValue.ToString();
                audit.Recordid = ObjectId == null ? "" :  ObjectId[0].ToString();
                audit.Userid  = (int)UserId;
                // audit.columnname =  auditEntry.columnName;
                admAuditTrail.Add(audit);
            }
            if(entry.State == EntityState.Deleted)
            {
                
                object[] ObjectId = entry.Metadata.FindPrimaryKey()
                    .Properties
                    .Select(p => entry.Property(p.Name).CurrentValue)
                    .ToArray();
                
                /* admAuditTrail  audit = new admAuditTrail();
                audit.eventdateutc = DateTime.Now;
                audit.tablename = entry.Metadata.Relational().TableName;
                audit.originalvalue = auditEntry.OldValues.Count == 0 ? null: JsonConvert.SerializeObject(auditEntry.OldValues);
                audit.newvalue =  auditEntry.NewValues.Count == 0 ? null: JsonConvert.SerializeObject(auditEntry.NewValues);
                Guid id = Guid.NewGuid();
                audit.auditlogid = id;
                audit.eventtype = "D";
                audit.recordid = ObjectId == null ? "" :  ObjectId[0].ToString();
                audit.userId = (int)UserId;
                audit.auditlogid = Guid.NewGuid();
                audit.columnname = auditEntry.columnName;
                
                admAuditTrail.Add(audit);
                */

                AuditTrail  audit = new AuditTrail();

                audit.Eventdateutc = DateTime.Now;
                audit.Tablename = entry.Metadata.Relational().TableName;
                audit.Originalvalue = auditEntry.OldValues.Count == 0 ? null: JsonConvert.SerializeObject(auditEntry.OldValues);
                audit.Newvalue =  auditEntry.NewValues.Count == 0 ? null: JsonConvert.SerializeObject(auditEntry.NewValues);
                audit.Eventtype = "D";// entry.Property("Status").CurrentValue.ToString();
                audit.Recordid = ObjectId == null ? "" :  ObjectId[0].ToString();
                audit.Userid  = (int)UserId;
                // audit.columnname =  auditEntry.columnName;
                admAuditTrail.Add(audit);
            }
        }

        
        saved = await base.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        var exM = ex == null ? ex.InnerException.Message : ex.Message;
        
    }
    return saved;
}


    
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }
        public EntityEntry Entry { get; }
        public string TableName { get; set; }
        public string columnName { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();
        public bool HasTemporaryProperties => TemporaryProperties.Any();
    }
    
    }


}