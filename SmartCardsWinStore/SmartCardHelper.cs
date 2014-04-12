using SmartCardLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public enum SmartcardState
    {
        None = 0,
        Inserted = 1,
        Ejected = 2
    }

    public class SmartCardHelper
    {
        int retCode, hCard, Protocol, Aprotocol;
        UnsafeNativeMethods.SCARD_IO_REQUEST pioSendRequest;

        private SmartcardContextSafeHandle _context;
        private SmartcardErrorCode _lastErrorCode;
        private ReaderState[] _readers;

        public bool HasContext
        {
            get { return (!this._context.IsInvalid); }
        }  

        public ReaderState[] Readers
        {
            get { return this._readers; }
        }

        public SmartcardContextSafeHandle Context
        {
            get { return this._context; }
        }

        public SmartCardHelper()
        {
            _context = new SmartcardContextSafeHandle();
            _readers = GetCurrentReader();
        }

        #region Helper Methods for Smart Card Helper.
        private bool EstablishContext()
        {
            if ((HasContext))
            {
                return true;
            }
            _lastErrorCode =
                (SmartcardErrorCode)UnsafeNativeMethods.EstablishContext(ScopeOption.System,
                IntPtr.Zero, IntPtr.Zero, ref _context);
            return (_lastErrorCode == SmartcardErrorCode.None);
        }

        private int GetReaderListBufferSize()
        {
            if ((_context.IsInvalid))
            {
                return 0;
            }
            int result = 0;
            _lastErrorCode =
                (SmartcardErrorCode)UnsafeNativeMethods.ListReaders(
                _context, null, null, ref result);
            return result;
        }

        private List<string> GetReaders()
        {
            List<string> result = new List<string>();

            //Make sure a context has been established before 
            //retrieving the list of smartcard readers.
            if (EstablishContext())
            {
                //Ask for the size of the buffer first.
                int size = GetReaderListBufferSize();

                //Allocate a string of the proper size in which 
                //to store the list of smartcard readers.
                string readerList = new string('\0', size);
                //Retrieve the list of smartcard readers.
                _lastErrorCode =
                    (SmartcardErrorCode)UnsafeNativeMethods.ListReaders(_context,
                    null, readerList, ref size);
                if ((_lastErrorCode == SmartcardErrorCode.None))
                {
                    //Extract each reader from the returned list.
                    //The readerList string will contain a multi-string of 
                    //the reader names, i.e. they are seperated by 0x00 
                    //characters.
                    string readerName = string.Empty;
                    for (int i = 0; i <= readerList.Length - 1; i++)
                    {
                        if ((readerList[i] == '\0'))
                        {
                            if ((readerName.Length > 0))
                            {
                                //We have a smartcard reader's name.
                                result.Add(readerName);
                                readerName = string.Empty;
                            }
                        }
                        else
                        {
                            //Append the found character.
                            readerName += new string(readerList[i], 1);
                        }
                    }
                }
            }
            return result;
        } 

        #endregion

        public bool Connect(SmartCardHelper smartCard, string readerName)
        {
            retCode = UnsafeNativeMethods.SCardConnect(smartCard._context, readerName, UnsafeNativeMethods.SCARD_SHARE_SHARED,
                                              UnsafeNativeMethods.SCARD_PROTOCOL_T0 | UnsafeNativeMethods.SCARD_PROTOCOL_T1, ref smartCard.hCard, ref smartCard.Protocol);

            if (retCode != UnsafeNativeMethods.SCARD_S_SUCCESS)
                return false;
            else
                return true;
        }

        public string GetCardID(SmartCardHelper smartCard)
        {
            smartCard.pioSendRequest.dwProtocol = Aprotocol;
            smartCard.pioSendRequest.cbPciLength = 8;

            byte[] receivedUID = new byte[10];
            byte[] sendBytes = new byte[] { 0xFF, 0xCA, 0x00, 0x00, 0x04 }; //get UID command      for Mifare cards

            int outBytes = receivedUID.Length;
            int retCode = UnsafeNativeMethods.SCardTransmit(smartCard.hCard, ref smartCard.pioSendRequest, ref sendBytes[0], sendBytes.Length, ref smartCard.pioSendRequest, ref receivedUID[0], ref outBytes);
            string ID = BitConverter.ToString(receivedUID.Take(8).ToArray());

            if (retCode != UnsafeNativeMethods.SCARD_S_SUCCESS)
            {
                return string.Empty;
            }

            return ID;
        }

        public ReaderState[] GetCurrentReader()
        {
            var availableReaders = GetReaders();
            ReaderState[] readers = new ReaderState[availableReaders.Count];
            for (int i = 0; i <= availableReaders.Count - 1; i++)
            {
                readers[i].Reader = availableReaders[i].ToString();
            }

            return readers;
        }

        public SmartcardErrorCode GetStatusChange(SmartCardHelper smartCard)
        {
           return (SmartcardErrorCode)UnsafeNativeMethods.GetStatusChange(
                        smartCard.Context, 1000, smartCard.Readers, smartCard.Readers.Length);
        }
    }
}
