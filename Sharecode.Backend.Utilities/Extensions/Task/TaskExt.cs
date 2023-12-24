using System.Diagnostics;

namespace Sharecode.Backend.Utilities.Extensions.Task;

public static class TaskExt
{
    public static async Task<T[]> WhenAll<T>(params Task<T>[] tasks)
    {
        var allTasks = System.Threading.Tasks.Task.WhenAll(tasks);

        try
        {
            return await allTasks;
        }
        catch (Exception)
        {
            //ignore
        }

        throw allTasks.Exception ??
              throw new UnreachableException("Failed to compute multiple tasks together!");


    }
}
