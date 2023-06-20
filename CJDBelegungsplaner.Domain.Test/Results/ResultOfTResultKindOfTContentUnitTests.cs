using CJDBelegungsplaner.Domain.Models;
using CJDBelegungsplaner.Domain.Results;
using FluentAssertions;
using System.Windows.Media.Media3D;

namespace CJDBelegungsplaner.Domain.Test.Results;

// https://blog.ploeh.dk/2011/05/09/GenericunittestingwithxUnit.net/

public abstract class ResultOfTResultKindOfTContentUnitTests<TResultKind, TContent>
    where TResultKind : ResultKind
    where TContent : EntityObject
{

    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public void Failure_ReturnsResultOfTResultKindOfTContentWithFailure_WhenGivenAnyInt(int kind)
    {
        /// Arange
        var originResult = new Result<TResultKind, TContent>();
        var expectedResult = new Result<TResultKind, TContent>(
            false,
            (TResultKind)Activator.CreateInstance(typeof(TResultKind), new object[] { kind }));

        /// Act
        var result = originResult.Failure(kind);

        /// Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Theory]
    [InlineData(1, "matter1")]
    [InlineData(-1, "matter-1")]
    public void Failure_ReturnsResultOfTResultKindOfTContentWithFailure_WhenGivenAnyIntAndMatterAndTContent(int kind, string matter)
    {
        /// Arange
        var resultObject = new Result<TResultKind, TContent>();
        var entityObject = (TContent)Activator.CreateInstance(typeof(TContent));

        /// Act
        var result = resultObject.Failure(kind, matter, entityObject);

        /// Assert
        Assert.True(result.IsFailure);
        Assert.Equal(kind, result.Kind);
        Assert.Equal(matter, result.Matter);
        Assert.Equal(entityObject, result.Content);
    }

    public void PassOn_ReturnsResultOfTResultKindOfTContentWithValuesOfGivenResultObject_WhenGivenResultObject<TResultKindGivenType>()
        where TResultKindGivenType : ResultKind
    {
        /// Arange
        var resultWithGivenType = new Result<TResultKindGivenType, TContent>(
            true,
            (TResultKindGivenType)Activator.CreateInstance(typeof(TResultKindGivenType), new object[] { 1 }),
            (TContent)Activator.CreateInstance(typeof(TContent)));
        var resultWithTargetType = new Result<TResultKind, TContent>();

        /// Act
        var result = resultWithTargetType.PassOn(resultWithGivenType);

        /// Assert
        Assert.Equal(typeof(Result<TResultKind, TContent>), result.GetType()); // Eigentlich überflüssig, weil PassOn aufgrund der Typensicherheit stets den richtigen Type zurückgibt
        Assert.Equal(resultWithGivenType.Kind.KindValue, result.Kind.KindValue);
        Assert.Equal(resultWithGivenType.Content, result.Content);
    }
}

public class ResultOfDataServiceResultKindOfGuestTests : ResultOfTResultKindOfTContentUnitTests<DataServiceResultKind, Guest>
{
    [Fact]
    public void PassOn_ReturnsResultOfDataServiceResultKindOfGuestWithValuesOfGivenResultObject_WhenGivenResultOfDataServiceResultKind()
    {
        PassOn_ReturnsResultOfTResultKindOfTContentWithValuesOfGivenResultObject_WhenGivenResultObject<DataServiceResultKind>();
    }

    [Fact]
    public void PassOn_ReturnsResultOfDataServiceResultKindOfGuestWithValuesOfGivenResultObject_WhenGivenResultOfAuthenticationResultKind()
    {
        PassOn_ReturnsResultOfTResultKindOfTContentWithValuesOfGivenResultObject_WhenGivenResultObject<AuthenticationResultKind>();
    }
}

public class ResultOfAuthenticationResultKindOfUserTest : ResultOfTResultKindOfTContentUnitTests<AuthenticationResultKind, User>
{
    [Fact]
    public void PassOn_ReturnsResultOfDataServiceResultKindOfGuestWithContentOfGivenResultObject_WhenGivenResultOfDataServiceResultKind()
    {
        PassOn_ReturnsResultOfTResultKindOfTContentWithValuesOfGivenResultObject_WhenGivenResultObject<DataServiceResultKind>();
    }

    [Fact]
    public void PassOn_ReturnsResultOfDataServiceResultKindOfGuestWithContentOfGivenResultObject_WhenGivenResultOfAuthenticationResultKind()
    {
        PassOn_ReturnsResultOfTResultKindOfTContentWithValuesOfGivenResultObject_WhenGivenResultObject<AuthenticationResultKind>();
    }
}
