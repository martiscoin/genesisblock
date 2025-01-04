using System.IO;
using Marscore.Consensus.BlockInfo;
using Marscore.NBitcoin;
using Marscore.NBitcoin.Crypto;
using DBreeze.Utils;

namespace Marscore.Networks.Marscoin.Consensus
{
    public class MarsBlockHeader : PosBlockHeader
    {
        public override uint256 GetPoWHash()
        {
            byte[] serialized;

            using (var ms = new MemoryStream())
            {
                this.ReadWriteHashingStream(new BitcoinStream(ms, true));
                serialized = ms.ToArray();
            }

            serialized[76] = 0;
            serialized[77] = 0;
            serialized[78] = 0;
            serialized[79] = 0;

            var tempBuffer = MarsHashX13.Instance.Hash(serialized);
            serialized = tempBuffer.Concat(new byte[16]);

            var bytes = BitConverter.GetBytes(this.Nonce);
            serialized[76] = bytes[0];
            serialized[77] = bytes[1];
            serialized[78] = bytes[2];
            serialized[79] = bytes[3];

            return Sha512T.GetHash(serialized);
        }
    }
}