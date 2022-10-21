using PaymentServices.PaymentServices.Interface;
using PaymentServices.PaymentServices.Types;

namespace PaymentServices.PaymentServices.Data;

public class DataStore : IDataStore
{
    public DataStore()
    {
        Accounts = new List<Account>();
    }
    public Account GetAccount(string accountNumber)
    {
        return new Account();
    }

    public void UpdateAccount(Account account)
    {
    }

    public List<Account> Accounts { get; set; }
}