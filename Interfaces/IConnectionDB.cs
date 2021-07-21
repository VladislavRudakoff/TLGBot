namespace TLGBot.Interfaces
{
    public interface IConnectionDB
    {
        public void Registration(string chatId, string userName);
        public void Delete(string chatId);
        public string UserRequest(string chatId);
        public string Read(string chatId);
        public void Update(string request, string chatId);
    }
}