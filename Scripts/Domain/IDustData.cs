namespace Domain
{
    public interface IDustData
    {
        string DustName { get; }

        float Price { get; }

        float Weight { get; }

        float SpawnWeight { get; }
    }
}