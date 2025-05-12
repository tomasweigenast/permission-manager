export default interface IPagedList<T> {
    items: T[];
    hasMore: boolean;
}