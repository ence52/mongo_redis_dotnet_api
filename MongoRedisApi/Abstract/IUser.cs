namespace MongoRedisApi.Abstract
{
    public interface IUser:IBaseModel
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
