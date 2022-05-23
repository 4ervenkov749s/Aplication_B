using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication_B.BL
{
    public interface IPersonDataFlow
    {
        //Task SendPerson(byte[] data);
        Task SendPerson(byte[] person);

    }
}
