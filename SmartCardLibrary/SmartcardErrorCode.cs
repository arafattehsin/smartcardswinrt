﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SmartCardLibrary
{
    public enum SmartcardErrorCode : uint
    {
        None = 0,
        InternalError = 2148532225,
        Canceled = 2148532226,
        InvalidHandle = 2148532227,
        InvalidParameter = 2148532228,
        InvalidTarget = 2148532229,
        NoMemory = 2148532230,
        WaitedTooLong = 2148532231,
        InsufficientBuffer = 2148532232,
        UnknownReader = 2148532233,
        Timeout = 2148532234,
        SharingViolation = 2148532235,
        NoSmartcard = 2148532236,
        UnknownCard = 2148532237,
        CannotDispose = 2148532238,
        ProtocolMismatch = 2148532239,
        NotReady = 2148532240,
        InvalidValue = 2148532241,
        SystemCanceled = 2148532242,
        CommunicationError = 2148532243,
        UnknownError = 2148532244,
        InvalidAttribute = 2148532245,
        NotTransacted = 2148532246,
        ReaderUnavailable = 2148532248,
        Shutdown = 2148532248,
        PCITooSmall = 2148532249,
        ReaderUnsupported = 2148532250,
        DuplicateReader = 2148532251,
        CardUnsupported = 2148532252,
        NoService = 2148532253,
        ServiceStopped = 2148532254,
        Unexpected = 2148532255,
        ICCInstallation = 2148532256,
        ICCCreationOrder = 2148532257,
        UnsupportedFeature = 2148532258,
        DirectoryNotFound = 2148532259,
        FileNotFound = 2148532260,
        NoDirectory = 2148532261,
        NoFile = 2148532262,
        NoAccess = 2148532263,
        WriteTooMany = 2148532264,
        BadSeek = 2148532265,
        InvalidPin = 2148532266,
        UnknownResourceManagement = 2148532267,
        NoSuchCertificate = 2148532268,
        CertificateUnavailable = 2148532269,
        NoReadersAvailable = 2148532270,
        CommunicationDataLast = 2148532271,
        NoKeyContainer = 2148532272,
        ServerTooBusy = 2148532273,
        UnsupportedCard = 2148532325,
        UnresponsiveCard = 2148532326,
        UnpoweredCard = 2148532327,
        ResetCard = 2148532328,
        RemovedCard = 2148532329,
        SecurityViolation = 2148532330,
        WrongPin = 2148532331,
        PinBlocked = 2148532332,
        EndOfFile = 2148532333,
        CanceledByUser = 2148532334,
        CardNotAuthenticated = 2148532335
    }    
}
