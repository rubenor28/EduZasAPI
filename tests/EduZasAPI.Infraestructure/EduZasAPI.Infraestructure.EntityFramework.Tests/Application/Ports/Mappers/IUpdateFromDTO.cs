public interface IUpdateFromDTO<T>
where T : class
{
    void UpdateEntityProperties(T updatedProperties);
}
