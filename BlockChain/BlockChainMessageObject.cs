using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain
{
    public class BlockChainMessageObject
    {
        public string Message { get; set; }
        public bool IsValid { get; set; }

        public override string ToString()
        {
            return $"Message: {Message} IsValid: {IsValid}";
        }
    }
}
