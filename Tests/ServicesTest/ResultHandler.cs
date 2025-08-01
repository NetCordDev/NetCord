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

    public static ResultHandler ParseFail()
    {
        return new(Assert.IsInstanceOfType<TypeReaderFailResult>);
    }
}
