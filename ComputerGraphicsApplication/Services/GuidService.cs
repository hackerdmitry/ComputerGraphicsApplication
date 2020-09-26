using System;
using ComputerGraphicsApplication.Interfaces;

namespace ComputerGraphicsApplication.Services
{
    public class GuidService : IService
    {
        public string Create()
        {
            return Guid.NewGuid().ToString();
        }
    }
}