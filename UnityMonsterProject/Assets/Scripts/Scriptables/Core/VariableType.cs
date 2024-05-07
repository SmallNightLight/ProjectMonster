namespace ScriptableArchitecture.Core
{
    /// <summary>
    /// Enum for different types of variables
    /// </summary>
    public enum VariableType
    {
        Variable, VariableEvent, Event, RuntimeSet
    }

    /// <summary>
    /// Enum for different initialization types for variables
    /// </summary>
    public enum InitializeType
    {
        Normal, ResetOnGameStart, ReadOnly
    }
}