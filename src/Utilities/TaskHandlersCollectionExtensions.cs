using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
    internal static class TaskHandlersCollectionExtensions
    {
        public static void ExecuteTaskHandlers(
            [CanBeNull] this List<ActivityTaskHandler> taskHandlers, [NotNull] Task<object> activityTask)
        {
            if (taskHandlers == null || taskHandlers.Count == 0) return;
            activityTask.AssertNotNull("activityTask != null");

            foreach (ActivityTaskHandler handler in taskHandlers)
            {
                handler(activityTask);
            }
        }
    }
}