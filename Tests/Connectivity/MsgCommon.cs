using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Asclepius.Connectivity.Messages
{
    public class MsgCommon
    {
        //Process of adding a message: Add size to dict->Event->Struct->Modify constructor

        #region "Common variables"

        internal const int BUFFER_SIZE = 256;

        internal static Dictionary<byte, Int16> dictMessages = new Dictionary<byte, Int16> 
        { 
            {100,sizeof(Int16)+1}, //TEST CMD

        };

        #endregion

        #region "Message Structs"

        internal class Msg_Test
        {
            public byte ID = 100;
            //Constructors
            public Msg_Test(byte[] bData) { }

            public Msg_Test() { }

            //To byte array
            public byte[] ToByteArray
            {
                get
                {
                    var bData = new byte[dictMessages[ID]];
                    var objStream = new MemoryStream(bData);
                    var objWrite = new BinaryWriter(objStream);

                    try
                    {
                        objWrite.Write(ID);
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        objWrite.Flush();
                        objWrite.Dispose();
                        objStream.Dispose();
                    }

                    return bData;
                }
            }
        }
        
        #endregion

    }
}
