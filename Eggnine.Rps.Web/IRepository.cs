//  ©️ 2024 by RF At EggNine All Rights Reserved
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Eggnine.Rps.Web;

public interface IRepository<T>
{
    public Task<T?> GetAsync(Func<T, bool> query, CancellationToken cancellationToken = default);

    public Task<bool> AddAsync(T t, CancellationToken cancellationToken = default);

    public Task<bool> UpdateAsync(T t, CancellationToken cancellationToken = default);
}