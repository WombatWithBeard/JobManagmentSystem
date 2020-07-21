#nullable enable
using System.Threading.Tasks;

namespace JobManagmentSystem.Scheduler.Common.Interfaces
{
    public interface IJobTask
    {
        Task Invoke(object? state);

    }
}