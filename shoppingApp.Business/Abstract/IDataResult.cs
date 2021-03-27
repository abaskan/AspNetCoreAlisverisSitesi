namespace shoppingApp.Business.Abstract
{
    public interface IDataResult <T> : IResult
    {
        T Data { get ; }
    }
}