using AutoMapper;
using PermissionManager.Core.Pagination;

namespace PermissionManager.Application.Mappings;

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