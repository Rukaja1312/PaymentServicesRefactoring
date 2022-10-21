using System.Configuration;
using PaymentServices.PaymentServices.Types;

namespace PaymentServices.PaymentServices.Data;

public class AccountTypes
{
    public DataStore GetDataStore()
    {
        var dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];

        DataStore accountDataStore;

        if (dataStoreType == "Backup")
        {
            accountDataStore = new BackupAccountDataStore();
        }
        else
        {
            accountDataStore = new AccountDataStore();
        }
        return accountDataStore;
    }
    
}