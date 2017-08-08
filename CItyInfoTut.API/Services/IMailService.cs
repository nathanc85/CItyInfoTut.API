using System;
namespace CItyInfoTut.API.Services
{
    public interface IMailService
    {
        void Send(string subject, string message);
    }
}
