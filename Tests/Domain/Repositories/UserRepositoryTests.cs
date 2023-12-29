using Cassandra;
using Cassandra.Data.Linq;
using Domain.DTO;
using Domain.Models;
using Domain.Repositories.UserRepository;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Tests.Domain.Repositories;

public class UserRepositoryTests
{
    private IUserRepository _userRepository = null!;
    private Mock<Table<UserEntity>> _table = null!;
    private Mock<ISession> _session = null!;

    private readonly ILogger<UserRepository> _logger = new LoggerFactory()
        .CreateLogger<UserRepository>();

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

        _table = new Mock<Table<UserEntity>>(_session.Object);
        _userRepository = new UserRepository(
            _table.Object,
            _logger);
    }

    [Test]
    public async Task GetUserAsync_HasOneUser_ReturnUser()
    {
        // Arrange
        var userEntity = new UserEntity
        {
            Login = "login",
            Password = "password",
            AccessToken = Guid.NewGuid().ToString(),
            AccessTokenExpire = DateTimeOffset.Now
        };
        var row = CreateMockRow(userEntity);
        _session.SetupExecuteAsync(
            new object[] {userEntity.Login},
            CreateMockRowSet(row),
            Times.Once());
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(t => t.Login == userEntity.Login)
                .ToString(),
            Times.Once());

        // Act
        var userEntityActual = await _userRepository.GetUserAsync(userEntity.Login);

        // Assert
        _session.Verify();
        Assert.IsNotNull(userEntityActual);
        Assert.AreEqual(CreateUserEntityForCompare(userEntity), CreateUserEntityForCompare(userEntityActual));
    }

    [Test]
    public void GetUserAsync_HasNotUser_ReturnNull()
    {
        // Arrange
        const string login = "login";
        _session.SetupExecuteAsync(new object[] {login}, CreateMockRowSet(), Times.Once());
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(t => t.Login == login)
                .ToString(),
            Times.Once());

        // Act / Assert
        var actualException = Assert.ThrowsAsync<Exception>(async () => await _userRepository.GetUserAsync(login));
        _session.Verify();
        Assert.AreEqual($"User with login {login} doesnt exist", actualException!.Message);
    }

    [Test]
    public async Task TryGetUserAsync_HasNotUser_ReturnNull()
    {
        // Arrange
        const string login = "login";
        _session.SetupExecuteAsync(
            new object[] {login},
            CreateMockRowSet(),
            Times.Once());
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(t => t.Login == login)
                .ToString(),
            Times.Once());

        // Act
        var userEntityActual = await _userRepository.TryGetUserAsync(login);

        // Assert
        _session.Verify();
        Assert.IsNull(userEntityActual);
    }

    [Test]
    public async Task TryGetUserAsync_HasOneUser_ReturnUser()
    {
        // Arrange
        var userEntity = new UserEntity
        {
            Login = "login",
            Password = "password",
            AccessToken = Guid.NewGuid().ToString(),
            AccessTokenExpire = DateTimeOffset.Now
        };
        var row = CreateMockRow(userEntity);
        _session.SetupExecuteAsync(
            new object[] {userEntity.Login},
            CreateMockRowSet(row),
            Times.Once());
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(t => t.Login == userEntity.Login)
                .ToString(),
            Times.Once());

        // Act
        var userEntityActual = await _userRepository.TryGetUserAsync(userEntity.Login);

        // Assert
        _session.Verify();
        Assert.IsNotNull(userEntityActual);
        Assert.AreEqual(CreateUserEntityForCompare(userEntity), CreateUserEntityForCompare(userEntityActual!));
    }

    [Test]
    public void TryGetUserAsync_HasMoreThanOneUser_ThrowException()
    {
        // Arrange
        var userEntity = new UserEntity
        {
            Login = "login",
            Password = "password",
            AccessToken = Guid.NewGuid().ToString(),
            AccessTokenExpire = DateTimeOffset.Now
        };
        var row = CreateMockRow(userEntity);
        _session.SetupExecuteAsync(
            new object[] {userEntity.Login},
            CreateMockRowSet(row, row),
            Times.Once());
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(t => t.Login == userEntity.Login)
                .ToString(),
            Times.Once());

        // Act / Assert
        var exceptionActual = Assert.ThrowsAsync<Exception>(
            async () => await _userRepository.TryGetUserAsync(userEntity.Login));
        _session.Verify();
        Assert.AreEqual("Key Login in User doesnt unique", exceptionActual!.Message);
    }

    [Test]
    public async Task DeleteUserAsync_UserExist_ReturnUser()
    {
        // Arrange
        var userEntity = new UserEntity
        {
            Login = "login",
            Password = "password",
            AccessToken = Guid.NewGuid().ToString(),
            AccessTokenExpire = DateTimeOffset.Now
        };
        var row = CreateMockRow(userEntity);
        _session.SetupExecuteAsync(
            new object[] {userEntity.Login},
            CreateMockRowSet(row),
            Times.Exactly(2));
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(t => t.Login == userEntity.Login)
                .ToString(),
            Times.Once());
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(t => t.Login == userEntity.Login)
                .Delete()
                .ToString(),
            Times.Once());

        // Act
        await _userRepository.DeleteUserAsync(userEntity.Login);

        // Assert
        _session.Verify();
    }

    [Test]
    public void DeleteUserAsync_UserDoesntExist_ThrowException()
    {
        // Arrange
        var login = "login";
        _session.SetupExecuteAsync(
            new object[] {login},
            CreateMockRowSet(),
            Times.Once());
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(t => t.Login == login)
                .ToString(),
            Times.Once());

        // Act / Assert
        var exceptionActual = Assert.ThrowsAsync<Exception>(async () => await _userRepository.DeleteUserAsync(login));
        _session.Verify();
        Assert.AreEqual($"User with login {login} doesnt exist", exceptionActual!.Message);
    }

    [Test]
    public async Task CreateUserAsync_UserDoesntExist_ExecuteQuery()
    {
        // Arrange
        var userEntity = new UserEntity
        {
            Login = "login",
            Password = "password",
            AccessToken = Guid.NewGuid().ToString(),
            AccessTokenExpire = DateTimeOffset.Now
        };
        _session.SetupExecuteAsync(
            new object[] {userEntity.Login},
            CreateMockRowSet(),
            Times.Once());
        _session.SetupExecuteAsync(
            new object[]
            {
                userEntity.AccessToken,
                userEntity.AccessTokenExpire,
                userEntity.Login,
                userEntity.Password
            },
            CreateMockRowSet(),
            Times.Once());
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(t => t.Login == userEntity.Login)
                .ToString(),
            Times.Once());
        _session.SetupPrepareAsync(
            _table
                .Object
                .Insert(userEntity)
                .ToString(), Times.Once());

        // Act
        await _userRepository.CreateUserAsync(userEntity);

        // Assert
        _session.Verify();
    }

    [Test]
    public void CreateUserAsync_UserAlreadyExist_ThrowException()
    {
        // Arrange
        var userEntity = new UserEntity
        {
            Login = "login",
            Password = "password",
            AccessToken = Guid.NewGuid().ToString(),
            AccessTokenExpire = DateTimeOffset.Now
        };
        var row = CreateMockRow(userEntity);
        _session.SetupExecuteAsync(
            new object[] {userEntity.Login},
            CreateMockRowSet(row),
            Times.Once());
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(t => t.Login == userEntity.Login)
                .ToString(),
            Times.Once());

        // Act / Assert
        var exceptionActual =
            Assert.ThrowsAsync<Exception>(async () => await _userRepository.CreateUserAsync(userEntity));
        _session.Verify();
        Assert.AreEqual($"User already exist", exceptionActual!.Message);
    }

    [Test]
    public void ChangePasswordAsync_UserDoesntExist_ThrowException()
    {
        // Arrange
        var userEntity = new UserEntity
        {
            Login = "login",
            Password = "newPassword",
            AccessToken = Guid.NewGuid().ToString(),
            AccessTokenExpire = DateTimeOffset.Now
        };
        _session.SetupExecuteAsync(
            new object[]
            {
                userEntity.Login
            },
            CreateMockRowSet(),
            Times.Once());
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(t => t.Login == userEntity.Login)
                .ToString(),
            Times.Once());

        // Act / Assert
        var exceptionActual =
            Assert.ThrowsAsync<Exception>(async () =>
                await _userRepository.ChangePasswordAsync(userEntity.Login, userEntity.Password));
        _session.Verify();
        Assert.AreEqual($"User with login {userEntity.Login} doesnt exist", exceptionActual!.Message);
    }

    [Test]
    public async Task ChangePasswordAsync_UserExist_ExecuteQuery()
    {
        // Arrange
        var userEntity = new UserEntity
        {
            Login = "login",
            Password = "newPassword",
            AccessToken = Guid.NewGuid().ToString(),
            AccessTokenExpire = DateTimeOffset.Now
        };
        _session.SetupExecuteAsync(
            new object[]
            {
                userEntity.Login
            },
            CreateMockRowSet(CreateMockRow(userEntity)),
            Times.Once());
        _session.SetupExecuteAsync(
            new object[]
            {
                userEntity.Password,
                userEntity.Login
            },
            CreateMockRowSet(),
            Times.Once());
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(t => t.Login == userEntity.Login)
                .ToString(),
            Times.Once()
        );
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(r => r.Login == userEntity.Login)
                .Select(r => new {Password = string.Empty})
                .Update()
                .ToString(),
            Times.Once());

        // Act
        await _userRepository.ChangePasswordAsync(userEntity.Login, userEntity.Password);

        // Assert
        _session.Verify();
    }

    [Test]
    public async Task ChangeAccessTokenAsync_UserExist_ExecuteQuery()
    {
        // Arrange
        var userEntity = new UserEntity
        {
            Login = "login",
            Password = "password",
            AccessToken = Guid.NewGuid().ToString(),
            AccessTokenExpire = DateTimeOffset.Now
        };
        var token = new TokenInfo(Guid.NewGuid().ToString(), DateTimeOffset.Now);
        _session.SetupExecuteAsync(
            new object[]
            {
                userEntity.Login
            },
            CreateMockRowSet(CreateMockRow(userEntity)),
            Times.Once());
        _session.SetupExecuteAsync(
            new object[]
            {
                token.Token,
                token.Expire,
                userEntity.Login
            },
            CreateMockRowSet(),
            Times.Once());
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(t => t.Login == userEntity.Login)
                .ToString(),
            Times.Once());
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(r => r.Login == userEntity.Login)
                .Select(r => new {AccessToken = string.Empty, AccessTokenExpire = string.Empty})
                .Update()
                .ToString(),
            Times.Once());

        // Act
        await _userRepository.ChangeAccessTokenAsync(userEntity.Login, token);

        // Assert
        _session.Verify();
    }

    [Test]
    public void ChangeAccessTokenAsync_UserDoesntExist_ThrowException()
    {
        // Arrange
        const string login = "login";
        _session.SetupExecuteAsync(
            new object[]
            {
                login
            },
            CreateMockRowSet(),
            Times.Once());
        _session.SetupPrepareAsync(
            _table
                .Object
                .Where(t => t.Login == login)
                .ToString(),
            Times.Once()
        );

        // Act / Assert
        var exceptionActual = Assert.ThrowsAsync<Exception>(async () =>
            await _userRepository.ChangeAccessTokenAsync(login, new TokenInfo(string.Empty, DateTimeOffset.Now))
        );
        _session.Verify();
        Assert.AreEqual($"User with login {login} doesnt exist", exceptionActual!.Message);
    }

    private record UserEntityForCompare(string Login, string Password, string AccessToken,
        DateTimeOffset AccessTokenExpire);

    private UserEntityForCompare CreateUserEntityForCompare(UserEntity userEntity)
    {
        return new UserEntityForCompare(userEntity.Login, userEntity.Password, userEntity.AccessToken,
            userEntity.AccessTokenExpire);
    }

    private Row CreateMockRow(UserEntity userEntity)
    {
        var rowMock = new Mock<Row>();
        //String type is needed here, because in the process of mapping
        //row to poco in the runtime, the element type is retrieved
        rowMock.Setup(r => r.GetValue<String>(0)).Returns(userEntity.AccessToken);
        rowMock.Setup(r => r.GetValue<DateTimeOffset>(1)).Returns(userEntity.AccessTokenExpire);
        rowMock.Setup(r => r.GetValue<String>(2)).Returns(userEntity.Login);
        rowMock.Setup(r => r.GetValue<String>(3)).Returns(userEntity.Password);
        return rowMock.Object;
    }

    private RowSet CreateMockRowSet(params Row[] rows)
    {
        var mockList = new List<Row>();
        mockList.AddRange(rows);
        var rowSet = new Mock<RowSet>();
        rowSet.Setup(rs => rs.Columns).Returns(CreateColumns());
        rowSet.Setup(m => m.GetEnumerator()).Returns(mockList.GetEnumerator());
        rowSet.Setup(rs => rs.Info).Returns(new ExecutionInfo());
        return rowSet.Object;
    }

    private CqlColumn[] CreateColumns()
    {
        // Reflection could be used here, but one problem remains,
        // how will the mapping of types like string -> ascii, string -> text will work,
        // so far there is no answer to this
        var accessTokenColumn = new CqlColumn
        {
            Keyspace = "password_storage_system",
            Name = "access_token",
            Table = "users",
            TypeCode = ColumnTypeCode.Varchar,
            TypeInfo = null,
            IsStatic = false,
            Index = 0,
            Type = typeof(string)
        };
        var accessTokenExpireColumn = new CqlColumn
        {
            Keyspace = "password_storage_system",
            Name = "access_token_expire",
            Table = "users",
            TypeCode = ColumnTypeCode.Timestamp,
            TypeInfo = null,
            IsStatic = false,
            Index = 1,
            Type = typeof(DateTimeOffset)
        };
        var loginColumn = new CqlColumn
        {
            Keyspace = "password_storage_system",
            Name = "login",
            Table = "users",
            TypeCode = ColumnTypeCode.Varchar,
            TypeInfo = null,
            IsStatic = false,
            Index = 2,
            Type = typeof(string)
        };
        var passwordColumn = new CqlColumn
        {
            Keyspace = "password_storage_system",
            Name = "password",
            Table = "users",
            TypeCode = ColumnTypeCode.Varchar,
            TypeInfo = null,
            IsStatic = false,
            Index = 3,
            Type = typeof(string)
        };
        return new[]
        {
            accessTokenColumn,
            accessTokenExpireColumn,
            loginColumn,
            passwordColumn
        };
    }
}