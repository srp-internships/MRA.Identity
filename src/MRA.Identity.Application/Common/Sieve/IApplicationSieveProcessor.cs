using MRA.Identity.Application.Contract.Common;
using Sieve.Models;
using Sieve.Services;

namespace MRA.Identity.Application.Common.Sieve;

public interface IApplicationSieveProcessor : ISieveProcessor
{
    PagedList<TResult> ApplyAdnGetPagedList<TSource, TResult>(SieveModel model, IQueryable<TSource> source,
        Func<TSource, TResult> converter);
}