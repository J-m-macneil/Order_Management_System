namespace Application.Common.Models;

public class PaginationQuery
{
    private const int MaxPageSize = 100;

    private int _pageNumber = 1;
    private int _pageSize = 25;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value < 1)
            {
                _pageSize = 25;
                return;
            }

            _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }

    public int Skip => (PageNumber - 1) * PageSize;
}