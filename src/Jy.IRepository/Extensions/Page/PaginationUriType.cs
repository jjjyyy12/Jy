

namespace Jy.IRepositories
{
    public enum PaginationUriType
    {
        CurrentPage,
        PreviousPage,
        NextPage
    }
    /*
             private string CreateProductUri(PaginationBase parameters, PaginationUriType uriType)
        {
            switch (uriType)
            {
                case PaginationUriType.PreviousPage:
                    var previousParameters = parameters.Clone();
                    previousParameters.PageIndex--;
                    return _urlHelper.Link("GetPagedProducts", previousParameters);
                case PaginationUriType.NextPage:
                    var nextParameters = parameters.Clone();
                    nextParameters.PageIndex++;
                    return _urlHelper.Link("GetPagedProducts", nextParameters);
                case PaginationUriType.CurrentPage:
                default:
                    return _urlHelper.Link("GetPagedProducts", parameters);
            }
        }
     */

}
