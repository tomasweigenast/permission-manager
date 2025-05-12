using AutoMapper;
using PermissionManager.Core.Pagination;

namespace PermissionManager.Application.Mappings;

/// <summary>
/// An AutoMapper <see cref="ITypeConverter{TSource,TDestination}"/> that handles
/// <see cref="PagedList{T}"/>
/// </summary>
public class PagedListConverter<TSource, TDestination> : ITypeConverter<PagedList<TSource>, PagedList<TDestination>>
{
    public PagedList<TDestination> Convert(PagedList<TSource> source, PagedList<TDestination> destination, ResolutionContext context)
    {
        var items = context.Mapper.Map<List<TDestination>>(source.ToList());

        return new PagedList<TDestination>(
            items,
            source.HasNextPage
        );
    }
}