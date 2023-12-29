using Cassandra;
using Moq;

namespace Tests.Domain.Repositories;

public static class UserRepositoryTestHelper
{
    private static readonly PreparedStatement PreparedStatement = new();

    public static void SetupExecuteAsync(
        this Mock<ISession> sessionMock,
        object[] expectedQueryValues,
        RowSet resultMock,
        Times countOfInvocations)
    {
        sessionMock.Setup(_ => _.ExecuteAsync(
                It.Is<IStatement>(s =>
                    s.QueryValues.Length == expectedQueryValues.Length
                    && s
                        .QueryValues
                        .Select(qv => qv.ToString())
                        .OrderBy(qv => qv)
                        .SequenceEqual(expectedQueryValues
                            .Select(eqv => eqv.ToString())
                            .OrderBy(eqv => eqv))),
                It.IsAny<string>()))
            .ReturnsAsync(resultMock)
            .Verifiable(countOfInvocations);
    }

    public static void SetupPrepareAsync(
        this Mock<ISession> sessionMock,
        string cql,
        Times countOfInvocations)
    {
        sessionMock.Setup(_ =>
                _.PrepareAsync(It.Is<string>(q => q == cql)))
            .ReturnsAsync(PreparedStatement)
            .Verifiable(countOfInvocations);
    }
}