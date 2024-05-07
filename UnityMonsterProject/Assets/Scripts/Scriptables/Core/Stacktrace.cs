using ScriptableArchitecture.Core;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Is a stack trace of messages for variables with a maximum capacity. Is excluded in build
/// </summary>
public class Stacktrace
{
    private Queue<string> _messages = new Queue<string>();

    private VariableType _stackType;
    private int _maxCapacity;

    /// <summary>
    /// Initializes a new stacktrace with a given variabletype and capacity
    /// </summary>
    public Stacktrace(VariableType stackType, int maxCapacity = 100)
    {
        _stackType = stackType;
        _maxCapacity = Math.Max(1, maxCapacity);
    }

    /// <summary>
    /// Adds a message to the stacktrace. Dequeues elements from the stacktrace when the capacity has been reached.
    /// Appends the current date to the begin of the message
    /// </summary>
    public void Add(string message)
    {
        if (_messages.Count >= _maxCapacity)
            _messages.Dequeue();

        _messages.Enqueue(DateTime.Now.ToString("HH:mm:ss") + ": " + message);
    }

    /// <summary>
    /// CLears the messages of the stacktrace
    /// </summary>
    public void Clear() => _messages.Clear();

    /// <summary>
    /// Gets the messages of the stacktrace as a string list
    /// </summary>
    public List<string> GetMessages() => _messages.ToList();

    /// <summary>
    /// Ges the variable type of the stacktrace
    /// </summary>
    public VariableType GetStackType() => _stackType;
}