using Marscore.Consensus.BlockInfo;

namespace Marscore.Networks.Marscoin.Consensus
{
    public class MarsProvenBlockHeader : ProvenBlockHeader
    {
        public MarsProvenBlockHeader()
        {
        }

        public MarsProvenBlockHeader(PosBlock block, MarsBlockHeader x1BlockHeader) : base(block, x1BlockHeader)
        {
        }
    }
}