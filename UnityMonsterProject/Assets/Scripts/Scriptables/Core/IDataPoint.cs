namespace ScriptableArchitecture.Core
{
    /// <summary>
    /// Interface for the serialized datastructures, does not add functionality, but it used for editor drawers
    /// </summary>
    public interface IDataPoint { }

    public interface IAssignment<T>
    {
        public T Copy();
    }
}