using PaymentServices.PaymentServices.Types;

namespace PaymentServices.PaymentServices.Service;

public interface IPaymentService
{
        MakePaymentResult MakePayment(MakePaymentRequest request);
}