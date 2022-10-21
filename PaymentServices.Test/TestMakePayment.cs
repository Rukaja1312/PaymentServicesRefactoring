using PaymentServices.PaymentServices.Data;
using PaymentServices.PaymentServices.Service;
using PaymentServices.PaymentServices.Types;

namespace PaymentServices.Test;

public class Tests
{
     private const string AaccountNumberFirst = "1";
        private const string AaccountNumberSecond = "12";
        private const string AaccountNumberThird = "123";

        private const decimal ammount = 10m;

        private const AllowedPaymentSchemes Chaps = AllowedPaymentSchemes.Chaps;
        private const AllowedPaymentSchemes Bacs = AllowedPaymentSchemes.Bacs;
        private const AllowedPaymentSchemes Faster = AllowedPaymentSchemes.FasterPayments;

        private const PaymentScheme FasterScheme = PaymentScheme.FasterPayments;
        private const PaymentScheme BacsScheme = PaymentScheme.Bacs;
        private const PaymentScheme ChapsScheme = PaymentScheme.Chaps;

        private const AccountStatus LiveStatus = AccountStatus.Live;
        private const AccountStatus DisabledStatus = AccountStatus.Disabled;
        private const AccountStatus InboundStatus = AccountStatus.InboundPaymentsOnly;

        private Account firstAccount;
        private Account secondAccount;
        private Account thirdAccount;

        private AccountDataStore accountDataStore;
        private PaymentService service;

        [SetUp]
        public void Setup()
        {
            firstAccount = new Account()
            {
                AccountNumber = AaccountNumberFirst,
                Balance = ammount,
                AllowedPaymentSchemes = Chaps,
                Status = LiveStatus
            };

            secondAccount = new Account()
            {
                AccountNumber = AaccountNumberSecond,
                Balance = ammount,
                AllowedPaymentSchemes = Faster,
                Status = LiveStatus
            };

            thirdAccount = new Account()
            {
                AccountNumber = AaccountNumberThird,
                Balance = ammount,
                AllowedPaymentSchemes = Bacs,
                Status = LiveStatus
            };

            accountDataStore = new AccountDataStore();

            service = new PaymentService(accountDataStore);

            accountDataStore.Accounts
                .AddRange(new Account[] { firstAccount, secondAccount, thirdAccount });
        }

        [Test]
        public void PaymentFailsWhenUserIsNull()
        {
            string dummyNumber = "Dummy";

            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = ammount,
                CreditorAccountNumber = dummyNumber,
                DebtorAccountNumber = dummyNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = ChapsScheme
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void PaymentFailsWhenPaymentIsNotAllowed()
        {
            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = ammount,
                CreditorAccountNumber = firstAccount.AccountNumber,
                DebtorAccountNumber = firstAccount.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = FasterScheme
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsFalse(result.Success);
        }

        [Test]
        [TestCase(PaymentScheme.Chaps, 5.1d, AccountStatus.Disabled)]
        [TestCase(PaymentScheme.FasterPayments, 55.2d, AccountStatus.Live)]
        public void PaymentFailsWhenExtraConditionsAreFalse(PaymentScheme scheme, decimal amount, AccountStatus status)
        {
            firstAccount.Status = status;

            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = amount,
                CreditorAccountNumber = firstAccount.AccountNumber,
                DebtorAccountNumber = firstAccount.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = scheme
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void PaymentSuccessfulBacs()
        {
            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = ammount,
                CreditorAccountNumber = firstAccount.AccountNumber,
                DebtorAccountNumber = thirdAccount.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = BacsScheme
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsTrue(result.Success);
        }

        [Test]
        public void PaymentSuccessfulChaps()
        {
            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = ammount,
                CreditorAccountNumber = thirdAccount.AccountNumber,
                DebtorAccountNumber = firstAccount.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = ChapsScheme
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsTrue(result.Success);
        }

        [Test]
        public void PaymentSuccessfulFaster()
        {
            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = ammount,
                CreditorAccountNumber = thirdAccount.AccountNumber,
                DebtorAccountNumber = secondAccount.AccountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = FasterScheme
            };

            MakePaymentResult result = service.MakePayment(request);

            Assert.IsTrue(result.Success);
        }

        [Test]
        [TestCase(AaccountNumberFirst, ChapsScheme)]
        [TestCase(AaccountNumberSecond, FasterScheme)]
        [TestCase(AaccountNumberThird, Bacs)]
        public void PaymentWithdrawsCorrectly(string accountNumber, PaymentScheme scheme)
        {
            decimal expected = 8m;

            MakePaymentRequest request = new MakePaymentRequest()
            {
                Amount = 2m,
                CreditorAccountNumber = thirdAccount.AccountNumber,
                DebtorAccountNumber = accountNumber,
                PaymentDate = DateTime.Now,
                PaymentScheme = scheme
            };

            MakePaymentResult result = service.MakePayment(request);

            Account account = accountDataStore.GetAccount(accountNumber);

            Assert.AreEqual(expected, account.Balance);
        }
}