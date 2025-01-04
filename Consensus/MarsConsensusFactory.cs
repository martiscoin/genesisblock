using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Marscore.Consensus.BlockInfo;
using Marscore.Consensus.ScriptInfo;
using Marscore.Consensus.TransactionInfo;
using Marscore.NBitcoin;
using Marscore.NBitcoin.DataEncoders;
using static System.Windows.Forms.Design.AxImporter;

namespace Marscore.Networks.Marscoin.Consensus
{
    public class MarsConsensusFactory : PosConsensusFactory
    {
        public override BlockHeader CreateBlockHeader()
        {
            return new MarsBlockHeader();
        }

        public override ProvenBlockHeader CreateProvenBlockHeader()
        {
            return new MarsProvenBlockHeader();
        }

        public override ProvenBlockHeader CreateProvenBlockHeader(PosBlock block)
        {
            var provenBlockHeader = new MarsProvenBlockHeader(block, (MarsBlockHeader)this.CreateBlockHeader());

            // Serialize the size.
            provenBlockHeader.ToBytes(this);

            return provenBlockHeader;
        }

        public override Transaction CreateTransaction()
        {
            return new MarsTransaction();
        }

        public override Transaction CreateTransaction(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            var transaction = new MarsTransaction();
            transaction.ReadWrite(bytes, this);
            return transaction;
        }

        public override Transaction CreateTransaction(string hex)
        {
            if (hex == null)
                throw new ArgumentNullException(nameof(hex));

            return CreateTransaction(Encoders.Hex.DecodeData(hex));
        }

        public Block ComputeGenesisBlock(uint genesisTime, uint genesisNonce, uint genesisBits, int genesisVersion, Money genesisReward, NetworkType networkType, bool? mine = false)
        {
            Block findGenesis = null;
            uint loopLength = 20_000_000;
            ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 4 };
            Parallel.For(1, 4, options, (index, state) => {
                uint nonce = (uint)index * loopLength;
                var end = nonce + loopLength;
                while (nonce < end)
                {
                    string pszTimestamp = "https://www.marscoin.network";
                    Transaction txNew = CreateTransaction();
                    Debug.Assert(txNew.GetType() == typeof(MarsTransaction));
                    txNew.Version = 1;
                    txNew.AddInput(new TxIn()
                    {
                        ScriptSig = new Script(Op.GetPushOp(0), new Op()
                        {
                            Code = (OpcodeType)0x1,
                            PushData = new[] { (byte)42 }
                        }, Op.GetPushOp(Encoding.UTF8.GetBytes(pszTimestamp)))
                    });
                    txNew.AddOutput(new TxOut()
                    {
                        Value = genesisReward,
                    });
                    Block genesis = CreateBlock();
                    genesis.Header.BlockTime = Utils.UnixTimeToDateTime(genesisTime);
                    genesis.Header.Bits = genesisBits;
                    genesis.Header.Nonce = nonce;
                    genesis.Header.Version = genesisVersion;
                    genesis.Transactions.Add(txNew);
                    genesis.Header.HashPrevBlock = uint256.Zero;
                    genesis.UpdateMerkleRoot();
                    var hash = genesis.GetHash().ToString();
                    if (hash.StartsWith("00000"))
                    {
                        state.Stop();
                        findGenesis = genesis;
                        return;
                    }
                    else
                    {
                        ++nonce;
                    }
                    if (state.IsStopped) return;
                }
            });
            return findGenesis;
        }
    }
}