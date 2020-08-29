using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;

namespace DungeonRPG.Criteria
{
    public interface ICriterion<in T>
    {
        Task<bool> JudgeAsync(SocketCommandContext sourceContext, T parameter);
    }
}