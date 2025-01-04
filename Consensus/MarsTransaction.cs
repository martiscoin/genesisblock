using Marscore.Consensus.TransactionInfo;

namespace Marscore.Networks.Marscoin.Consensus
{
    public class MarsTransaction : Transaction
    {
        public override bool IsProtocolTransaction()
        {
            return this.IsCoinBase || this.IsCoinStake;
        }
    }
}