namespace api_gerenciar_funcionarios.Core.Application
{
    public class UseCaseResult<T>
    {
        public bool Success { get; }
        public IEnumerable<string> ErrorMessages { get; }
        public T Data { get; }

        private UseCaseResult(bool success, T data, IEnumerable<string> errorMessage)
        {
            Success = success;
            Data = data;
            ErrorMessages = errorMessage;
        }

        private UseCaseResult(bool success, T data, string errorMessage)
        {
            Success = success;
            Data = data;
            ErrorMessages = [errorMessage];
        }

        private UseCaseResult(bool success, T data)
        {
            Success = success;
            Data = data;
            ErrorMessages = [];
        }

        public static UseCaseResult<T> Ok(T data) => new UseCaseResult<T>(true, data);
        public static UseCaseResult<T> Fail(string errorMessage) => new UseCaseResult<T>(false, default, errorMessage);

        public static UseCaseResult<T> Fail(IEnumerable<string> errorMessage) => new UseCaseResult<T>(false, default, errorMessage);
    }
}
