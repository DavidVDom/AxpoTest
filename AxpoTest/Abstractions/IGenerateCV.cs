namespace AxpoTest.Abstractions
{
    public interface IGenerateCV
    {
        Task GenerateCSVAsync(DateTime date);
    }
}
