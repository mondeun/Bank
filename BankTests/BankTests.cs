using System.Collections.Generic;
using Bank;
using Bank.Exceptions;
using Bank.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace BankTests
{
    [TestFixture]
    public class BankTests
    {
        private IAuditLogger _auditLogger;
        private Bank.Bank _sut;
        private Account _account;

        [SetUp]
        public void Setup()
        {
            _auditLogger = Substitute.For<IAuditLogger>();
            _sut = new Bank.Bank(_auditLogger);
            _account = new Account
            {
                Balance = 0.0m,
                Name = "John",
                Number = "1"
            };
        }

        [Test]
        public void CanCreateBankAccount()
        {
            _sut.CreateAccount(_account);
            var acc = _sut.GetAccount("1");

            Assert.AreEqual("John", acc.Name);
            Assert.AreEqual("1", acc.Number);
            Assert.Zero(acc.Balance);
        }

        [Test]
        public void CanNotCreateDuplicateAccounts()
        {
            _sut.CreateAccount(_account);

            Assert.Throws<DuplicateAccount>(() => _sut.CreateAccount(_account));
        }

        [Test]
        public void WhenCreatingAnAccount_AMessageIsWrittenToTheAuditLog()
        {
            _sut.CreateAccount(_account);

            _auditLogger.Received().AddMessage(Arg.Is<string>(x => x.Contains("John") && x.Contains("1")));
        }

        [Test]
        public void WhenCreatingAnValidAccount_OnMessageAreWrittenToTheAuditLog()
        {
            _sut.CreateAccount(_account);

            _auditLogger.Received(1).AddMessage(Arg.Any<string>());
        }

        [Test]
        public void WhenCreatingAnInvalidAccount_TwoMessagesAreWrittenToTheAuditLog()
        {
            _account.Number = "error";

            Assert.Throws<InvalidAccount>(() => _sut.CreateAccount(_account));
            _auditLogger.Received(2).AddMessage(Arg.Any<string>());
        }

        [Test]
        public void WhenCreatingAnInvalidAccount_AWarn12AndErro45MessageIsWrittenToAuditLog()
        {
            _account.Number = "error";

            Assert.Throws<InvalidAccount>(() => _sut.CreateAccount(_account));
            _auditLogger.Received(2).AddMessage(Arg.Is<string>(m => m.Contains("Warn12:") || m.Contains("Error45:")));
        }

        [Test]
        public void VerifyThat_GetAuditLog_GetsTheLogFromTheAuditLogger()
        {
            _auditLogger.GetLog().Returns(new List<string>
            {
                "Log 1",
                "Log 2",
                "Log 3"
            });

            var logs = _sut.GetAuditLog();

            Assert.AreEqual(3, logs.Count);
        }
    }
}
