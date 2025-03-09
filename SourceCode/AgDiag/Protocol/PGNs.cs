﻿using System.Drawing;
using System;

namespace AgDiag.Protocol
{
    public abstract class PGN
    {
        public byte[] Bytes { get; set; }

        public string ToHexString()
        {
            return BitConverter.ToString(Bytes);
        }
    }

    public class PGNs
    {
        //AutoSteerData
        public class CPGN_FE : PGN
        {
            private const int speedLo = 5;
            private const int speedHi = 6;
            private const int status = 7;
            private const int steerAngleLo = 8;
            private const int steerAngleHi = 9;
            private const int sc1to8 = 11;

            /// <summary>
            /// autoSteerData FE 254 speedHi=5 speedLo=6  status = 7 free = 8;
            /// steerAngleHi = 9 steerAngleLo = 10 tramControl = 11 sc1to8 = 12 sc9to16 = 13;
            /// </summary>
            public CPGN_FE()
            {
                Bytes = new byte[] { 0x80, 0x81, 0x7f, 0xFE, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0xCC };
            }

            public int Speed => Bytes[speedHi] << 8 | Bytes[speedLo];
            public byte Status => Bytes[status];
            public int SteerAngle => Bytes[steerAngleHi] << 8 | Bytes[steerAngleLo];

            public bool IsSectionOn(int section)
            {
                if (section < 1 || section > 8) throw new ArgumentOutOfRangeException(nameof(section));

                int bitmask = (1 << (section - 1));
                return (Bytes[sc1to8] & bitmask) != 0;
            }
        }

        //From steer module
        public class CPGN_FD : PGN
        {
            private const int actualLo = 5;
            private const int actualHi = 6;
            private const int headLo = 7;
            private const int headHi = 8;
            private const int rollLo = 9;
            private const int rollHi = 10;
            private const int switchStatus = 11;
            private const int pwm = 12;

            public CPGN_FD()
            {
                Bytes = new byte[] { 0x80, 0x81, 0x7f, 0xFD, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0xCC };
            }

            public int ActualSteerAngle => Bytes[actualHi] << 8 | Bytes[actualLo];
            public int Heading => Bytes[headHi] << 8 | Bytes[headLo];
            public int Roll => Bytes[rollHi] << 8 | Bytes[rollLo];
            public byte PWM => Bytes[pwm];
            public bool IsWorkSwitchOn => (Bytes[switchStatus] & 1) != 0;
            public bool IsSteerSwitchOn => (Bytes[switchStatus] & 2) != 0;
        }

        //AutoSteer Settings
        public class CPGN_FC : PGN
        {
            private const int gainProportional = 5;
            private const int highPWM = 6;
            private const int lowPWM = 7;
            private const int minPWM = 8;
            private const int countsPerDegree = 9;
            private const int wasOffsetLo = 10;
            private const int wasOffsetHi = 11;
            private const int ackerman = 12;

            /// <summary>
            /// PGN - 252 - FC gainProportional=5 HighPWM=6  LowPWM = 7 MinPWM = 8 
            /// CountsPerDegree = 9 wasOffsetHi = 10 wasOffsetLo = 11 
            /// </summary>
            public CPGN_FC()
            {
                Bytes = new byte[] { 0x80, 0x81, 0x7f, 0xFC, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0xCC };
            }

            public byte GainProportional => Bytes[gainProportional];
            public byte HighPWM => Bytes[highPWM];
            public byte LowPWM => Bytes[lowPWM];
            public byte MinPWM => Bytes[minPWM];
            public byte CountsPerDegree => Bytes[countsPerDegree];
            public int SteerOffset => Bytes[wasOffsetHi] << 8 | Bytes[wasOffsetLo];
            public byte Ackerman => Bytes[ackerman];
        }

        //Autosteer Board Config
        public class CPGN_FB : PGN
        {
            private const int set0 = 5;
            private const int maxPulse = 6;
            private const int minSpeed = 7;

            /// <summary>
            /// PGN - 251 - FB 
            /// set0=5 maxPulse = 6 minSpeed = 7 ackermanFix = 8
            /// </summary>
            public CPGN_FB()
            {
                Bytes = new byte[] { 0x80, 0x81, 0x7f, 0xFB, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0xCC };
            }

            public byte Set0 => Bytes[set0];
            public byte MaxPulse => Bytes[maxPulse];
            public byte MinSpeed => Bytes[minSpeed];
        }

        //Machine Data
        public class CPGN_EF : PGN
        {
            private const int speed = 6;

            /// <summary>
            /// PGN - 239 - EF 
            /// uturn=5  tree=6  hydLift = 8 
            /// </summary>
            public CPGN_EF()
            {
                Bytes = new byte[] { 0x80, 0x81, 0x7f, 0xEF, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0xCC };
            }

            public byte Speed => Bytes[speed];
        }

        //Machine Config
        public class CPGN_EE : PGN
        {
            public int raiseTime = 5;
            public int lowerTime = 6;
            public int enableHyd = 7;
            public int set0 = 8;

            /// <summary>
            /// PGN - 238 - EE 
            /// raiseTime=5  lowerTime=6   enableHyd= 7 set0 = 8
            /// </summary>
            public CPGN_EE()
            {
                Bytes = new byte[] { 0x80, 0x81, 0x7f, 0xEE, 3, 0, 0, 0, 0xCC };
            }
        }

        //pgn instances

        /// <summary>
        /// autoSteerData - FE - 254 - 
        /// </summary>
        public CPGN_FE asData = new CPGN_FE();

        /// <summary>
        /// autoSteerReturn - FD - 253 - 
        /// </summary>
        public CPGN_FD asModule = new CPGN_FD();

        /// <summary>
        /// autoSteerSettings PGN - 252 - FC
        /// </summary>
        public CPGN_FC asSet = new CPGN_FC();

        /// <summary>
        /// autoSteerConfig PGN - 251 - FB
        /// </summary>
        public CPGN_FB asConfig = new CPGN_FB();

        /// <summary>
        /// machineData PGN - 239 - EF
        /// </summary>
        public CPGN_EF maData = new CPGN_EF();

        /// <summary>
        /// machineConfig PGN - 238 - EE
        /// </summary>
        public CPGN_EE maConfig = new CPGN_EE();
    }
}
