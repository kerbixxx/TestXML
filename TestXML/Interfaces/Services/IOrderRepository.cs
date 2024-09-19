using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestXML.Models.Dto;

namespace TestXML.Interfaces.Services
{
    public interface IOrderProcessor
    {
        List<OrderXml> ReadOrdersFromXml(string filePath);
        void ProcessOrders(List<OrderXml> orders);
    }
}
