namespace AxpoTest.Abstractions
{
    public interface IGenerateCV
    {
        void GenerateCSVAsync(DateTime date);
        void CreateCSVFolder(string path);
    }
}
