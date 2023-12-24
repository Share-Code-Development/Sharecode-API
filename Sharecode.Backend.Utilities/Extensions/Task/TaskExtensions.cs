using System.Runtime.CompilerServices;

namespace Sharecode.Backend.Utilities.Extensions.Task;

public static class TaskExtensions
{
    
    public static TaskAwaiter<(T, T)> GetAwaiter<T>(this (Task<T>, Task<T>) tasksTuple)
    {
        async Task<(T, T)> CombineTasks()
        {
            var (task1, task2) = tasksTuple;
            await TaskExt.WhenAll(task1, task2);
            return (task1.Result, task2.Result);
        }

        return CombineTasks().GetAwaiter();
    }
    
    public static TaskAwaiter<T[]> GetAwaiter<T>(this (Task<T>, Task<T>, Task<T>) tasksTuple)
    {
        return TaskExt.WhenAll(tasksTuple.Item1, tasksTuple.Item2, tasksTuple.Item3).GetAwaiter();
    }
    
}
