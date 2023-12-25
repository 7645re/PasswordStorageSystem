using System.Globalization;
using Cassandra;
using Cassandra.Data.Linq;
using Domain.Models;
using Domain.Repositories.UserRepository;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Tests.Domain.Repositories;

public class UserRepositoryTests
{
    private IUserRepository _userRepository;
    private Mock<Table<UserEntity>> _table;
    private Mock<ISession> _session;

    public RowSet CreateMockRowSet(params Row[] rows)
    {
        var mockList = new List<Row>();
        mockList.AddRange(rows);
        var rowSet = new Mock<RowSet>();
        rowSet.Setup(m => m.GetEnumerator()).Returns(mockList.GetEnumerator());
        rowSet.Setup(rs => rs.Info).Returns(new ExecutionInfo());
        return rowSet.Object;
    }

    [SetUp]
    public void Setup()
    {
        _session = new Mock<ISession>();
        _session
            .Setup(s => s.Cluster)
            .Returns(Cluster
                .Builder()
                .WithCredentials("user_name", "password")
                .WithPort(9042)
                .AddContactPoint("0.0.0.0")
                .Build());

        var logger = new LoggerFactory().CreateLogger<UserRepository>();
        var table = new Mock<Table<UserEntity>>(_session.Object);
        _table = table;
        _userRepository = new UserRepository(
            table.Object,
            logger);
        _session.Setup(_ =>
                _.PrepareAsync(It.IsAny<string>()))
            .ReturnsAsync(new PreparedStatement())
            .Verifiable();
        _session.Setup(_ => _.ExecuteAsync(
                It.IsAny<IStatement>(),
                It.IsAny<string>()))
            .ReturnsAsync(CreateMockRowSet());
    }

    [Test]
    public async Task TryGetUserAsync_SearchByKeyLogin_GenerateValidQuery()
    {
        var login = "login";

        await _userRepository.TryGetUserAsync(login);
        _session.Verify(_ => _.ExecuteAsync(
            It.Is<IStatement>(s =>
                s.QueryValues.Length == 1
                && s.QueryValues
                    .First()
                    .ToString() == login),
            It.IsAny<string>()));
        _session.Verify(_ =>
            _.PrepareAsync(It.Is<string>(q => q == _table
                .Object
                .Where(t => t.Login == login)
                .ToString())));
    }

    [Test]
    public async Task TryGetUserAsync_ResultLengthMoreThanOne_ThrowException()
    {
        var login = "login";

        _session.Setup(_ => _.ExecuteAsync(
                It.IsAny<IStatement>(),
                It.IsAny<string>()))
            .ReturnsAsync(CreateMockRowSet(new Mock<Row>().Object, new Mock<Row>().Object));

        var exceptionActual = Assert.ThrowsAsync<Exception>(
            () => _userRepository.TryGetUserAsync(login));
        Assert.AreEqual("Key Login in User doesnt unique", exceptionActual.Message);
    }
}