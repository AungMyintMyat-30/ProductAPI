namespace ProductCore.Models
{
    public record PaginationResultModel<T>(int RecordsTotal, T Records);
}
