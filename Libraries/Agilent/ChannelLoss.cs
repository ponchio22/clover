using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Valutech.Wtm
{
    public class ChannelLoss
    {
        private double rxFrequencyLoss = -14;

        private double txFrequencyLoss = -10;

        private const double DEFAULT_TXRX_DIFF = -4;

        public Channel channel;

        public ChannelLoss(Channel channel)
        {
            this.channel = channel;
        }

        public ChannelLoss(Channel channel,double rxFrequencyLoss,double txFrequencyLoss) {
            this.channel = channel;
            this.rxFrequencyLoss = rxFrequencyLoss;
            this.txFrequencyLoss = txFrequencyLoss;
        }

        public double RxFrequencyLoss
        {
            set
            {
                this.rxFrequencyLoss = value;
            }
            get
            {
                return this.rxFrequencyLoss;
            }
        }

        public double TxFrequencyLoss
        {
            set
            {
                this.txFrequencyLoss = value;
            }
            get
            {
                return this.txFrequencyLoss;
            }
        }
    }
}
