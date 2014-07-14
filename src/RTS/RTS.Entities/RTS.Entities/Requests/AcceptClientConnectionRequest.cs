using RTS.Networking.Helios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Requests
{
    public class AcceptClientConnectionRequest
    {
        public Helios.Net.IConnection Connection { get; set; }
    }
}
