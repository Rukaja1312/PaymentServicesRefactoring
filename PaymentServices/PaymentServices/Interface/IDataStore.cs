using PaymentServices.PaymentServices.Types;

namespace PaymentServices.PaymentServices.Interface;

public interface IDataStore
{
    Account GetAccount(string accountNumber);
    void UpdateAccount(Account account);
}