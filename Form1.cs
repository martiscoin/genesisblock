using System.Security.Policy;
using System;
using Marscore.NBitcoin;
using Marscore.Consensus.BlockInfo;
using Marscore.Consensus;
using Marscore.Networks;
using Marscore.Networks.Marscoin.Consensus;

namespace computegenesisblock
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        uint GenesisTime;
        uint GenesisNonce;
        Target GenesisBits;
        int GenesisVersion;
        Money GenesisReward;
        Block Genesis;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.GenesisTime = Utils.DateTimeToUnixTime(new DateTime(2025, 1, 11, 10, 05, 00, DateTimeKind.Utc));
            this.GenesisNonce = 20301515;
            this.GenesisBits = new Target(new uint256("00000fffffffffffffffffffffffffffffffffffffffffffffffffffffffffff"));
            this.GenesisVersion = 1;
            this.GenesisReward = Money.Zero;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            this.textBox1.Enabled = false;
            var consensusFactory = new MarsConsensusFactory();
            this.Genesis = consensusFactory.ComputeGenesisBlock(this.GenesisTime, this.GenesisNonce, this.GenesisBits,
                this.GenesisVersion, this.GenesisReward, NetworkType.Mainnet, true);
            this.Invoke(new Action(() =>
            {
                if (Genesis != null)
                {
                    this.textBox1.Text = ($"Found: Nonce:{Genesis.Header.Nonce}, Hash: {Genesis.GetHash()}, Hash Merkle Root: {Genesis.Header.HashMerkleRoot}");
                }
                else
                {
                    this.textBox1.Text = ($"Not Found");
                }
                this.button1.Enabled = true;
                this.textBox1.Enabled = true;
            }));
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
