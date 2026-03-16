namespace ECommerceWebAPI.Expection
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string messsage) : base(messsage) { }
    }
}
