using PaymentServices.PaymentServices.Interface;
using PaymentServices.PaymentServices.Types;

namespace PaymentServices.PaymentServices.Service;

public class PaymentService : IPaymentService
{
    private readonly IDataStore dataStore;

    public PaymentService(IDataStore dataStore)
    {
        this.dataStore = dataStore;
    }
    public MakePaymentResult MakePayment(MakePaymentRequest request)
    {
        Account account = dataStore.GetAccount(request.DebtorAccountNumber);
        var result = new MakePaymentResult();

        PaymentScheme paymentScheme = request.PaymentScheme;
        string paymentKey = Enum.GetName<PaymentScheme>(paymentScheme)!;
        AllowedPaymentSchemes allowedPaymentScheme = Enum.Parse<AllowedPaymentSchemes>(paymentKey);

        if (account == null
            || !account.AllowedPaymentSchemes.HasFlag(allowedPaymentScheme)
            || (account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments) && account.Balance < request.Amount)
            || (account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps) && account.Status != AccountStatus.Live))
        {
            result.Success = false;
            return result;
        }
        
        if (result.Success)
        {
            account.Balance -= request.Amount;
            dataStore.UpdateAccount(account);
        }
        return result;
    }
}
