using System.Collections;

using NetCord.Services;

namespace ServicesTest;

public class ResultHandler(Action<IExecutionResult> handler)
{
    public void Handle(IExecutionResult result)
    {
        handler(result);
    }

    public static ResultHandler Success()
    {
        return new(Assert.IsNotInstanceOfType<IFailResult>);
    }

    public static ResultHandler DataMatch<TData>(TData expectedData)
    {
        return new(result =>
        {
            Assert.IsInstanceOfType<IExceptionResult>(result, out var exceptionResult);

            Assert.IsInstanceOfType<ResultDataException<TData>>(exceptionResult.Exception, out var dataException);

            Assert.AreEqual(expectedData, dataException.Data, "The data returned from the exception does not match the expected data.");
        });
    }

    public static ResultHandler DataMatchCollection<TCollection>(TCollection expectedCollection) where TCollection : IEnumerable
    {
        return new(result =>
        {
            Assert.IsInstanceOfType<IExceptionResult>(result, out var exceptionResult);

            Assert.IsInstanceOfType<ResultDataException<TCollection>>(exceptionResult.Exception, out var dataException);

            CollectionAssert.AreEqual(expectedCollection.Cast<object>().ToArray(), dataException.Data.Cast<object>().ToArray(), "The collection returned from the exception does not match the expected collection.");
        });
    }

    public static ResultHandler ParseFail()
    {
        return new(result =>
        {
            var resultTypeName = result.GetType().Name;
            Assert.EndsWith("TypeReaderFailResult", resultTypeName, $"Expected a type reader to fail, got '{resultTypeName}' instead");
        });
    }
}
