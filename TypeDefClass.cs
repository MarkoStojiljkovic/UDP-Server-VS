using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomTypes
{
    struct senderInfo
    {
        // header
        public string seq; // Packet stamp
        public string mac; // MAC address
        public string ip; // IP of a sender
        public string senderError; // Error transimet by sender
        public string crc;

        //data
        public string name; // Alarm name
        public int address; // Not sure what it is
        public int value; // Alarm state, 1 or 0
        public string valid; // Validity 
        public string time; // Timestamp
        public string comment; // Comment for alarm

        // Error code
        public string errorCode; // Error code, if something goes wrong
        public bool error; // Indicates is there error or not


        // Router IP address
        public string trueIPAddress;
    }


    class TypeDefClass
    {
    }
}
