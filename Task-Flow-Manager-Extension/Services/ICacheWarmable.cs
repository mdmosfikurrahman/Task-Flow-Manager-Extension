namespace Task_Flow_Manager_Extension.Services;

public interface ICacheWarmable
{
    string EntityName { get; }
    Task<int> RefreshCache();
}